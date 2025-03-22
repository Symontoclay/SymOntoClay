/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Common.Disposing;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using SymOntoClay.UnityAsset.Core.Internal.DateAndTime;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.Images;
using SymOntoClay.UnityAsset.Core.Internal.LogicQueryParsingAndCache;
using SymOntoClay.UnityAsset.Core.Internal.ModulesStorage;
using SymOntoClay.UnityAsset.Core.Internal.Storage;
using SymOntoClay.UnityAsset.Core.Internal.Threads;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters;
using SymOntoClay.UnityAsset.Core.Internal.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public class WorldContext: IWorldCoreContext, IWorldCoreGameComponentContext, ISymOntoClayDisposable
    {
        public void SetSettings(WorldSettings settings)
        {
            WorldSettingsValidator.Validate(settings);

            ImplementGeneralSettings(settings);
            CreateMonitoring(settings);
            CreateComponents(settings);

            if (settings.EnableAutoloadingConvertors)
            {
                LoadTypesPlatformTypesConvertors();
            }

            _isInitialized = true;
        }
        
        private void ImplementGeneralSettings(WorldSettings settings)
        {
            _tmpDir = settings.TmpDir;

            Directory.CreateDirectory(_tmpDir);

            _cancellationTokenSource = new CancellationTokenSource();
            _linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, settings?.CancellationToken ?? CancellationToken.None);

            ThreadingSettings = settings.ThreadingSettings;

            var threadingSettings = settings.ThreadingSettings?.AsyncEvents;

            AsyncEventsThreadPool = new CustomThreadPool(threadingSettings?.MinThreadsCount ?? DefaultCustomThreadPoolSettings.MinThreadsCount,
                threadingSettings?.MaxThreadsCount ?? DefaultCustomThreadPoolSettings.MaxThreadsCount,
                _linkedCancellationTokenSource.Token);

            InvokerInMainThread = settings.InvokerInMainThread;
            SoundBus = settings.SoundBus;
            StandardFactsBuilder = settings.StandardFactsBuilder;

            HtnExecutionSettings = settings.HtnExecutionSettings;
        }

        private void CreateMonitoring(WorldSettings settings)
        {
            Monitor = settings.Monitor;            
            MonitorNode = Monitor.CreateMotitorNode("6B299F25-9FD9-46BE-A833-9C52B279444F", "world");
            Logger = MonitorNode;
        }

        private void CreateComponents(WorldSettings settings)
        {
            NLPConverterFactory = settings.NLPConverterProvider?.GetFactory(Logger);

            ImagesRegistry = new ImagesRegistry(this);
            ThreadsComponent = new ThreadsCoreComponent(this);
            PlatformTypesConvertorsRegistry = new PlatformTypesConvertersRegistry(Logger);
            DateTimeProvider = new DateTimeProvider(Logger, ThreadsComponent, AsyncEventsThreadPool, _linkedCancellationTokenSource.Token);
            LogicQueryParseAndCache = new LogicQueryParseAndCache(settings, this);

            ModulesStorage = new ModulesStorageComponent(settings, this);
            StandaloneStorage = new StandaloneStorageComponent(settings, this);
            ModulesStorage.Init(StandaloneStorage.StandaloneStorage.Context);
        }
        
        private void LoadTypesPlatformTypesConvertors()
        {
            var targetAttributeType = typeof(PlatformTypesConverterAttribute);

            var typesList = AppDomainTypesEnumerator.GetTypes().Where(p => p.GetCustomAttributesData().Any(x => x.AttributeType == targetAttributeType)).ToList();

            foreach (var type in typesList)
            {
                var convertor = (IPlatformTypesConverter)Activator.CreateInstance(type);

                PlatformTypesConvertorsRegistry.AddConvertor(Logger, convertor);
            }
        }

        public void RunInMainThread(Action function)
        {
            InvokerInMainThread.RunInMainThread(function);
        }

        public TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            return InvokerInMainThread.RunInMainThread(function);
        }
        
        public void AddConvertor(IPlatformTypesConverter convertor)
        {
            PlatformTypesConvertorsRegistry.AddConvertor(Logger, convertor);
        }

        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        private string _tmpDir;
        public string TmpDir => _tmpDir;

        public IMonitor Monitor { get; private set; }
        public IMonitorNode MonitorNode { get; private set; }
        public IMonitorLogger Logger { get; private set; }

        /// <inheritdoc/>
        IMonitor IWorldCoreGameComponentContext.Motitor => Monitor;

        public ImagesRegistry ImagesRegistry { get; private set; }
        public ThreadsCoreComponent ThreadsComponent { get; private set; }

        IActiveObjectCommonContext IWorldCoreGameComponentContext.SyncContext => ThreadsComponent;

        public ModulesStorageComponent ModulesStorage { get; private set; }

        IModulesStorage IWorldCoreGameComponentContext.ModulesStorage => ModulesStorage.ModulesStorage;
        IModulesStorage IWorldCoreContext.ModulesStorage => ModulesStorage.ModulesStorage;

        public StandaloneStorageComponent StandaloneStorage { get; private set; }

        IStandaloneStorage IWorldCoreGameComponentContext.StandaloneStorage => StandaloneStorage.StandaloneStorage;

        public PlatformTypesConvertersRegistry PlatformTypesConvertorsRegistry { get; private set; }
        IPlatformTypesConvertersRegistry IWorldCoreContext.PlatformTypesConvertors => PlatformTypesConvertorsRegistry;
        IPlatformTypesConvertersRegistry IWorldCoreGameComponentContext.PlatformTypesConvertors => PlatformTypesConvertorsRegistry;

        public INLPConverterFactory NLPConverterFactory { get; private set; }
        INLPConverterFactory IWorldCoreGameComponentContext.NLPConverterFactory => NLPConverterFactory;

        public IStandardFactsBuilder StandardFactsBuilder { get; private set; }
        IStandardFactsBuilder IWorldCoreGameComponentContext.StandardFactsBuilder => StandardFactsBuilder;

        /// <inheritdoc/>
        public IInvokerInMainThread InvokerInMainThread { get; private set; }

        /// <inheritdoc/>
        public ThreadingSettings ThreadingSettings { get; private set; }

        /// <inheritdoc/>
        public HtnExecutionSettings HtnExecutionSettings { get; private set; }

        /// <inheritdoc/>
        public ICustomThreadPool AsyncEventsThreadPool { get; private set; }

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _linkedCancellationTokenSource;

        /// <inheritdoc/>
        public CancellationToken GetCancellationToken()
        {
            return _linkedCancellationTokenSource.Token;
        }

        /// <inheritdoc/>
        public ISoundBus SoundBus { get; private set; }

        public DateTimeProvider DateTimeProvider { get; private set; }
        IDateTimeProvider IWorldCoreGameComponentContext.DateTimeProvider => DateTimeProvider;
        IDateTimeProvider IWorldCoreContext.DateTimeProvider => DateTimeProvider;

        public LogicQueryParseAndCache LogicQueryParseAndCache { get; private set; }
        ILogicQueryParseAndCache IWorldCoreGameComponentContext.LogicQueryParseAndCache => LogicQueryParseAndCache;
        ILogicQueryParseAndCache IWorldCoreContext.LogicQueryParseAndCache => LogicQueryParseAndCache;

        private readonly object _worldComponentsListLockObj = new object();
        private readonly List<IWorldCoreComponent> _worldComponentsList = new List<IWorldCoreComponent>();

        /// <inheritdoc/>
        void IWorldCoreContext.AddWorldComponent(IWorldCoreComponent component)
        {
            lock(_worldComponentsListLockObj)
            {
                if(_worldComponentsList.Contains(component))
                {
                    return;
                }

                _worldComponentsList.Add(component);
            }
        }

        private readonly object _gameComponentsListLockObj = new object();
        private readonly List<IGameComponent> _gameComponentsList = new List<IGameComponent>();
        private readonly List<int> _availableInstanceIdList = new List<int>();
        private readonly Dictionary<int, IGameComponent> _gameComponentsDictByInstanceId = new Dictionary<int, IGameComponent>();
        private readonly Dictionary<string, int> _instancesIdDict = new Dictionary<string, int>();

        private readonly List<IGameComponent> _gameComponentsForLateInitializingList = new List<IGameComponent>();

        /// <inheritdoc/>
        void IWorldCoreGameComponentContext.AddGameComponent(IGameComponent component)
        {
            lock(_gameComponentsListLockObj)
            {
                if (_gameComponentsList.Contains(component))
                {
                    return;
                }

                var instanceId = component.InstanceId;

                _availableInstanceIdList.Add(instanceId);
                _gameComponentsList.Add(component);
                _gameComponentsDictByInstanceId[instanceId] = component;
                _instancesIdDict[NameHelper.NormalizeString(component.Id)] = instanceId;

                if(_state == ComponentState.Started)
                {
                    if(!_gameComponentsForLateInitializingList.Contains(component))
                    {
                        _gameComponentsForLateInitializingList.Add(component);
                    }
                }
            }
        }

        /// <inheritdoc/>
        void IWorldCoreGameComponentContext.AddPublicFactsStorage(IGameComponent component)
        {
            lock (_gameComponentsListLockObj)
            {
                var publicFactsStorage = component.PublicFactsStorage;

                StandaloneStorage.StandaloneStorage.WorldPublicFactsStorage.AddConsolidatedStorage(Logger, publicFactsStorage);
            }
        }

        /// <inheritdoc/>
        void IWorldCoreGameComponentContext.RemoveGameComponent(IGameComponent component)
        {
            lock (_gameComponentsListLockObj)
            {
                if (_gameComponentsList.Contains(component))
                {
                    var instanceId = component.InstanceId;

                    _availableInstanceIdList.Remove(component.InstanceId);
                    _gameComponentsList.Remove(component);
                    _gameComponentsDictByInstanceId.Remove(instanceId);
                    _instancesIdDict.Remove(NameHelper.NormalizeString(component.Id));

                    var publicFactsStorage = component.PublicFactsStorage;

                    StandaloneStorage.StandaloneStorage.WorldPublicFactsStorage.RemoveConsolidatedStorage(Logger, publicFactsStorage);
                }
            }
        }

        /// <inheritdoc/>
        bool IWorldCoreGameComponentContext.CanBeTakenBy(int instanceId, IEntity subject)
        {
            lock (_gameComponentsListLockObj)
            {
                if(!_gameComponentsDictByInstanceId.ContainsKey(instanceId))
                {
                    return false;
                }

                return _gameComponentsDictByInstanceId[instanceId].CanBeTakenBy(Logger, subject);
            }
        }

        /// <inheritdoc/>
        Vector3? IWorldCoreGameComponentContext.GetPosition(int instanceId)
        {
            lock (_gameComponentsListLockObj)
            {
                if (!_gameComponentsDictByInstanceId.ContainsKey(instanceId))
                {
                    return null;
                }

                return _gameComponentsDictByInstanceId[instanceId].GetPosition(Logger);
            }
        }

        /// <inheritdoc/>
        IList<int> IWorldCoreGameComponentContext.AvailableInstanceIdList
        {
            get
            {
                lock (_gameComponentsListLockObj)
                {
                    return _availableInstanceIdList;
                }
            }
        }

        /// <inheritdoc/>
        IStorage IWorldCoreGameComponentContext.GetPublicFactsStorageByInstanceId(int instanceId)
        {
            lock (_gameComponentsListLockObj)
            {
                return _gameComponentsDictByInstanceId[instanceId].PublicFactsStorage;
            }
        }

        /// <inheritdoc/>
        string IWorldCoreGameComponentContext.GetIdForFactsByInstanceId(int instanceId)
        {
            lock (_gameComponentsListLockObj)
            {
                return _gameComponentsDictByInstanceId[instanceId].IdForFacts;
            }
        }

        /// <inheritdoc/>
        int IWorldCoreGameComponentContext.GetInstanceIdByIdForFacts(string id)
        {
            lock (_gameComponentsListLockObj)
            {
                if(_instancesIdDict.ContainsKey(id))
                {
                    return _instancesIdDict[id];
                }

                return 0;
            }
        }

        public bool EnableLogging { get => Monitor.Enable; set => Monitor.Enable = value; }

        public bool EnableRemoteConnection { get => Monitor.EnableRemoteConnection; set => Monitor.EnableRemoteConnection = value; }

        private void NLoadFromSourceCode()
        {
            ModulesStorage.LoadFromSourceCode();
            StandaloneStorage.LoadFromSourceCode();
            DateTimeProvider.LoadFromSourceCode();

            lock (_gameComponentsListLockObj)
            {
                foreach (var item in _gameComponentsList)
                {
                    item.LoadFromSourceCode();
                }
            }
        }

        public void Start()
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                ThreadsComponent.Lock();

                if (_state != ComponentState.Loaded)
                {
                    NLoadFromSourceCode();
                    Thread.Sleep(100);
                }

                NStart();
            }
        }

        private void NStart()
        {
            ThreadsComponent.Lock();

            DateTimeProvider.Start();

            lock (_gameComponentsListLockObj)
            {
                foreach (var item in _gameComponentsList)
                {
                    item.BeginStarting();
                }
            }

            WaitForAllGameComponentsWaiting();

            ThreadsComponent.UnLock();

            _state = ComponentState.Started;

            lock (_gameComponentsListLockObj)
            {
                foreach (var item in _gameComponentsList)
                {
                    item.EndStarting();
                }
            }

            var cancellationToken = _cancellationTokenSource.Token;

            ThreadTask.Run(() => {
                try
                {
                    while (true)
                    {
                        lock (_gameComponentsListLockObj)
                        {
                            if (_gameComponentsForLateInitializingList.Any())
                            {
                                foreach (var component in _gameComponentsForLateInitializingList)
                                {
                                    component.LoadFromSourceCode();
                                    component.BeginStarting();
                                    component.EndStarting();
                                }

                                _gameComponentsForLateInitializingList.Clear();
                            }
                        }

                        if (_cancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception e)
                {
                    Error("CDF6BAD4-76E3-4B1F-9379-C64BF752F9AE", e);
                }
            }, AsyncEventsThreadPool, cancellationToken);
        }
        
        private void WaitForAllGameComponentsWaiting()
        {
            lock (_gameComponentsListLockObj)
            {
                while(!_gameComponentsList.All(p => p.IsWaited))
                {
                    Thread.Sleep(10);
                }
            }
        }

        public void Stop()
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                throw new NotImplementedException("6136D992-B17A-4FB0-97D1-C647D733FCF3");
            }
        }

        public bool IsActive
        {
            get
            {
                lock (_stateLockObj)
                {
                    return _state == ComponentState.Started; 
                }
            }
        }

        private ComponentState _state = ComponentState.Created;
        private readonly object _stateLockObj = new object();

        /// <inheritdoc/>
        public bool IsDisposed
        {
            get
            {
                lock (_stateLockObj)
                {
                    return _state == ComponentState.Disposed;
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    return;
                }

                _state = ComponentState.Disposed;
            }

            _cancellationTokenSource?.Cancel();

            lock (_gameComponentsListLockObj)
            {
                foreach (var item in _gameComponentsList.ToList())
                {
                    item.Dispose();
                }
            }

            lock (_worldComponentsListLockObj)
            {
                foreach (var item in _worldComponentsList)
                {
                    item.Dispose();
                }
            }

            Monitor.Dispose();
        }

        protected void Trace(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Trace(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        protected void Debug(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Debug(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        protected void Info(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Info(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        protected void Warn(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Warn(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        protected void Error(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Error(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        protected void Error(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Error(messagePointId, exception, memberName, sourceFilePath, sourceLineNumber);
        }

        protected void Fatal(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Fatal(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        protected void Fatal(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Fatal(messagePointId, exception, memberName, sourceFilePath, sourceLineNumber);
        }
    }
}

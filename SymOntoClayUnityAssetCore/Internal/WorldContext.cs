/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Common.Cancellation;
using SymOntoClay.Common.Disposing;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.SerializationToImage;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using SymOntoClay.UnityAsset.Core.Internal.DateAndTime;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.LogicQueryParsingAndCache;
using SymOntoClay.UnityAsset.Core.Internal.ModulesStorage;
using SymOntoClay.UnityAsset.Core.Internal.Storage;
using SymOntoClay.UnityAsset.Core.Internal.Threads;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters;
using SymOntoClay.UnityAsset.Core.Internal.Validators;
using SymOntoClay.UnityAsset.Core.InternalImplementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public class WorldContext: IWorldCoreContext, IWorldCoreGameComponentContext, ISymOntoClayDisposable
    {
        public void SetSettings(WorldSettings settings)
        {
            WorldSettingsValidator.Validate(settings);

            _settings = settings;

            ImplementGeneralSettings();
            CreateMonitoring();
            CreateComponents();

            if (settings.EnableAutoloadingConvertors)
            {
                LoadTypesPlatformTypesConvertors();
            }

            _isInitialized = true;
        }
        
        private void ImplementGeneralSettings()
        {
            _tmpDir = _settings.TmpDir;

            Directory.CreateDirectory(_tmpDir);

            _cancellationTokenSourceContext = new CancellationTokenSourceContext();
            _linkedCancellationTokenSourceContext = new CancellationLinkedTokenSourceContext(_cancellationTokenSourceContext, _settings?.CancellationContext);

            WorldThreadingSettings = _settings.WorldThreadingSettings;
            HumanoidNpcDefaultThreadingSettings = _settings.HumanoidNpcDefaultThreadingSettings;
            PlayerDefaultThreadingSettings = _settings.PlayerDefaultThreadingSettings;
            GameObjectDefaultThreadingSettings = _settings.GameObjectDefaultThreadingSettings;
            PlaceDefaultThreadingSettings = _settings.PlaceDefaultThreadingSettings;

            var threadingSettings = _settings.WorldThreadingSettings?.AsyncEvents;

            AsyncEventsThreadPool = new CustomThreadPool(threadingSettings?.MinThreadsCount ?? DefaultCustomThreadPoolSettings.MinThreadsCount,
                threadingSettings?.MaxThreadsCount ?? DefaultCustomThreadPoolSettings.MaxThreadsCount,
                _linkedCancellationTokenSourceContext);

            InvokerInMainThread = _settings.InvokerInMainThread;
            SoundBus = _settings.SoundBus;
            StandardFactsBuilder = _settings.StandardFactsBuilder;

            HtnExecutionSettings = _settings.HtnExecutionDefaultSettings;
        }

        private void CreateMonitoring()
        {
            Monitor = _settings.Monitor;            
            MonitorNode = Monitor.CreateMonitorNode("6B299F25-9FD9-46BE-A833-9C52B279444F", "world");
            Logger = MonitorNode;
        }

        private void CreateComponents()
        {
            NLPConverterFactory = _settings.NLPConverterProvider?.GetFactory(Logger);

            ThreadsComponent = new ThreadsCoreComponent(this);
            PlatformTypesConvertorsRegistry = new PlatformTypesConvertersRegistry(Logger);

            //CreateWorldSerializedContext();
        }

        private void CreateWorldSerializedContext()
        {
            _serializedWorldContext?.Dispose();
            _serializedWorldContext = new SerializedWorldContext(this);
            _serializedWorldContext.Init();
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

        private WorldSettings _settings;

        WorldSettings IWorldCoreContext.WorldSettings => _settings;

        private string _tmpDir;
        public string TmpDir => _tmpDir;

        public IMonitor Monitor { get; private set; }
        public IMonitorNode MonitorNode { get; private set; }
        public IMonitorLogger Logger { get; private set; }

        /// <inheritdoc/>
        IMonitor IWorldCoreGameComponentContext.Motitor => Monitor;

        private SerializedWorldContext _serializedWorldContext;

        public ThreadsCoreComponent ThreadsComponent { get; private set; }

        IActiveObjectCommonContext IWorldCoreContext.SyncContext => ThreadsComponent;

        IActiveObjectCommonContext IWorldCoreGameComponentContext.SyncContext => ThreadsComponent;

        IModulesStorage IWorldCoreGameComponentContext.ModulesStorage => _serializedWorldContext.ModulesStorage.ModulesStorage;
        IModulesStorage IWorldCoreContext.ModulesStorage => _serializedWorldContext.ModulesStorage.ModulesStorage;

        IStandaloneStorage IWorldCoreGameComponentContext.StandaloneStorage => _serializedWorldContext.StandaloneStorage.StandaloneStorage;

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
        public ThreadingSettings WorldThreadingSettings { get; private set; }

        /// <inheritdoc/>
        public ThreadingSettings HumanoidNpcDefaultThreadingSettings { get; private set; }

        /// <inheritdoc/>
        public ThreadingSettings PlayerDefaultThreadingSettings { get; private set; }

        /// <inheritdoc/>
        public ThreadingSettings GameObjectDefaultThreadingSettings { get; private set; }

        /// <inheritdoc/>
        public ThreadingSettings PlaceDefaultThreadingSettings { get; private set; }

        /// <inheritdoc/>
        public ThreadingSettings GetDefaultThreadingSettings(KindOfWorldItem kindOfWorldItem)
        {
            switch(kindOfWorldItem)
            {
                case KindOfWorldItem.Player:
                    return PlayerDefaultThreadingSettings;

                case KindOfWorldItem.GameObject:
                    return GameObjectDefaultThreadingSettings;

                case KindOfWorldItem.Place:
                    return PlaceDefaultThreadingSettings;

                case KindOfWorldItem.HumanoidNPC:
                    return HumanoidNpcDefaultThreadingSettings;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfWorldItem), kindOfWorldItem, null);
            }
        }

        /// <inheritdoc/>
        public HtnExecutionSettings HtnExecutionSettings { get; private set; }

        /// <inheritdoc/>
        public ICustomThreadPool AsyncEventsThreadPool { get; private set; }

        private CancellationTokenSourceContext _cancellationTokenSourceContext;
        private ICancellationContext _linkedCancellationTokenSourceContext;
        private CancellationTokenSourceContext _startedCancellationContext;

        /// <inheritdoc/>
        public ICancellationContext GetCancellationContext()
        {
            return _linkedCancellationTokenSourceContext;
        }

        /// <inheritdoc/>
        public CancellationToken GetCancellationToken()
        {
            return _linkedCancellationTokenSourceContext.Token;
        }

        /// <inheritdoc/>
        public ISoundBus SoundBus { get; private set; }

        IDateTimeProvider IWorldCoreGameComponentContext.DateTimeProvider => _serializedWorldContext.DateTimeProvider;
        IDateTimeProvider IWorldCoreContext.DateTimeProvider => _serializedWorldContext.DateTimeProvider;

        
        ILogicQueryParseAndCache IWorldCoreGameComponentContext.LogicQueryParseAndCache => _serializedWorldContext.LogicQueryParseAndCache;
        ILogicQueryParseAndCache IWorldCoreContext.LogicQueryParseAndCache => _serializedWorldContext.LogicQueryParseAndCache;

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

        /// <inheritdoc/>
        void IWorldCoreContext.AddSerializedWorldComponent(ISerializedWorldCoreComponent component)
        {
            _serializedWorldContext.AddSerializedWorldComponent(component);
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

                _serializedWorldContext.AddPublicFactsStorage(publicFactsStorage);
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

                    _serializedWorldContext.RemoveGameComponent(publicFactsStorage);
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

        public void LoadFromSourceCode()
        {
            lock (_stateLockObj)
            {
                if (!GameComponentGuard.Check("F69A12D4-8B21-469B-9582-F7DE53C0466A", GetType().Name, ref _state, ThreadsComponent.WaitEvent, ComponentState.Loading))
                {
                    return;
                }

                NLoadFromSourceCode();
            }
        }

        private void NLoadFromSourceCode()
        {
#if DEBUG
            //Info("ABBEACF5-242F-487A-AE96-8C7F61B04B7E", $"_state = {_state}");
            //Info("17CB100F-3DB9-40E3-8EA5-CDF76ADC4272", $"ComponentStateHelper.IsStopped(_state) = {ComponentStateHelper.IsStopped(_state)}");
#endif

            if (!ComponentStateHelper.IsStopped(_state))
            {
                NStop();
            }

            CreateWorldSerializedContext();

            _serializedWorldContext.LoadFromSourceCode();

            lock (_gameComponentsListLockObj)
            {
                foreach (var item in _gameComponentsList)
                {
                    item.LoadFromSourceCode();
                }
            }

            _state = ComponentState.Loaded;
        }

        public void LoadFromImage(SerializationToImageSettings settings)
        {
            lock (_stateLockObj)
            {
                if (!GameComponentGuard.Check("2CCDA020-46EA-4234-A35A-8045F804A2B3", GetType().Name, ref _state, ThreadsComponent.WaitEvent, ComponentState.Loading))
                {
                    return;
                }

                throw new NotImplementedException("C06FA5F1-80F9-4365-A41E-182CFD35B497");
            }
        }

        public void SaveToImage(SerializationToImageSettings settings)
        {
            lock (_stateLockObj)
            {
                if (!GameComponentGuard.Check("7F8B10DD-8255-4110-8BD3-9F459276A78D", GetType().Name, ref _state, ThreadsComponent.WaitEvent, ComponentState.Saving))
                {
                    return;
                }

                if (!ComponentStateHelper.IsStopped(_state))
                {
                    NStop();
                }

                NSaveToImage(settings);
            }
        }

        private void NSaveToImage(SerializationToImageSettings settings)
        {
#if DEBUG
            Info("882C72DD-4D60-4A70-AB2C-44A5CCC51F15", $"settings = {settings}");
#endif

            throw new NotImplementedException("C87D7B69-F0F5-416E-8CBB-7D1727651172");
        }

        public void Start()
        {
            lock (_stateLockObj)
            {
                if(!GameComponentGuard.Check("A7EBF771-7201-43AB-8034-725E5CF81C82", GetType().Name, ref _state, ThreadsComponent.WaitEvent, ComponentState.Started))
                {
                    return;
                }

                if (!ComponentStateHelper.IsLoaded(_state))
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

            _serializedWorldContext.DateTimeProvider.Start();

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

            StartgameComponentsForLateInitializing();
        }
        
        private void StartgameComponentsForLateInitializing()
        {
            _startedCancellationContext = new CancellationTokenSourceContext();
            var startedCancellationLinkedContext = new CancellationLinkedTokenSourceContext(_cancellationTokenSourceContext, _linkedCancellationTokenSourceContext);

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

                        if (startedCancellationLinkedContext.IsCancellationRequested)
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
            }, AsyncEventsThreadPool, startedCancellationLinkedContext);
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
                if (!GameComponentGuard.Check("633DD96E-52F0-4737-88A8-405EC03C1C7B", GetType().Name, ref _state, ThreadsComponent.WaitEvent, ComponentState.Stopped))
                {
                    return;
                }

                if (ComponentStateHelper.IsStopped(_state))
                {
                    return;
                }

                NStop();
            }
        }

        private void NStop()
        {
            _startedCancellationContext?.Cancel();

            ThreadsComponent.Lock();

            WaitForAllGameComponentsWaiting();

            _serializedWorldContext.DateTimeProvider.Stop();

            _state = ComponentState.Stopped;
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

            _cancellationTokenSourceContext?.Cancel();
            
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

            _serializedWorldContext?.Dispose();

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

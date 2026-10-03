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

using NLog;
using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Common.Cancellation;
using SymOntoClay.Common.Disposing;
using SymOntoClay.Common.SerializationToImage;
using SymOntoClay.Common.SerializationToImage.Attributes;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.SerializationToImage;
using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
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
    //[WorldRootAttribute]
    //[SerializeOnlyExplicitlySerializableMembersAttribute]
    public class WorldContext: IWorldCoreContext, IWorldCoreGameComponentContext, ISymOntoClayDisposable, IPostDeserializationHandler
    {
        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="DeserializationCtor"]/*' />
        private WorldContext()
        {
        }

        public WorldContext(WorldSettings settings)
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

            CreateWorldSerializedContext();

            _isInitialized = true;
        }

        /// <inheritdoc/>
        void IPostDeserializationHandler.Handle()
        {
            if(!Directory.Exists(_tmpDir))
            {
                Directory.CreateDirectory(_tmpDir);
            }

            _state = ComponentState.Loaded;
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

            AsyncEventsThreadPool = new CustomThreadPool(threadingSettings, _linkedCancellationTokenSourceContext);

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

        //[SettingsMemberAttribute]
        private WorldSettings _settings;

        WorldSettings IWorldCoreContext.WorldSettings => _settings;

        private string _tmpDir;
        public string TmpDir => _tmpDir;

        [MemberWithExternalValue]
        public IMonitor Monitor { get; private set; }
        public IMonitorNode MonitorNode { get; private set; }
        public IMonitorLogger Logger { get; private set; }

        /// <inheritdoc/>
        IMonitor IWorldCoreGameComponentContext.Motitor => Monitor;

        //[SerializedMember]
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

        //[SerializedMemberAttributeWithChildrenAttribute]
        private readonly List<IGameComponent> _gameComponentsList = new List<IGameComponent>();

        //[SerializedMemberAttributeWithChildrenAttribute]
        private readonly List<int> _availableInstanceIdList = new List<int>();

        //[SerializedMemberAttributeWithChildrenAttribute]
        private readonly Dictionary<int, IGameComponent> _gameComponentsDictByInstanceId = new Dictionary<int, IGameComponent>();

        //[SerializedMemberAttributeWithChildrenAttribute]
        private readonly Dictionary<string, int> _instancesIdDict = new Dictionary<string, int>();

        //[SerializedMemberAttributeWithChildrenAttribute]
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

        public void Start()
        {
            lock (_stateLockObj)
            {
#if DEBUG
                //Info("CB60D9E5-32C3-474D-841C-605002B7B8D1", "Begin");
                //Info("4F8E2DE7-8BCB-4882-A961-BBB80B4BBCB9", $"_state = {_state}");
#endif

                if (!GameComponentGuard.Check("A7EBF771-7201-43AB-8034-725E5CF81C82", GetType().Name, ref _state, ThreadsComponent.WaitEvent, ComponentState.Started))
                {
#if DEBUG
                    //Info("7B2E2523-9634-46AF-817A-26A23E382029", "return;");
#endif

                    return;
                }

#if DEBUG
                //Info("0EA9E2AA-AFE5-4DEB-967D-AB81609DDBDB", $"!ComponentStateHelper.IsLoaded(_state) = {!ComponentStateHelper.IsLoaded(_state)}");
#endif

                if (!ComponentStateHelper.IsLoaded(_state))
                {
                    NLoadFromSourceCode();
                    Thread.Sleep(100);
                }

#if DEBUG
                //Info("2B38978E-E03B-46DB-8D39-CAEAF13A3B0C", "After NLoadFromSourceCode()");
#endif

                NStart();
            }
        }

        private void NStart()
        {
#if DEBUG
            //Info("A68349FF-6986-498C-AF82-431FDD4D4E7F", "Begin");
#endif

            ThreadsComponent.Lock();

#if DEBUG
            //Info("97641D0B-D85C-4FF7-9F5A-D9A7C9209A2A", "After ThreadsComponent.Lock()");
#endif

            _serializedWorldContext.DateTimeProvider.Start();

#if DEBUG
            //Info("3DC94F41-FC30-4F3D-BA1C-59097921E649", "After _serializedWorldContext.DateTimeProvider.Start()");
#endif

            lock (_gameComponentsListLockObj)
            {
#if DEBUG
                //Info("568CAB3D-8B72-4A03-9AAF-9F96614A6CBF", $"_gameComponentsList.Count = {_gameComponentsList.Count}");
#endif

                foreach (var item in _gameComponentsList)
                {
                    item.BeginStarting();
                }
            }

#if DEBUG
            //Info("26D6B738-A004-473A-8DC7-E761203FCE3B", "After item.BeginStarting()");
#endif

            WaitForAllGameComponentsWaiting();

#if DEBUG
            //Info("B893C80F-83CD-4E12-8C5C-583EFD40B374", "After WaitForAllGameComponentsWaiting()");
#endif

            ThreadsComponent.UnLock();

#if DEBUG
            //Info("2496B6D1-DB86-4411-B54C-ABFB5556C859", "After ThreadsComponent.UnLock()");
#endif

            _state = ComponentState.Started;

#if DEBUG
            //Info("CE02ACA6-91C9-4A7A-9DF0-040B2DF33F1E", $"_state = {_state}");
#endif

            lock (_gameComponentsListLockObj)
            {
                foreach (var item in _gameComponentsList)
                {
                    item.EndStarting();
                }
            }

#if DEBUG
            //Info("B4D58F09-B33D-4A95-8861-C3329AC2661A", "After item.EndStarting()");
#endif

            StartGameComponentsForLateInitializing();

#if DEBUG
            //Info("AB2083A6-C0F2-491B-B34E-2D702104DA7C", "End");
#endif
        }

        private void StartGameComponentsForLateInitializing()
        {
            _startedCancellationContext = new CancellationTokenSourceContext();
            var startedCancellationLinkedContext = new CancellationLinkedTokenSourceContext(_cancellationTokenSourceContext, _linkedCancellationTokenSourceContext);

            LoggedFunctorWithoutResult.Run(Logger, "5B3A8DB7-F7FF-469A-A3BB-D3DF197D2358",
            (IMonitorLogger loggerValue) => { },
            AsyncEventsThreadPool, startedCancellationLinkedContext);

            ThreadTask.Run(() => {//Must be refactored for serialization
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

            NPause();

            _serializedWorldContext.DateTimeProvider.Stop();

            _state = ComponentState.Stopped;
        }

        public void NPause()
        {
            ThreadsComponent.Lock();

            WaitForAllGameComponentsWaiting();
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


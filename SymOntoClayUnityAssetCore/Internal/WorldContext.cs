/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using NLog.Fluent;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.DateAndTime;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.Images;
using SymOntoClay.UnityAsset.Core.Internal.Logging;
using SymOntoClay.UnityAsset.Core.Internal.LogicQueryParsingAndCache;
using SymOntoClay.UnityAsset.Core.Internal.ModulesStorage;
using SymOntoClay.UnityAsset.Core.Internal.Storage;
using SymOntoClay.UnityAsset.Core.Internal.Threads;
using SymOntoClay.UnityAsset.Core.Internal.TypesConvertors;
using SymOntoClay.UnityAsset.Core.Internal.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public class WorldContext: IWorldCoreContext, IWorldCoreGameComponentContext
    {
        //TODO: fix me!
        public void SetSettings(WorldSettings settings)
        {
            WorldSettingsValidator.Validate(settings);

            ImplementGeneralSettings(settings);
            CreateLogging(settings);
            CreateComponents(settings);

            if(settings.EnableAutoloadingConvertors)
            {
                LoadTypesPlatformTypesConvertors();
            }

            //throw new NotImplementedException();

            _isInitialized = true;
        }

        private void ImplementGeneralSettings(WorldSettings settings)
        {
            _tmpDir = settings.TmpDir;

            Directory.CreateDirectory(_tmpDir);

            InvokerInMainThread = settings.InvokerInMainThread;
        }

        private void CreateLogging(WorldSettings settings)
        {
            CoreLogger = new CoreLogger(settings.Logging, this);
            Logger = CoreLogger.WordCoreLogger;
        }

        private void CreateComponents(WorldSettings settings)
        {
            ImagesRegistry = new ImagesRegistry(this);
            ThreadsComponent = new ThreadsCoreComponent(this);
            
            ModulesStorage = new ModulesStorageComponent(this);
            StandaloneStorage = new StandaloneStorageComponent(settings, this);
            PlatformTypesConvertorsRegistry = new PlatformTypesConvertorsRegistry(Logger);
            DateTimeProvider = new DateTimeProvider(Logger, ThreadsComponent);
            LogicQueryParseAndCache = new LogicQueryParseAndCache(settings, this);
        }

        private void LoadTypesPlatformTypesConvertors()
        {
            var targetAttributeType = typeof(PlatformTypesConvertorAttribute);

            var typesList = AppDomainTypesEnumerator.GetTypes().Where(p => p.GetCustomAttributesData().Any(x => x.AttributeType == targetAttributeType)).ToList();

            foreach (var type in typesList)
            {
                var convertor = (IPlatformTypesConvertor)Activator.CreateInstance(type);

                PlatformTypesConvertorsRegistry.AddConvertor(convertor);
            }
        }

        public void RunInMainThread(Action function)
        {
            var invocableInMainThreadObj = new InvocableInMainThread(function, InvokerInMainThread);
            invocableInMainThreadObj.Run();
        }

        public TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            var invocableInMainThreadObj = new InvocableInMainThreadObj<TResult>(function, InvokerInMainThread);
            return invocableInMainThreadObj.Run();
        }

        public void AddConvertor(IPlatformTypesConvertor convertor)
        {
            PlatformTypesConvertorsRegistry.AddConvertor(convertor);
        }

        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        private string _tmpDir;
        public string TmpDir => _tmpDir;

        public CoreLogger CoreLogger { get; private set; }
        public IEntityLogger Logger { get; private set; }

        IEntityLogger IWorldCoreGameComponentContext.CreateLogger(string name)
        {
            return CoreLogger.CreateLogger(name);
        }

        public ImagesRegistry ImagesRegistry { get; private set; }
        public ThreadsCoreComponent ThreadsComponent { get; private set; }

        IActivePeriodicObjectCommonContext IWorldCoreGameComponentContext.SyncContext => ThreadsComponent;

        public ModulesStorageComponent ModulesStorage { get; private set; }

        IModulesStorage IWorldCoreGameComponentContext.ModulesStorage => ModulesStorage.ModulesStorage;
        IModulesStorage IWorldCoreContext.ModulesStorage => ModulesStorage.ModulesStorage;

        public StandaloneStorageComponent StandaloneStorage { get; private set; }

        IStandaloneStorage IWorldCoreGameComponentContext.StandaloneStorage => StandaloneStorage.StandaloneStorage;

        public PlatformTypesConvertorsRegistry PlatformTypesConvertorsRegistry { get; private set; }
        IPlatformTypesConvertorsRegistry IWorldCoreContext.PlatformTypesConvertors => PlatformTypesConvertorsRegistry;
        IPlatformTypesConvertorsRegistry IWorldCoreGameComponentContext.PlatformTypesConvertors => PlatformTypesConvertorsRegistry;

        /// <inheritdoc/>
        public IInvokerInMainThread InvokerInMainThread { get; private set; }

        public DateTimeProvider DateTimeProvider { get; private set; }
        IDateTimeProvider IWorldCoreGameComponentContext.DateTimeProvider => DateTimeProvider;
        IDateTimeProvider IWorldCoreContext.DateTimeProvider => DateTimeProvider;

        public LogicQueryParseAndCache LogicQueryParseAndCache { get; private set; }
        ILogicQueryParseAndCache IWorldCoreGameComponentContext.LogicQueryParseAndCache => LogicQueryParseAndCache;
        ILogicQueryParseAndCache IWorldCoreContext.LogicQueryParseAndCache => LogicQueryParseAndCache;

        private readonly object _worldComponentsListLockObj = new object();
        private readonly List<IWorldCoreComponent> _worldComponentsList = new List<IWorldCoreComponent>();

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
            }
        }

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
                }
            }
        }

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

        IStorage IWorldCoreGameComponentContext.GetPublicFactsStorageByInstanceId(int instanceId)
        {
            lock (_gameComponentsListLockObj)
            {
                return _gameComponentsDictByInstanceId[instanceId].PublicFactsStorage;
            }
        }

        string IWorldCoreGameComponentContext.GetIdForFactsByInstanceId(int instanceId)
        {
            lock (_gameComponentsListLockObj)
            {
                return _gameComponentsDictByInstanceId[instanceId].IdForFacts;
            }
        }

        public bool EnableLogging { get => CoreLogger.EnableLogging; set => CoreLogger.EnableLogging = value; }

        public bool EnableRemoteConnection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Load(IRunTimeImageInfo imageInfo)
        {
            lock (_stateLockObj)
            {
                if(_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                throw new NotImplementedException();
            }
        }

        public void Load(string id)
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                throw new NotImplementedException();
            }
        }

        public void Load()
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                throw new NotImplementedException();
            }
        }

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

                throw new NotImplementedException();
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

        public IRunTimeImageInfo CreateImage(RunTimeImageSettings settings)
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                throw new NotImplementedException();
            }
        }

        public IRunTimeImageInfo CreateImage()
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                throw new NotImplementedException();
            }
        }

        public IRunTimeImageInfo CurrentImage 
        {   
            get
            {
                lock (_stateLockObj)
                {
                    if (_state == ComponentState.Disposed)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    throw new NotImplementedException();
                }
            } 
        }

        public IList<IRunTimeImageInfo> GetImages()
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                throw new NotImplementedException();
            }
        }

        public void DeleteImage(IRunTimeImageInfo imageInfo)
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                throw new NotImplementedException();
            }
        }

        public void DeleteImage(string id)
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                throw new NotImplementedException();
            }             
        }

        private ComponentState _state = ComponentState.Created;
        private readonly object _stateLockObj = new object();

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

            CoreLogger.Dispose();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        private void Log(string message)
        {
            Logger.Log(message);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        private void Warning(string message)
        {
            Logger.Warning(message);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        private void Error(string message)
        {
            Logger.Error(message);
        }
    }
}

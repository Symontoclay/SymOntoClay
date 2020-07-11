using NLog.Fluent;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.Images;
using SymOntoClay.UnityAsset.Core.Internal.Logging;
using SymOntoClay.UnityAsset.Core.Internal.ModulesStorage;
using SymOntoClay.UnityAsset.Core.Internal.SharedDictionary;
using SymOntoClay.UnityAsset.Core.Internal.Storage;
using SymOntoClay.UnityAsset.Core.Internal.Threads;
using SymOntoClay.UnityAsset.Core.Internal.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            //throw new NotImplementedException();
        }

        private void ImplementGeneralSettings(WorldSettings settings)
        {
            _tmpDir = settings.TmpDir;

            Directory.CreateDirectory(_tmpDir);
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
            SharedDictionary = new SharedDictionaryComponent(this);
            ModulesStorage = new ModulesStorageComponent(this);
            StandaloneStorage = new StandaloneStorageComponent(settings, this);
        }

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

        public SharedDictionaryComponent SharedDictionary { get; private set; }

        SymOntoClay.Core.IEntityDictionary IWorldCoreGameComponentContext.SharedDictionary => SharedDictionary.Dictionary;
        SymOntoClay.Core.IEntityDictionary IWorldCoreContext.SharedDictionary => SharedDictionary.Dictionary;

        public ModulesStorageComponent ModulesStorage { get; private set; }

        IModulesStorage IWorldCoreGameComponentContext.ModulesStorage => ModulesStorage.ModulesStorage;
        IModulesStorage IWorldCoreContext.ModulesStorage => ModulesStorage.ModulesStorage;

        public StandaloneStorageComponent StandaloneStorage { get; private set; }

        IStandaloneStorage IWorldCoreGameComponentContext.StandaloneStorage => StandaloneStorage.StandaloneStorage;

        private readonly object _worldComponentsListLockObj = new object();
        private List<IWorldCoreComponent> _worldComponentsList = new List<IWorldCoreComponent>();

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
        private List<IGameComponent> _gameComponentsList = new List<IGameComponent>();

        void IWorldCoreGameComponentContext.AddGameComponent(IGameComponent component)
        {
            lock(_gameComponentsListLockObj)
            {
                if (_gameComponentsList.Contains(component))
                {
                    return;
                }

                _gameComponentsList.Add(component);
            }
        }

        void IWorldCoreGameComponentContext.RemoveGameComponent(IGameComponent component)
        {
            lock (_gameComponentsListLockObj)
            {
                if (_gameComponentsList.Contains(component))
                {
                    _gameComponentsList.Remove(component);
                }
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
            SharedDictionary.LoadFromSourceCode();
            ModulesStorage.LoadFromSourceCode();
            StandaloneStorage.LoadFromSourceCode();

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

                if(_state != ComponentState.Loaded)
                {
                    NLoadFromSourceCode();
                }

                NStart();
            }
        }

        private void NStart()
        {
            ThreadsComponent.Lock();

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

            foreach(var item in _gameComponentsList)
            {
                item.Dispose();
            }

            foreach(var item in _worldComponentsList)
            {
                item.Dispose();
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

using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using SymOntoClay.Serialization.Implementation;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.World;
using System;
using System.Collections.Generic;
using System.IO;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.World
{
    public class WorldImlementation: IWorld
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        #region constructors
        public WorldImlementation()
        {
            _worldCore = new WorldCore();
        }
        #endregion

        #region public members
        /// <inheritdoc/>
        public string Id => string.Empty;

        /// <inheritdoc/>
        public string IdForFacts => string.Empty;

        /// <inheritdoc/>
        public int InstanceId => 0;

        /// <inheritdoc/>
        public WorldContext WorldContext => _worldCore.WorldContext;

        /// <inheritdoc/>
        public IMonitor Monitor => _worldCore.Monitor;

        /// <inheritdoc/>
        public void SetSettings(WorldSettings settings) => _worldCore.SetSettings(settings);

        /// <inheritdoc/>
        public void AddConvertor(IPlatformTypesConverter convertor) => _worldCore.AddConvertor(convertor);

        /// <inheritdoc/>
        public bool EnableLogging { get => _worldCore.EnableLogging; set => _worldCore.EnableLogging = value; }

        /// <inheritdoc/>
        public bool EnableRemoteConnection { get => _worldCore.EnableRemoteConnection; set => _worldCore.EnableRemoteConnection = value; }

        /// <inheritdoc/>
        public IMonitorLogger Logger => _worldCore.Logger;

        /// <inheritdoc/>
        public void RunInMainThread(Action function)
        {
            _worldCore.RunInMainThread(function);
        }

        /// <inheritdoc/>
        public TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            return _worldCore.RunInMainThread(function);
        }

        /// <inheritdoc/>
        public IHumanoidNPC GetHumanoidNPC(HumanoidNPCSettings settings) => _worldCore.GetHumanoidNPC(settings);

        /// <inheritdoc/>
        public IPlayer GetPlayer(PlayerSettings settings) => _worldCore.GetPlayer(settings);

        /// <inheritdoc/>
        public IGameObject GetGameObject(GameObjectSettings settings) => _worldCore.GetGameObject(settings);

        /// <inheritdoc/>
        public IPlace GetPlace(PlaceSettings settings) => _worldCore.GetPlace(settings);

        /// <inheritdoc/>
        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text) => _worldCore.InsertPublicFact(logger, text);

        /// <inheritdoc/>
        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact) => _worldCore.InsertPublicFact(logger, fact);

        /// <inheritdoc/>
        public ISyncMethodResponse RemovePublicFact(IMonitorLogger logger, string id) => _worldCore.RemovePublicFact(logger, id);

        /// <inheritdoc/>
        public ISyncMethodResponse PushSoundFact(float power, string text) => _worldCore.PushSoundFact(power, text);

        /// <inheritdoc/>
        public ISyncMethodResponse PushSoundFact(float power, RuleInstance fact) => _worldCore.PushSoundFact(power, fact);

        /// <inheritdoc/>
        public IStandardFactsBuilder StandardFactsBuilder => _worldCore.StandardFactsBuilder;

        /// <inheritdoc/>
        public ISyncMethodResponse AddCategory(IMonitorLogger logger, string category) => _worldCore.AddCategory(logger, category);

        /// <inheritdoc/>
        public ISyncMethodResponse AddCategories(IMonitorLogger logger, List<string> categories) => _worldCore.AddCategories(logger, categories);

        /// <inheritdoc/>
        public ISyncMethodResponse RemoveCategory(IMonitorLogger logger, string category) => _worldCore.RemoveCategory(logger, category);

        public ISyncMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories) => _worldCore.RemoveCategories(logger, categories);

        /// <inheritdoc/>
        public bool EnableCategories { get => _worldCore.EnableCategories; set => _worldCore.EnableCategories = value; }

        /// <inheritdoc/>
        public void Save(SerializationSettings settings)
        {
#if DEBUG
            _globalLogger.Info($"settings = {settings}");
#endif

            if(_worldCore.IsActive)
            {
                _worldCore.Stop();
            }

#if DEBUG
            if(!Directory.Exists(settings.Path))
            {
                Directory.CreateDirectory(settings.Path);
            }

            var targetPath = Path.Combine(settings.Path, settings.ImageName);

            _globalLogger.Info($"targetPath = {targetPath}");

            if(Directory.Exists(targetPath))
            {
                Directory.Delete(targetPath, true);
            }

            Directory.CreateDirectory(targetPath);

            var serializationContext = new SerializationContext(targetPath);

            var serializer = new Serializer(serializationContext);

            serializer.Serialize(_worldCore);
#endif

            throw new NotImplementedException("F99E1765-298B-42A8-B0E9-E38D41D5767C");
        }

        /// <inheritdoc/>
        public void Start() => _worldCore.Start();

        /// <inheritdoc/>
        public void Stop() => _worldCore.Stop();

        /// <inheritdoc/>
        public bool IsActive => _worldCore.IsActive;

        /// <inheritdoc/>
        public void Dispose() => _worldCore.Dispose();

        /// <inheritdoc/>
        public bool IsDisposed => _worldCore.IsDisposed;
        #endregion

        #region private members
        private WorldCore _worldCore;
        #endregion
    }
}

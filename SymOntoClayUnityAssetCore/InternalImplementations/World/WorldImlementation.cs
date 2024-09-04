using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.World;
using System;
using System.Collections.Generic;

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
        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, string text)
        {
            throw new NotSupportedException("5DDFB479-76EA-45B8-815C-08C8040836EF");
        }

        /// <inheritdoc/>
        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text) => _worldCore.InsertPublicFact(logger, text);

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            throw new NotSupportedException("E3C92A4E-1DB1-44A7-8DE3-AD31AEA68F6B");
        }

        /// <inheritdoc/>
        public IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact) => _worldCore.InsertPublicFact(logger, fact);

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemovePublicFact(IMonitorLogger logger, string id)
        {
            throw new NotSupportedException("359EF9E0-F336-425F-8DAF-EAFB4A59BBD0");
        }

        /// <inheritdoc/>
        public IMethodResponse RemovePublicFact(IMonitorLogger logger, string id) => _worldCore.RemovePublicFact(logger, id);

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldPushSoundFact(float power, string text)
        {
            throw new NotSupportedException("68A0CCDA-6C04-49B4-B780-1D17182601EE");
        }

        /// <inheritdoc/>
        public IMethodResponse PushSoundFact(float power, string text) => _worldCore.PushSoundFact(power, text);

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldPushSoundFact(float power, RuleInstance fact)
        {
            throw new NotSupportedException("9E09C25C-1C95-4213-8A9B-698FC0FD746A");
        }

        /// <inheritdoc/>
        public IMethodResponse PushSoundFact(float power, RuleInstance fact) => _worldCore.PushSoundFact(power, fact);

        /// <inheritdoc/>
        public IStandardFactsBuilder StandardFactsBuilder => _worldCore.StandardFactsBuilder;

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategory(IMonitorLogger logger, string category)
        {
            throw new NotSupportedException("87F248C0-F45E-4E02-ACF0-9DE011A48067");
        }

        /// <inheritdoc/>
        public IMethodResponse AddCategory(IMonitorLogger logger, string category) => _worldCore.AddCategory(logger, category);

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategories(IMonitorLogger logger, List<string> categories)
        {
            throw new NotSupportedException("EFC13128-9880-4B10-8277-CAAC877AFE8E");
        }

        /// <inheritdoc/>
        public IMethodResponse AddCategories(IMonitorLogger logger, List<string> categories) => _worldCore.AddCategories(logger, categories);

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategory(IMonitorLogger logger, string category)
        {
            throw new NotSupportedException("4CA2D803-AAA3-46E1-9765-A6969BCD86AB");
        }

        /// <inheritdoc/>
        public IMethodResponse RemoveCategory(IMonitorLogger logger, string category) => _worldCore.RemoveCategory(logger, category);

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            throw new NotSupportedException("6352356D-3F0C-4B7A-A480-67D463AB93E4");
        }

        public IMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories) => _worldCore.RemoveCategories(logger, categories);

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

using NLog;
using SymOntoClay.Common.Disposing;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.ConditionalEntityHostSupport;
using SymOntoClay.UnityAsset.Core.Internal.SoundPerception;
using SymOntoClay.UnityAsset.Core.Internal.Vision;
using System;
using System.Runtime;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC
{
    public class HumanoidNPCGameComponentSerializedContext : Disposable
    {
        public HumanoidNPCGameComponentSerializedContext(HumanoidNPCGameComponent gameComponent, HumanoidNPCSettings settings, HumanoidNPCGameComponentContext internalContext, IWorldCoreGameComponentContext worldContext)
        {
            if (settings.VisionProvider != null)
            {
                VisionComponent = new VisionComponent(gameComponent.Logger, settings.VisionProvider, internalContext, this, worldContext);
            }

            ConditionalEntityHostSupportComponent = new ConditionalEntityHostSupportComponent(gameComponent.Logger, settings, VisionComponent, internalContext.HostSupportComponent, worldContext);

            SoundReceiverComponent = new SoundReceiverComponent(gameComponent.Logger, settings.InstanceId, internalContext, this, worldContext);

            BackpackStorage = new ConsolidatedPublicFactsStorage(gameComponent.Logger, KindOfStorage.BackpackStorage);

            var coreEngineSettings = new EngineSettings();
            coreEngineSettings.Id = settings.Id;
            coreEngineSettings.AppFile = settings.LogicFile;
            coreEngineSettings.MonitorNode = gameComponent.MonitorNode;
            coreEngineSettings.SyncContext = worldContext.SyncContext;
            coreEngineSettings.StandardFactsBuilder = worldContext.StandardFactsBuilder;

            coreEngineSettings.ModulesStorage = worldContext.ModulesStorage;
            coreEngineSettings.ParentStorage = worldContext.StandaloneStorage;
            coreEngineSettings.LogicQueryParseAndCache = worldContext.LogicQueryParseAndCache;
            coreEngineSettings.TmpDir = internalContext.TmpDir;
            coreEngineSettings.HostListener = gameComponent;
            coreEngineSettings.DateTimeProvider = worldContext.DateTimeProvider;
            coreEngineSettings.HostSupport = internalContext.HostSupportComponent;
            coreEngineSettings.ConditionalEntityHostSupport = ConditionalEntityHostSupportComponent;
            coreEngineSettings.SoundPublisherProvider = internalContext.SoundPublisherComponent;
            coreEngineSettings.NLPConverterFactory = worldContext.NLPConverterFactory;

            coreEngineSettings.Categories = settings.Categories;
            coreEngineSettings.EnableCategories = settings.EnableCategories;

            coreEngineSettings.CancellationContext = worldContext.GetCancellationContext();
            coreEngineSettings.ThreadingSettings = settings?.ThreadingSettings ?? worldContext.HumanoidNpcDefaultThreadingSettings;

            coreEngineSettings.HtnExecutionSettings = settings?.HtnExecutionSettings ?? worldContext.HtnExecutionSettings;

            CoreEngine = new Engine(coreEngineSettings);
        }

        public VisionComponent VisionComponent { get; set; }
        public Engine CoreEngine { get; set; }
        public ConditionalEntityHostSupportComponent ConditionalEntityHostSupportComponent { get; set; }
        public SoundReceiverComponent SoundReceiverComponent { get; set; }
        public ConsolidatedPublicFactsStorage BackpackStorage { get; set; }

        public void LoadFromSourceCode()
        {
            VisionComponent?.LoadFromSourceCode();
            SoundReceiverComponent.LoadFromSourceCode();
            CoreEngine.LoadFromSourceCode();
        }

        public void BeginStarting()
        {
            CoreEngine.BeginStarting();
            VisionComponent?.BeginStarting();
        }

        public void EndStarting()
        {
            CoreEngine.EndStarting();
        }

        public void Die()
        {
            CoreEngine.Die();
            VisionComponent?.Die();
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            CoreEngine.Dispose();
            VisionComponent?.Dispose();
            SoundReceiverComponent.Dispose();

            base.OnDisposing();
        }
    }
}

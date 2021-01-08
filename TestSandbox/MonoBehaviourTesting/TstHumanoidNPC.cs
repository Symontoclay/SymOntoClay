using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TestSandbox.CoreHostListener;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.MonoBehaviourTesting
{
    public class TstHumanoidNPC: TstMonoBehaviour
    {
        private readonly IEntityLogger _logger = new LoggerImpementation();

        private IHumanoidNPC _npc;

        public override void Awake()
        {
            _logger.Log("Begin");

            var platformListener = new TstPlatformHostListener();

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            //npcSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Hosts\PeaceKeeper\PeaceKeeper.host");
            npcSettings.LogicFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Npcs\PeaceKeeper\PeaceKeeper.npc");
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new TstPlatformSupport();

            _logger.Log($"npcSettings = {npcSettings}");

            _npc = WorldFactory.WorldInstance.GetHumanoidNPC(npcSettings);

            _logger.Log($"_npc == null = {_npc == null}");

            _logger.Log("End");
        }

        public override void Start()
        {
            _logger.Log("Begin");



            _logger.Log("End");
        }

        public override void Stop()
        {
            _logger.Log("Begin");

            _npc.Dispose();

            _logger.Log("End");
        }
    }
}

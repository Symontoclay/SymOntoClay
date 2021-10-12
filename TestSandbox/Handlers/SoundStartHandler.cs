using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClayDefaultCLIEnvironment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSandbox.CoreHostListener;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class SoundStartHandler : BaseGeneralStartHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var platformListener = new TstPlatformHostListener();

            CreateNPC(platformListener);

            var platformListener2 = new TstPlatformHostListener();

            var settings = new GameObjectSettings();

            settings.Id = "#a";
            settings.InstanceId = 2;
            settings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Npcs\Barrel\Barrel.sobj");
            settings.HostListener = platformListener2;
            settings.PlatformSupport = new PlatformSupportCLIStub(new Vector3(100, 100, 100));

            var gameObject = WorldFactory.WorldInstance.GetGameObject(settings);

            _world.Start();

            Thread.Sleep(5000);

            gameObject.PushSoundFact(60, "act(M16, shoot)");

            Thread.Sleep(5000);

            _world.Dispose();

            _logger.Log("!---");

            Thread.Sleep(5000);

            _logger.Log("End");
        }
    }
}

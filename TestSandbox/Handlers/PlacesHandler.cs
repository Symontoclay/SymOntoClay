using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Tests.HostListeners;
using SymOntoClayBaseTestLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class PlacesHandler : BaseGeneralStartHandler
    {
        public void Run()
        {
            _logger.Log("Begin");

            var platformListener = new FullGeneralized_Tests_HostListener();

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.HostListener = platformListener;

            CreateMainNPC(factorySettings);

            _world.Start();

            Thread.Sleep(50000);

            _logger.Log("End");
        }
    }
}

/*
 
 
 */

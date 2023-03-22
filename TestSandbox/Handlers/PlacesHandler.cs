using SymOntoClay.BaseTestLib;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.BaseTestLib.HostListeners;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSandbox.CoreHostListener;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class PlacesHandler : BaseGeneralStartHandler
    {
        public void Run()
        {
            _logger.Log("Begin");

            //var platformListener = new TstGoHostListener();
            var platformListener = new Exec_Tests_HostListener4();
            //var platformListener = new FullGeneralized_Tests_HostListener();

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.HostListener = platformListener;

            CreateMainNPC(factorySettings);

            var settings = new PlaceSettings();
            settings.Id = "#WP1";
            settings.InstanceId = 123;
            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

            settings.PlatformSupport = new PlatformSupportCLIStub();

            //settings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Navs\waypoint\waypoint.sobj");

            //_logger.Log($"settings.HostFile = {settings.HostFile}");

            settings.Categories = new List<string>() { "waypoint" };
            settings.EnableCategories = true;

            var place = _world.GetPlace(settings);

            _world.Start();

            Thread.Sleep(50000);

            _logger.Log("End");
        }
    }
}

/*
 
 var settings = new PlaceSettings();
            settings.Id = Id;
            settings.InstanceId = gameObject.GetInstanceID();
            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = new System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z);

            if(SobjFile != null)
            {
                var fullFileName = Path.Combine(Application.dataPath, SobjFile.FullName);
                settings.HostFile = fullFileName;
            }
            
            settings.PlatformSupport = this;

#if DEBUG
            //Debug.Log($"BaseElementaryArea Awake ('{name}') GetType().FullName  = {GetType().FullName}");
            Debug.Log($"BaseElementaryArea Awake ('{name}') gameObject.GetInstanceID() = {gameObject.GetInstanceID()}");
            Debug.Log($"BaseElementaryArea Awake ('{name}') Id = {Id}");
            if (GetType() == typeof(Waypoint))
            {
                settings.Categories = new List<string>() { "waypoint" };
                settings.EnableCategories = true;
            }
#endif

            _place = WorldFactory.WorldInstance.GetPlace(settings);
 */

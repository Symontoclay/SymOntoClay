/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

            var platformListener = new TstBattleRoyaleHostListener();

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.HostListener = platformListener;

            CreateMainNPC(factorySettings);

            var settings = new PlaceSettings();
            settings.Id = "#WP1";
            settings.InstanceId = 123;
            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

            settings.PlatformSupport = new PlatformSupportCLIStub();



            settings.Categories = new List<string>() { "waypoint" };
            settings.EnableCategories = true;

            var place = _world.GetPlace(settings);



            ////settings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Navs\waypoint\waypoint.sobj");

            ////_logger.Log($"settings.HostFile = {settings.HostFile}");



            _world.Start();

            Thread.Sleep(100000);

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

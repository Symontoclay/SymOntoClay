/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.BaseTestLib.HostListeners;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.UnityAsset.Core;

namespace SymOntoClayPlacesHardTests
{
    public class Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            using (var instance = new AdvancedBehaviorTestEngineInstance())
            {
                instance.CreateWorld((n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 3:
                            Assert.AreEqual("ByEntity", message);
                            break;

                        case 4:
                            Assert.AreEqual("123", message);
                            break;

                        case 5:
                            Assert.AreEqual("#`WP1`", message);
                            break;

                        case 6:
                            Assert.AreEqual("<10, 10, 10>", message);
                            break;

                        case 7:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        case 8:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, true);

                instance.WriteFile(@"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.go(to: #@(waypoint));
        'End' >> @>log;
    }
}");

                var hostListener = new Exec_Tests_HostListener4();

                instance.CreateNPC(hostListener);

                var settings = new PlaceSettings();
                settings.Id = "#WP1";
                settings.InstanceId = 123;
                settings.AllowPublicPosition = true;
                settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

                settings.PlatformSupport = new PlatformSupportCLIStub();

                settings.Categories = new List<string>() { "waypoint" };
                settings.EnableCategories = true;

                var place = instance.GetPlace(settings);

                instance.StartWorld();

                Thread.Sleep(5000);
            }
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            using (var instance = new AdvancedBehaviorTestEngineInstance())
            {
                instance.CreateWorld((n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 3:
                            Assert.AreEqual("ByEntity", message);
                            break;

                        case 4:
                            Assert.AreEqual("123", message);
                            break;

                        case 5:
                            Assert.AreEqual("#`WP1`", message);
                            break;

                        case 6:
                            Assert.AreEqual("<10, 10, 10>", message);
                            break;

                        case 7:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        case 8:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, true);

                instance.WriteFile(@"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.go(to: #@(waypoint & random));
        'End' >> @>log;
    }
}");

                var hostListener = new Exec_Tests_HostListener4();

                instance.CreateNPC(hostListener);

                var settings = new PlaceSettings();
                settings.Id = "#WP1";
                settings.InstanceId = 123;
                settings.AllowPublicPosition = true;
                settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

                settings.PlatformSupport = new PlatformSupportCLIStub();

                settings.Categories = new List<string>() { "waypoint" };
                settings.EnableCategories = true;

                var place = instance.GetPlace(settings);

                instance.StartWorld();

                Thread.Sleep(5000);
            }
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            using (var instance = new AdvancedBehaviorTestEngineInstance())
            {
                instance.CreateWorld((n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 3:
                            Assert.AreEqual("ByEntity", message);
                            break;

                        case 4:
                            Assert.AreEqual("123", message);
                            break;

                        case 5:
                            Assert.AreEqual("#`WP1`", message);
                            break;

                        case 6:
                            Assert.AreEqual("<10, 10, 10>", message);
                            break;

                        case 7:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        case 8:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, true);

                instance.WriteFile(@"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.go(to: #@(waypoint & nearest));
        'End' >> @>log;
    }
}");

                var hostListener = new Exec_Tests_HostListener4();

                instance.CreateNPC(hostListener);

                var settings = new PlaceSettings();
                settings.Id = "#WP1";
                settings.InstanceId = 123;
                settings.AllowPublicPosition = true;
                settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

                settings.PlatformSupport = new PlatformSupportCLIStub();

                settings.Categories = new List<string>() { "waypoint" };
                settings.EnableCategories = true;

                var place = instance.GetPlace(settings);

                instance.StartWorld();

                Thread.Sleep(5000);
            }
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            using (var instance = new AdvancedBehaviorTestEngineInstance())
            {
                instance.CreateWorld((n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 3:
                            Assert.AreEqual("ByEntity", message);
                            break;

                        case 4:
                            Assert.AreEqual("123", message);
                            break;

                        case 5:
                            Assert.AreEqual("#`WP1`", message);
                            break;

                        case 6:
                            Assert.AreEqual("<10, 10, 10>", message);
                            break;

                        case 7:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        case 8:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, true);

                instance.WriteFile(@"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.go(to: #@(waypoint & random));
        'End' >> @>log;
    }
}");

                var hostListener = new AsyncExec_Tests_HostListener4(1000);

                instance.CreateNPC(hostListener);

                var settings = new PlaceSettings();
                settings.Id = "#WP1";
                settings.InstanceId = 123;
                settings.AllowPublicPosition = true;
                settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

                settings.PlatformSupport = new PlatformSupportCLIStub();

                settings.Categories = new List<string>() { "waypoint" };
                settings.EnableCategories = true;

                var place = instance.GetPlace(settings);

                instance.StartWorld();

                Thread.Sleep(5000);
            }
        }

        [Test]
        [Parallelizable]
        public void Case4()
        {
            using (var instance = new AdvancedBehaviorTestEngineInstance())
            {
                instance.CreateWorld((n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("Begin go", message);
                            break;

                        case 3:
                            Assert.AreEqual("Begin move and check", message);
                            break;

                        case 4:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 5:
                            Assert.AreEqual("ByEntity", message);
                            break;

                        case 6:
                            Assert.AreEqual("123", message);
                            break;

                        case 7:
                            Assert.AreEqual("#`WP1`", message);
                            break;

                        case 8:
                            Assert.AreEqual("<10, 10, 10>", message);
                            break;

                        case 9:
                            Assert.AreEqual("Begin check", message);
                            break;

                        case 10:
                            Assert.AreEqual("StopImpl Begin", message);
                            break;

                        case 11:
                            Assert.AreEqual("RotateImpl Begin", message);
                            break;

                        case 12:
                            Assert.AreEqual("30", message);
                            break;

                        case 13:
                            Assert.AreEqual("RotateImpl Begin", message);
                            break;

                        case 14:
                            Assert.AreEqual("-60", message);
                            break;

                        case 15:
                            Assert.AreEqual("RotateImpl Begin", message);
                            break;

                        case 16:
                            Assert.AreEqual("30", message);
                            break;

                        case 17:
                            Assert.AreEqual("End check", message);
                            break;

                        case 18:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 19:
                            Assert.AreEqual("ByEntity", message);
                            break;

                        case 20:
                            Assert.AreEqual("123", message);
                            break;

                        case 21:
                            Assert.AreEqual("#`WP1`", message);
                            break;

                        case 22:
                            Assert.AreEqual("<10, 10, 10>", message);
                            break;

                        case 23:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        case 24:
                            Assert.AreEqual("on complete move", message);
                            break;

                        case 25:
                            Assert.AreEqual("End go", message);
                            break;

                        case 26:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, true);

                instance.WriteFile(@"app PeaceKeeper
{	
	on Enter =>
    {
        'Begin' >> @>log;

        go();

        'End' >> @>log;
    }
	
	fun go()
    {
	    'Begin go' >> @>log;
		`move and check`(#@(waypoint & random));
		'End go' >> @>log;
    }
}

action `move and check`
{
    op (@target)
	{
	    'Begin move and check' >> @>log;

	    repeat
		{
		    @@host.go(to: @target)[: timeout = 1.2, on complete {'on complete move' >> @>log; complete action;} :];
			check();			
		}

		'End move and check' >> @>log;
	}
}

action check
{
    op ()
	{
	    'Begin check' >> @>log;
	    @@host.`stop`();
	    @@host.`rotate`(30);
		@@host.`rotate`(-60);
		@@host.`rotate`(30);
		'End check' >> @>log;
	}
}");

                var hostListener = new BattleRoyaleHostListener();

                instance.CreateNPC(hostListener);

                var settings = new PlaceSettings();
                settings.Id = "#WP1";
                settings.InstanceId = 123;
                settings.AllowPublicPosition = true;
                settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

                settings.PlatformSupport = new PlatformSupportCLIStub();

                settings.Categories = new List<string>() { "waypoint" };
                settings.EnableCategories = true;

                var place = instance.GetPlace(settings);

                instance.StartWorld();

                Thread.Sleep(5000);
            }
        }

        [Test]
        [Parallelizable]
        public void Case4_a()
        {
            using (var instance = new AdvancedBehaviorTestEngineInstance())
            {
                instance.CreateWorld((n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("Begin go", message);
                            break;

                        case 3:
                            Assert.AreEqual("Begin move and check", message);
                            break;

                        case 4:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 5:
                            Assert.AreEqual("ByEntity", message);
                            break;

                        case 6:
                            Assert.AreEqual("123", message);
                            break;

                        case 7:
                            Assert.AreEqual("#`WP1`", message);
                            break;

                        case 8:
                            Assert.AreEqual("<10, 10, 10>", message);
                            break;

                        case 9:
                            Assert.AreEqual("Begin check", message);
                            break;

                        case 10:
                            Assert.AreEqual("StopImpl Begin", message);
                            break;

                        case 11:
                            Assert.AreEqual("RotateImpl Begin", message);
                            break;

                        case 12:
                            Assert.AreEqual("30", message);
                            break;

                        case 13:
                            Assert.AreEqual("RotateImpl Begin", message);
                            break;

                        case 14:
                            Assert.AreEqual("-60", message);
                            break;

                        case 15:
                            Assert.AreEqual("RotateImpl Begin", message);
                            break;

                        case 16:
                            Assert.AreEqual("30", message);
                            break;

                        case 17:
                            Assert.AreEqual("End check", message);
                            break;

                        case 18:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 19:
                            Assert.AreEqual("ByEntity", message);
                            break;

                        case 20:
                            Assert.AreEqual("123", message);
                            break;

                        case 21:
                            Assert.AreEqual("#`WP1`", message);
                            break;

                        case 22:
                            Assert.AreEqual("<10, 10, 10>", message);
                            break;

                        case 23:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        case 24:
                            Assert.AreEqual("on completed move", message);
                            break;

                        case 25:
                            Assert.AreEqual("End go", message);
                            break;

                        case 26:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, true);

                instance.WriteFile(@"app PeaceKeeper
{	
	on Enter =>
    {
        'Begin' >> @>log;

        go();

        'End' >> @>log;
    }
	
	fun go()
    {
	    'Begin go' >> @>log;
		`move and check`(#@(waypoint & random));
		'End go' >> @>log;
    }
}

action `move and check`
{
    op (@target)
	{
	    'Begin move and check' >> @>log;

	    repeat
		{
		    @@host.go(to: @target)[: timeout = 1.2, on completed {'on completed move' >> @>log; complete action;} :];
			check();			
		}

		'End move and check' >> @>log;
	}
}

action check
{
    op ()
	{
	    'Begin check' >> @>log;
	    @@host.`stop`();
	    @@host.`rotate`(30);
		@@host.`rotate`(-60);
		@@host.`rotate`(30);
		'End check' >> @>log;
	}
}");

                var hostListener = new BattleRoyaleHostListener();

                instance.CreateNPC(hostListener);

                var settings = new PlaceSettings();
                settings.Id = "#WP1";
                settings.InstanceId = 123;
                settings.AllowPublicPosition = true;
                settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

                settings.PlatformSupport = new PlatformSupportCLIStub();

                settings.Categories = new List<string>() { "waypoint" };
                settings.EnableCategories = true;

                var place = instance.GetPlace(settings);

                instance.StartWorld();

                Thread.Sleep(5000);
            }
        }
    }
}

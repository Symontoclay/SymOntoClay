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

using NLog;
using SymOntoClay.BaseTestLib;
using SymOntoClay.BaseTestLib.HostListeners;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.StandardFacts;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Tests
{
    public class Actions_HardTests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            instance.CreateWorld((n, message) =>
            {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "Begin move and check");
                        break;

                    case 4:
                        Assert.AreEqual(message, "I see!!!");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End go");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End");
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

    on {: see(I, $_) :} => 
    {
        'I see!!!' >> @>log;
        weak cancel action;
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
            var standardFactsBuilder = new StandardFactsBuilder();

            var hostListener = new BattleRoyaleSilentHostListener();

            var npc = instance.CreateNPC(hostListener);

            var settings = new PlaceSettings();
            settings.Id = "#WP1";
            settings.InstanceId = 123;
            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

            settings.PlatformSupport = new PlatformSupportCLIStub();

            settings.Categories = new List<string>() { "waypoint" };
            settings.EnableCategories = true;

            var place = instance.GetPlace(settings);

            var enemyId = "#enemy1";

            var enemySettings = new HumanoidNPCSettings();
            enemySettings.Id = enemyId;
            enemySettings.InstanceId = 567;
            enemySettings.AllowPublicPosition = true;
            enemySettings.UseStaticPosition = new System.Numerics.Vector3(15, 15, 15);

            enemySettings.PlatformSupport = new PlatformSupportCLIStub();
            enemySettings.HostListener = new object();

            enemySettings.Categories = new List<string>() { "soldier" };
            enemySettings.EnableCategories = true;

            var enemy = instance.GetHumanoidNPC(enemySettings);

            hostListener.AddOnEndPointEnterSyncHandler("Go", () => {
                npc.InsertFact(standardFactsBuilder.BuildSeeFactString(enemyId));
            });

            instance.StartWorld();

            Thread.Sleep(2000);
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            instance.CreateWorld((n, message) =>
            {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "Begin move and check");
                        break;

                    case 4:
                        Assert.AreEqual(message, "I see!!!");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End go");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End");
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

    on {: see(I, $_) :} => 
    {
        'I see!!!' >> @>log;
        complete action;
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
            var standardFactsBuilder = new StandardFactsBuilder();

            var hostListener = new BattleRoyaleSilentHostListener();

            var npc = instance.CreateNPC(hostListener);

            var settings = new PlaceSettings();
            settings.Id = "#WP1";
            settings.InstanceId = 123;
            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

            settings.PlatformSupport = new PlatformSupportCLIStub();

            settings.Categories = new List<string>() { "waypoint" };
            settings.EnableCategories = true;

            var place = instance.GetPlace(settings);

            var enemyId = "#enemy1";

            var enemySettings = new HumanoidNPCSettings();
            enemySettings.Id = enemyId;
            enemySettings.InstanceId = 567;
            enemySettings.AllowPublicPosition = true;
            enemySettings.UseStaticPosition = new System.Numerics.Vector3(15, 15, 15);

            enemySettings.PlatformSupport = new PlatformSupportCLIStub();
            enemySettings.HostListener = new object();

            enemySettings.Categories = new List<string>() { "soldier" };
            enemySettings.EnableCategories = true;

            var enemy = instance.GetHumanoidNPC(enemySettings);

            hostListener.AddOnEndPointEnterSyncHandler("Go", () => {
                npc.InsertFact(standardFactsBuilder.BuildSeeFactString(enemyId));
            });

            instance.StartWorld();

            Thread.Sleep(2000);
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            instance.CreateWorld((n, message) =>
            {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "Begin move and check");
                        break;

                    case 4:
                        Assert.AreEqual(message, "Begin move");
                        break;

                    case 5:
                        Assert.AreEqual(message, "I see!!!");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End go");
                        break;

                    case 7:
                        Assert.AreEqual(message, "End");
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
		    move(@target)[: timeout = 1.2, on complete { 'on complete move' >> @>log; complete action;}, on weak canceled { 'on weak canceled move and check (move)' >> @>log; } :];
			check();
		}

		'End move and check' >> @>log;
	}

	on {: see(I, $_) :} => 
	{
	    'I see!!!' >> @>log;
		complete action;
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
}

action move
{		
	op (@target)
	{
	    'Begin move' >> @>log;
		@@host.go(to: @target) [: on weak canceled { 'on weak canceled move (host)' >> @>log; complete action; }, on complete { 'on complete move (host)' >> @>log; complete action;} :];
		'End move' >> @>log;
	}
}");
            var standardFactsBuilder = new StandardFactsBuilder();

            var hostListener = new BattleRoyaleSilentHostListener();

            var npc = instance.CreateNPC(hostListener);

            var settings = new PlaceSettings();
            settings.Id = "#WP1";
            settings.InstanceId = 123;
            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

            settings.PlatformSupport = new PlatformSupportCLIStub();

            settings.Categories = new List<string>() { "waypoint" };
            settings.EnableCategories = true;

            var place = instance.GetPlace(settings);

            var enemyId = "#enemy1";

            var enemySettings = new HumanoidNPCSettings();
            enemySettings.Id = enemyId;
            enemySettings.InstanceId = 567;
            enemySettings.AllowPublicPosition = true;
            enemySettings.UseStaticPosition = new System.Numerics.Vector3(15, 15, 15);

            enemySettings.PlatformSupport = new PlatformSupportCLIStub();
            enemySettings.HostListener = new object();

            enemySettings.Categories = new List<string>() { "soldier" };
            enemySettings.EnableCategories = true;

            var enemy = instance.GetHumanoidNPC(enemySettings);

            hostListener.AddOnEndPointEnterSyncHandler("Go", () => {
                npc.InsertFact(standardFactsBuilder.BuildSeeFactString(enemyId));
            });

            instance.StartWorld();

            Thread.Sleep(2000);
        }

        [Test]
        [Parallelizable]
        public void Case3_1()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            instance.CreateWorld((n, message) =>
            {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "Begin move and check");
                        break;

                    case 4:
                        Assert.AreEqual(message, "Begin move");
                        break;

                    case 5:
                        Assert.AreEqual(message, "I see!!!");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End go");
                        break;

                    case 7:
                        Assert.AreEqual(message, "End");
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
		    move(@target)[: timeout = 1.2, on complete { 'on complete move' >> @>log; complete action;}, on weak canceled { 'on weak canceled move and check (move)' >> @>log; } :];
			check();
		}

		'End move and check' >> @>log;
	}

	on {: see(I, $_) :} => 
	{
	    'I see!!!' >> @>log;
		weak cancel action;
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
}

action move
{		
	op (@target)
	{
	    'Begin move' >> @>log;
		@@host.go(to: @target) [: on weak canceled { 'on weak canceled move (host)' >> @>log; complete action; }, on complete { 'on complete move (host)' >> @>log; complete action;} :];
		'End move' >> @>log;
	}
}");

            var standardFactsBuilder = new StandardFactsBuilder();

            var hostListener = new BattleRoyaleSilentHostListener();

            var npc = instance.CreateNPC(hostListener);

            var settings = new PlaceSettings();
            settings.Id = "#WP1";
            settings.InstanceId = 123;
            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

            settings.PlatformSupport = new PlatformSupportCLIStub();

            settings.Categories = new List<string>() { "waypoint" };
            settings.EnableCategories = true;

            var place = instance.GetPlace(settings);

            var enemyId = "#enemy1";

            var enemySettings = new HumanoidNPCSettings();
            enemySettings.Id = enemyId;
            enemySettings.InstanceId = 567;
            enemySettings.AllowPublicPosition = true;
            enemySettings.UseStaticPosition = new System.Numerics.Vector3(15, 15, 15);

            enemySettings.PlatformSupport = new PlatformSupportCLIStub();
            enemySettings.HostListener = new object();

            enemySettings.Categories = new List<string>() { "soldier" };
            enemySettings.EnableCategories = true;

            var enemy = instance.GetHumanoidNPC(enemySettings);

            hostListener.AddOnEndPointEnterSyncHandler("Go", () => {
                npc.InsertFact(standardFactsBuilder.BuildSeeFactString(enemyId));
            });

            instance.StartWorld();

            Thread.Sleep(2000);
        }

        [Test]
        [Parallelizable]
        public void Case3_a()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            instance.CreateWorld((n, message) =>
            {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "Begin move and check");
                        break;

                    case 4:
                        Assert.AreEqual(message, "Begin move");
                        break;

                    case 5:
                        Assert.AreEqual(message, "I see!!!");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End go");
                        break;

                    case 7:
                        Assert.AreEqual(message, "End");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            }, true);

            instance.WriteFile(@"
{: { enemy($x) } -> { soldier($x) } :}

app PeaceKeeper
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
		    move(@target)[: timeout = 1.2, on complete { 'on complete move' >> @>log; complete action;}, on weak canceled { 'on weak canceled move and check (move)' >> @>log; } :];
			check();
		}

		'End move and check' >> @>log;
	}

	on {: see(I, $_) & enemy($_) & state($_, alive) :} => 
	{
	    'I see!!!' >> @>log;
		weak cancel action;
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
}

action move
{		
	op (@target)
	{
	    'Begin move' >> @>log;
		@@host.go(to: @target) [: on weak canceled { 'on weak canceled move (host)' >> @>log; complete action; }, on complete { 'on complete move (host)' >> @>log; complete action;} :];
		'End move' >> @>log;
	}
}");

            var standardFactsBuilder = new StandardFactsBuilder();

            var hostListener = new BattleRoyaleSilentHostListener();

            var npc = instance.CreateNPC(hostListener);

            var settings = new PlaceSettings();
            settings.Id = "#WP1";
            settings.InstanceId = 123;
            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

            settings.PlatformSupport = new PlatformSupportCLIStub();

            settings.Categories = new List<string>() { "waypoint" };
            settings.EnableCategories = true;

            var place = instance.GetPlace(settings);

            var enemyId = "#enemy1";

            var enemySettings = new HumanoidNPCSettings();
            enemySettings.Id = enemyId;
            enemySettings.InstanceId = 567;
            enemySettings.AllowPublicPosition = true;
            enemySettings.UseStaticPosition = new System.Numerics.Vector3(15, 15, 15);

            enemySettings.PlatformSupport = new PlatformSupportCLIStub();
            enemySettings.HostListener = new object();

            enemySettings.Categories = new List<string>() { "soldier" };
            enemySettings.EnableCategories = true;

            var enemy = instance.GetHumanoidNPC(enemySettings);

            hostListener.AddOnEndPointEnterSyncHandler("Go", () => {
                npc.InsertFact(standardFactsBuilder.BuildAliveFactString(enemyId));
                npc.InsertFact(standardFactsBuilder.BuildDefaultInheritanceFactString(enemyId, "soldier"));
                npc.InsertFact(standardFactsBuilder.BuildSeeFactString(enemyId));
            });

            instance.StartWorld();

            Thread.Sleep(2000);
        }
    }
}

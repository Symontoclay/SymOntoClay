﻿using NUnit.Framework;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class SoundFacts_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var world = instance.CreateWorld((n, message) => {
                switch(n)
                {
                    case 1:
                        Assert.AreEqual(message, "m4a1");
                        break;

                    case 2:
                        Assert.AreEqual(message.StartsWith("155.88"), true);
                        break;

                    case 3:
                        Assert.AreEqual(message, "!!!M4A1!!!!");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            instance.WriteFile(@"app PeaceKeeper
{
    {: gun(M4A1) :}

    on {: hear(I, $x) & gun($x) & distance(I, $x, $y) :} ($x >> @x, $y >> @y)
    {
        @x >> @>log;
        @y >> @>log;
        '!!!M4A1!!!!' >> @>log;
    }
}");

            instance.CreateNPC(world);

            var thingProjName = "M4A1";

            instance.WriteThingFile(thingProjName, @"app M4A1_app is gun
{
}");

            var gun = instance.CreateThing(world, thingProjName, new Vector3(100, 100, 100));

            world.Start();

            Thread.Sleep(100);

            gun.PushSoundFact(60, "act(M4A1, shoot)");

            Thread.Sleep(5000);
        }
    }
}

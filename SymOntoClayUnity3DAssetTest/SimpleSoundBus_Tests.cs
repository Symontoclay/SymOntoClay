using NUnit.Framework;
using SymOntoClay.Core.Tests.Helpers;
using SymOntoClay.SoundBuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class SimpleSoundBus_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var bus = new SimpleSoundBus();

            var receiver1 = new TestSoundReceiver(11, new Vector3(10, 10, 10), (double power, double distance, Vector3 position, string query, string convertedQuery) => {
                Assert.AreEqual(59.37646171569824, power);
                Assert.AreEqual(15.588457107543945, distance);
                Assert.AreEqual(new Vector3(1, 1, 1), position);
                Assert.AreEqual("{: act(M16, shoot) :}", query);
                Assert.AreEqual("{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}", convertedQuery);
            });

            bus.AddReceiver(receiver1);

            bus.PushSound(12, 60, new Vector3(1, 1, 1), "act(M16, shoot)");

            Thread.Sleep(100);
        }
    }
}

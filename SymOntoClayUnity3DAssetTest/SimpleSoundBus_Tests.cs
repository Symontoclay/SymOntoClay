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

using NUnit.Framework;
using SymOntoClay.BaseTestLib;
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

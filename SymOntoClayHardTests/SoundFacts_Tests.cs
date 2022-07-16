/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using SymOntoClay.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Tests
{
    public class SoundFacts_Tests
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

                instance.CreateNPC();

                var thingProjName = "M4A1";

                instance.WriteThingFile(thingProjName, @"app M4A1_app is gun
{
}");

                var gun = instance.CreateThing(thingProjName, new Vector3(100, 100, 100));

                instance.StartWorld();

                Thread.Sleep(100);

                gun.PushSoundFact(60, "act(M4A1, shoot)");

                Thread.Sleep(5000);
            }
        }
    }
}

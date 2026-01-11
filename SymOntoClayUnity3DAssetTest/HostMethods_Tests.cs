/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.BaseTestLib.HostListeners;
using System;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class HostMethods_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;

        @@host.`boo`();

        'End' >> @>log;
    }
}";

            var hostListener = new HostMethods_Tests_HostListener();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("BooImpl Begin", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;

        @@host.`rotate`(30);

        'End' >> @>log;
    }
}";

            var hostListener = new HostMethods_Tests_HostListener();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("RotateImpl Begin", message);
                            return true;

                        case 3:
                            Assert.AreEqual("30", message);
                            return true;

                        case 4:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));

            Assert.AreEqual(4, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case2_b()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;

        @@host.`rotate`(NULL);

        'End' >> @>log;
    }
}";

            var hostListener = new HostMethods_Tests_HostListener();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("RotateImpl Begin", message);
                            return true;

                        case 3:
                            Assert.AreEqual(string.Empty, message);
                            return true;

                        case 4:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));

            Assert.AreEqual(4, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case2_c()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;

        @@host.`rotate`(#@(gun));

        'End' >> @>log;
    }
}";

            var hostListener = new HostMethods_Tests_HostListener();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("RotateToEntityImpl Begin", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case3_a()
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
                            Assert.AreEqual("TakeImpl Begin", message);
                            break;

                        case 3:
                            Assert.AreEqual("(entity == null) = False", message);
                            break;

                        case 4:
                            {
                                Assert.AreNotEqual(string.Empty, message);

                                var intVal = int.Parse(message);

                                Assert.AreEqual(true, intVal > 0);
                            }
                            break;

                        case 5:
                            Assert.AreNotEqual(string.Empty, message);
                            break;

                        case 6:
                            Assert.AreEqual("<10, 10, 10>", message);
                            break;

                        case 7:
                            Assert.AreEqual("TakeImpl End", message);
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
    on Enter
    {
        'Begin' >> @>log;
        @@host.take(#@(gun));
        'End' >> @>log;
    }
}");

                var hostListener = new HostMethods_Tests_HostListener();

                instance.CreateNPC(hostListener);

                var thingProjName = "M4A1";

                instance.WriteThingFile(thingProjName, @"app M4A1_app is gun
{
}");

                var gun = instance.CreateThing(thingProjName);

                instance.StartWorld();

                Thread.Sleep(5000);
            }
        }

        [Test]
        [Parallelizable]
        public void Case3_b()
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
                            Assert.AreEqual("TakeImpl Begin", message);
                            break;

                        case 3:
                            Assert.AreEqual("(entity == null) = False", message);
                            break;

                        case 4:
                            Assert.AreEqual("0", message);
                            break;

                        case 5:
                            Assert.AreEqual(string.Empty, message);
                            break;

                        case 6:
                            Assert.AreEqual(string.Empty, message);
                            break;

                        case 7:
                            Assert.AreEqual("TakeImpl End", message);
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
    on Enter
    {
        'Begin' >> @>log;
        @@host.take(#@(gun));
        'End' >> @>log;
    }
}");

                var hostListener = new HostMethods_Tests_HostListener();

                instance.CreateNPC(hostListener);

                instance.StartWorld();

                Thread.Sleep(5000);
            }
        }

        [Test]
        [Parallelizable]
        public void Case3_c()
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
                            Assert.AreEqual("TakeImpl Begin", message);
                            break;

                        case 3:
                            Assert.AreEqual("(entity == null) = True", message);
                            break;

                        case 4:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, true);

                instance.WriteFile(@"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @@host.take(NULL);
        'End' >> @>log;
    }
}");

                var hostListener = new HostMethods_Tests_HostListener();

                instance.CreateNPC(hostListener);

                var thingProjName = "M4A1";

                instance.WriteThingFile(thingProjName, @"app M4A1_app is gun
{
}");

                var gun = instance.CreateThing(thingProjName);

                instance.StartWorld();

                Thread.Sleep(5000);
            }
        }

        [Test]
        [Parallelizable]
        public void Case4()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {       
        @@host.go(to: #123);
    }
}";

            var hostListener = new Exec_Tests_HostListener2();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl", message);
                            return true;

                        case 2:
                            Assert.AreEqual("<0, 0, 0>", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case4_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {       
        @@host.go(to: weapon);
    }
}";

            var hostListener = new Exec_Tests_HostListener2();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl", message);
                            return true;

                        case 2:
                            Assert.AreEqual("<0, 0, 0>", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case5()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {       
        @@host.go(to: #123);
    }
}";

            var hostListener = new Exec_Tests_HostListener3();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("0", message);
                            return true;

                        case 3:
                            Assert.AreEqual(string.Empty, message);
                            return true;

                        case 4:
                            Assert.AreEqual(string.Empty, message);
                            return true;

                        case 5:
                            Assert.AreEqual("GoToImpl End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));

            Assert.AreEqual(5, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case5_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {       
        @@host.go(to: weapon);
    }
}";

            var hostListener = new Exec_Tests_HostListener3();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("0", message);
                            return true;

                        case 3:
                            Assert.AreEqual(string.Empty, message);
                            return true;

                        case 4:
                            Assert.AreEqual(string.Empty, message);
                            return true;

                        case 5:
                            Assert.AreEqual("GoToImpl End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));

            Assert.AreEqual(5, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case6()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {       
        @@host.go(to: #123);
    }
}";

            var hostListener = new Exec_Tests_HostListener4();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("ByEntity", message);
                            return true;

                        case 3:
                            Assert.AreEqual("0", message);
                            return true;

                        case 4:
                            Assert.AreEqual(string.Empty, message);
                            return true;

                        case 5:
                            Assert.AreEqual(string.Empty, message);
                            return true;

                        case 6:
                            Assert.AreEqual("GoToImpl End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));

            Assert.AreEqual(6, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case6_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {       
        @@host.go(to: weapon);
    }
}";

            var hostListener = new Exec_Tests_HostListener4();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("ByEntity", message);
                            return true;

                        case 3:
                            Assert.AreEqual("0", message);
                            return true;

                        case 4:
                            Assert.AreEqual(string.Empty, message);
                            return true;

                        case 5:
                            Assert.AreEqual(string.Empty, message);
                            return true;

                        case 6:
                            Assert.AreEqual("GoToImpl End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));

            Assert.AreEqual(6, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case7()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.`stop shoot`();
        'End' >> @>log;
    }

    fun `stop shoot`()
    {
    	'Begin stop shoot' >> @>log;
		'End stop shoot' >> @>log;
    }
}";

            var hostListener = new BattleRoyaleHostListener();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("StopShootImpl Begin", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));

            Assert.AreEqual(3, maxN);
        }
    }
}

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

using NUnit.Framework;
using SymOntoClay.Core.Tests.Helpers;
using SymOntoClay.UnityAsset.Core.Tests.HostListeners;
using System;
using System.Collections.Generic;
using System.Text;
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

            BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "BooImpl Begin");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener);
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

            BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "RotateImpl Begin");
                            break;

                        case 3:
                            Assert.AreEqual(message, "30");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener);
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

            BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "RotateImpl Begin");
                            break;

                        case 3:
                            Assert.AreEqual(message, string.Empty);
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener);
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

            BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "RotateToEntityImpl Begin");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener);
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
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "TakeImpl Begin");
                            break;

                        case 3:
                            Assert.AreEqual(message, "(entity == null) = False");
                            break;

                        case 4:
                            {
                                Assert.AreNotEqual(message, string.Empty);

                                var intVal = int.Parse(message);

                                Assert.AreEqual(intVal > 0, true);
                            }
                            break;

                        case 5:
                            Assert.AreNotEqual(message, string.Empty);
                            break;

                        case 6:
                            Assert.AreEqual(message, "<10, 10, 10>");
                            break;

                        case 7:
                            Assert.AreEqual(message, "TakeImpl End");
                            break;

                        case 8:
                            Assert.AreEqual(message, "End");
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
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "TakeImpl Begin");
                            break;

                        case 3:
                            Assert.AreEqual(message, "(entity == null) = False");
                            break;

                        case 4:
                            Assert.AreEqual(message, "0");
                            break;

                        case 5:
                            Assert.AreEqual(message, string.Empty);
                            break;

                        case 6:
                            Assert.AreEqual(message, string.Empty);
                            break;

                        case 7:
                            Assert.AreEqual(message, "TakeImpl End");
                            break;

                        case 8:
                            Assert.AreEqual(message, "End");
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
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "TakeImpl Begin");
                            break;

                        case 3:
                            Assert.AreEqual(message, "(entity == null) = True");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
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

            BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl", message);
                            break;

                        case 2:
                            Assert.AreEqual("<0, 0, 0>", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener);
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

            BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl", message);
                            break;

                        case 2:
                            Assert.AreEqual("<0, 0, 0>", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener);
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

            BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("0", message);
                            break;

                        case 3:
                            Assert.AreEqual(string.Empty, message);
                            break;

                        case 4:
                            Assert.AreEqual(string.Empty, message);
                            break;

                        case 5:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener);
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

            BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("0", message);
                            break;

                        case 3:
                            Assert.AreEqual(string.Empty, message);
                            break;

                        case 4:
                            Assert.AreEqual(string.Empty, message);
                            break;

                        case 5:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener);
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

            BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("ByEntity", message);
                            break;

                        case 3:
                            Assert.AreEqual("0", message);
                            break;

                        case 4:
                            Assert.AreEqual(string.Empty, message);
                            break;

                        case 5:
                            Assert.AreEqual(string.Empty, message);
                            break;

                        case 6:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener);
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

            BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("ByEntity", message);
                            break;

                        case 3:
                            Assert.AreEqual("0", message);
                            break;

                        case 4:
                            Assert.AreEqual(string.Empty, message);
                            break;

                        case 5:
                            Assert.AreEqual(string.Empty, message);
                            break;

                        case 6:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener);
        }
    }
}

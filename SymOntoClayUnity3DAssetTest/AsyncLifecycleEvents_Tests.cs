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

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class AsyncLifecycleEvents_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        var @task = a~();
        on @task complete { 'on complete' >> @>log; };
        wait 1;
        'End' >> @>log;
    }
}";

            var wasFunCalled = false;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            if (message == "`a` has been called!")
                            {
                                wasFunCalled = true;
                            }
                            Assert.AreEqual(true, (message == "`a` has been called!" || message == "End"));
                            break;

                        case 3:
                            if (message == "`a` has been called!")
                            {
                                wasFunCalled = true;
                            }
                            Assert.AreEqual(true, (message == (wasFunCalled ? "on complete" : "`a` has been called!") || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == (wasFunCalled ? "on complete" : "`a` has been called!") || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "DB79E6CF-3B5E-41AB-A520-8B65EE07D282");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case1_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        var @task = a~();
        on @task completed { 'on completed' >> @>log; };
        wait 1;
        'End' >> @>log;
    }
}";

            var wasFunCalled = false;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            if (message == "`a` has been called!")
                            {
                                wasFunCalled = true;
                            }
                            Assert.AreEqual(true, (message == "`a` has been called!" || message == "End"));
                            break;

                        case 3:
                            if (message == "`a` has been called!")
                            {
                                wasFunCalled = true;
                            }
                            Assert.AreEqual(true, (message == (wasFunCalled ? "on completed" : "`a` has been called!") || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == (wasFunCalled ? "on completed" : "`a` has been called!") || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "08D3201F-ED70-4AE5-9F07-2C3E7C9D6B19");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case1_a()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        var @task = a~() [: timeout = 0.1 :];
        on @task weak cancel { 'on weak cancel' >> @>log; };
        wait 2;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "841A2F57-CD1B-4B90-B5FC-97248B054512");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case1_a_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        var @task = a~() [: timeout = 0.1 :];
        on @task weak canceled { 'on weak canceled' >> @>log; };
        wait 2;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "210C4956-84B5-4FA6-B9A0-F562BD1C6C15");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        var @task = a~~();
        on @task complete { 'on complete' >> @>log; };
        'End' >> @>log;
    }
}";

            var wasFunCalled = false;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            if (message == "`a` has been called!")
                            {
                                wasFunCalled = true;
                            }
                            Assert.AreEqual(true, (message == "`a` has been called!" || message == "End"));
                            break;

                        case 3:
                            if (message == "`a` has been called!")
                            {
                                wasFunCalled = true;
                            }
                            Assert.AreEqual(true, (message == (wasFunCalled ? "on complete" : "`a` has been called!") || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == (wasFunCalled ? "on complete" : "`a` has been called!") || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "7ADD49C2-5548-4975-8EDD-D843BFE45DD8");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case2_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        var @task = a~~();
        on @task completed { 'on completed' >> @>log; };
        'End' >> @>log;
    }
}";

            var wasFunCalled = false;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            if (message == "`a` has been called!")
                            {
                                wasFunCalled = true;
                            }
                            Assert.AreEqual(true, (message == "`a` has been called!" || message == "End"));
                            break;

                        case 3:
                            if (message == "`a` has been called!")
                            {
                                wasFunCalled = true;
                            }
                            Assert.AreEqual(true, (message == (wasFunCalled ? "on completed" : "`a` has been called!") || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == (wasFunCalled ? "on completed" : "`a` has been called!") || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "FEF922C1-F0A5-4099-95F2-89610C77F3DD");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        var @task = a~~() [: timeout = 0.1 :];
        on @task weak cancel { 'on weak cancel' >> @>log; };
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "A756DCCE-BCE9-48DC-8EC9-DD750139D0EA");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case2_a_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        var @task = a~~() [: timeout = 0.1 :];
        on @task weak canceled { 'on weak canceled' >> @>log; };
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "1276708E-234A-43C0-86BB-E073D183B31A");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @task = @@host.SomeVeryShortSilentFun~();
        on @task complete { 'on complete' >> @>log; };
        wait 1;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryShortMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "7AC3137A-F058-42E1-8E6B-4797E2B263F0");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case3_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @task = @@host.SomeVeryShortSilentFun~();
        on @task completed { 'on completed' >> @>log; };
        wait 1;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryShortMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on completed" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on completed" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "59D8C2FB-4872-4EC0-BF5C-4F8F7AC6481E");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case3_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @task = @@host.SomeVeryLongSilentFun~() [: timeout = 0.1 :];
        on @task weak cancel { 'on weak cancel' >> @>log; };
        wait 2;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "BF90D242-8090-424F-A4A8-92AE83638059");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case3_a_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @task = @@host.SomeVeryLongSilentFun~() [: timeout = 0.1 :];
        on @task weak canceled { 'on weak canceled' >> @>log; };
        wait 2;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "B83A6257-BF06-4ADC-96E1-A2BBAC11DDAE");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case4()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @task = @@host.SomeVeryShortSilentFun~~();
        on @task complete { 'on complete' >> @>log; };
        'End' >> @>log;
    }
}";

            var hostListener = new VeryShortMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "F16AF0DD-1BC2-414E-98C1-9D00421B0CA6");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case4_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @task = @@host.SomeVeryShortSilentFun~~();
        on @task completed { 'on completed' >> @>log; };
        'End' >> @>log;
    }
}";

            var hostListener = new VeryShortMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on completed" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on completed" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "B57848B6-A7F5-4E53-A983-1E435E689069");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case4_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @task = @@host.SomeVeryLongSilentFun~~() [: timeout = 0.1 :];
        on @task weak cancel { 'on weak cancel' >> @>log; };
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "4835A4A5-B52C-4B1F-8626-98CDC8EF6F52");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case4_a_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @task = @@host.SomeVeryLongSilentFun~~() [: timeout = 0.1 :];
        on @task weak canceled { 'on weak canceled' >> @>log; };
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "3D891564-8C61-4544-B667-D4AA4118F039");
                    }
                }, hostListener));
        }
    }
}

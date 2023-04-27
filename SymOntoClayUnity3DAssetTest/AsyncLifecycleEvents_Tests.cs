using NUnit.Framework;
using SymOntoClay.BaseTestLib;
using SymOntoClay.BaseTestLib.HostListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        wait 1000;
        'End' >> @>log;
    }
}";

            var wasFunCalled = false;

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
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
        wait 1000;
        'End' >> @>log;
    }
}";

            var wasFunCalled = false;

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case1_a()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1000;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        var @task = a~() [: timeout = 100 :];
        on @task weak cancel { 'on weak cancel' >> @>log; };
        wait 2000;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case1_a_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1000;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        var @task = a~() [: timeout = 100 :];
        on @task weak canceled { 'on weak canceled' >> @>log; };
        wait 2000;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1000;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        var @task = a~~() [: timeout = 100 :];
        on @task weak cancel { 'on weak cancel' >> @>log; };
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case2_a_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1000;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        var @task = a~~() [: timeout = 100 :];
        on @task weak canceled { 'on weak canceled' >> @>log; };
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
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
        wait 1000;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryShortMehod_HostListener();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener), true);
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
        wait 1000;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryShortMehod_HostListener();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener), true);
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
        var @task = @@host.SomeVeryLongSilentFun~() [: timeout = 100 :];
        on @task weak cancel { 'on weak cancel' >> @>log; };
        wait 2000;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMehod_HostListener();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener), true);
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
        var @task = @@host.SomeVeryLongSilentFun~() [: timeout = 100 :];
        on @task weak canceled { 'on weak canceled' >> @>log; };
        wait 2000;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMehod_HostListener();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener), true);
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

            var hostListener = new VeryShortMehod_HostListener();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener), true);
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

            var hostListener = new VeryShortMehod_HostListener();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener), true);
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
        var @task = @@host.SomeVeryLongSilentFun~~() [: timeout = 100 :];
        on @task weak cancel { 'on weak cancel' >> @>log; };
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMehod_HostListener();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener), true);
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
        var @task = @@host.SomeVeryLongSilentFun~~() [: timeout = 100 :];
        on @task weak canceled { 'on weak canceled' >> @>log; };
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMehod_HostListener();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener), true);
        }
    }
}

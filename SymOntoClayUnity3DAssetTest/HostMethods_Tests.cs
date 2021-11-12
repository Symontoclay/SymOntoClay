using NUnit.Framework;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
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
    on Init =>
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
    on Init =>
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
    on Init =>
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
    on Init =>
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
    on Init
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
    on Init
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
    on Init
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
    }
}

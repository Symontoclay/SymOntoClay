using NUnit.Framework;
using SymOntoClay.BaseTestLib;
using System;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class Properties_Tests
    {
        [Test]
        [Parallelizable]
        public void AutoProperty_Get_Test_Case1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        SomeAutoProp >> @>log;
        'End' >> @>log;
    }

    prop SomeAutoProp: number = 16;
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "16");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void AutoProperty_Get_Test_Case2()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        SomeAutoProp + 2 >> @>log;
        'End' >> @>log;
    }

    prop SomeAutoProp: number = 16;
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "18");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void AutoProperty_Set_Test_Case1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        SomeAutoProp = 15;
        SomeAutoProp >> @>log;
        'End' >> @>log;
    }

    prop SomeAutoProp: number;
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "15");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void ReadOnlyProperty_Get_Test_Case1()
        {
            var text = @"app PeaceKeeper
{
    var @b: number = 2;

    on Enter =>
    {
        'Begin' >> @>log;
        SomeGetProp >> @>log;
        'End' >> @>log;
    }

    prop SomeGetProp: number => @b;
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "2");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void ReadOnlyProperty_Get_Test_Case2()
        {
            var text = @"app PeaceKeeper
{
    var @b: number = 2;

    on Enter =>
    {
        'Begin' >> @>log;
        SomeGetProp + 3 >> @>log;
        'End' >> @>log;
    }

    prop SomeGetProp: number => @b;
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "5");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }
    }
}

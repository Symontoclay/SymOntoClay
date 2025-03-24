using NUnit.Framework;
using SymOntoClay.BaseTestLib;
using System;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class Properties_Tests
    {
        [Test]
        [Parallelizable]
        public void AutoProperty_Get_Case1()
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

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "16");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void AutoProperty_Get_Case2()
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

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "18");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void AutoProperty_Get_Case2_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        SomeAutoProp + 2 >> @>log;
        'End' >> @>log;
    }

    prop SomeAutoProp = 16;
}";

            var maxN = 0;
            
            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "18");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void AutoProperty_Get_Case2_2()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        SomeAutoProp + 2 >> @>log;
        'End' >> @>log;
    }

    SomeAutoProp = 16;
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "18");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void AutoProperty_Get_Case3()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        SomeAutoProp >> @>log;
        'End' >> @>log;
    }

    SomeAutoProp;
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "NULL");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void AutoProperty_Set_Case1()
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

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "15");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void AutoProperty_Use_As_Fact_Case1()
        {
            var text = @"app PeaceKeeper
{
    SomeVal: number = 2;

    on Enter =>
    {
        'Begin' >> @>log;
        SomeVal >> @>log;
        select {: SomeVal(I, $x) :} >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "2");
                            return true;

                        case 3:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            Assert.AreEqual(message.Contains("$x = 2"), true);
                            return true;

                        case 4:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(4, maxN);
        }

        [Test]
        [Parallelizable]
        public void ReadOnlyProperty_Get_Case1()
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

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "2");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void ReadOnlyProperty_Get_Case2()
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

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "5");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void ReadOnlyProperty_Get_Case3()
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

    prop SomeGetProp => @b;
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "2");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void ReadOnlyProperty_Get_Case4()
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

    SomeGetProp => @b;
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "2");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void ReadOnlyProperty_GetAsFact_Case1()
        {
            var text = @"app PeaceKeeper
{
    prop ReadOnlyProp: number => 16;

    on Enter =>
    {
        'Begin' >> @>log;
        ReadOnlyProp >> @>log;
        select {: ReadOnlyProp(I, $x) :} >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "16");
                            return true;

                        case 3:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            Assert.AreEqual(message.Contains("$x = 16"), true);
                            return true;

                        case 4:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(4, maxN);
        }

        [Test]
        [Parallelizable]
        public void AbsentProperty_Case1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        ReadOnlyProp >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "NULL");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void ImplicitProperty_Case1()
        {
            var text = @"app PeaceKeeper
{
    {: ImplicitProp(I, 16) :}

    on Enter =>
    {
        'Begin' >> @>log;
        ImplicitProp >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "16");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void ImplicitProperty_Case2()
        {
            var text = @"app PeaceKeeper
{
    {: ImplicitProp(I, 16) :}

    on Enter =>
    {
        'Begin' >> @>log;
        insert {: ImplicitProp(I, 22) :};
        ImplicitProp >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "22");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void ImplicitProperty_Case3()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        ImplicitProp = 15;
        ImplicitProp >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "15");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }
    }
}

using NUnit.Framework;
using SymOntoClay.Unity3DAsset.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test
{
    public class LogicConditionalTriggers_Tests
    {
        [Test]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
	on {: see(I, #`gun 1`) :} => {
	     'D' >> @>log;
	}
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case1_a()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
        insert {: see(I, #`gun 1`) :};
    }

	on {: see(I, #`gun 1`) :} => 
    {
	    'D' >> @>log;
	}
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("D"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case1_b()
        {
            var text = @"app PeaceKeeper
{
    {: see(I, #`gun 1`) :}

	on {: see(I, #`gun 1`) :} => 
    {
	    'D' >> @>log;
	}
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("D"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case2()
        {
            var text = @"app PeaceKeeper
{
    on {: see(I, $x) & barrel($x) & !focus(I, friend) :} ($x >> @x) => 
    {
	    @x >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case2_a()
        {
            var text = @"app PeaceKeeper
{
    {: barrel(#a) :}
    {: see(I, #a) :}

    on {: see(I, $x) & barrel($x) & !focus(I, friend) :} ($x >> @x) => 
    {
	    @x >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("#a"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case2_b()
        {
            var text = @"app PeaceKeeper
{
    {: barrel(#a) :}

    on Init =>
    {
        insert {: see(I, #a) :};
    }

    on {: see(I, $x) & barrel($x) & !focus(I, friend) :} ($x >> @x) => 
    {
	    @x >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("#a"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case2_c()
        {
            var text = @"app PeaceKeeper
{
    {: barrel(#a) :}
    {: see(I, #a) :}
    {: focus(I, friend) :}

    on {: see(I, $x) & barrel($x) & !focus(I, friend) :} ($x >> @x) => 
    {
	    @x >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case3()
        {
            var text = @"app PeaceKeeper
{
    {: barrel(#a) :}
    {: see(I, #a) :}

    on {: see(I, $x) & barrel($x) & !focus(I, friend) :} => 
    {
         @x >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "NULL");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }
    }
}

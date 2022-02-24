/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class LogicConditionalTriggers_Tests
    {
        [Test]
        [Parallelizable]
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
        [Parallelizable]
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
        [Parallelizable]
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
        [Parallelizable]
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
        [Parallelizable]
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
        [Parallelizable]
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
        [Parallelizable]
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
        [Parallelizable]
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

        [Test]
        [Parallelizable]
        public void Case4()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{ 
    on {: see(I, #a) :} =>
    {
        'on Fired in App' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired $x in App' >> @>log;
        @x >> @>log;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "on Fired in App");
                        break;

                    case 2:
                        Assert.AreEqual(message, "on Fired $x in App");
                        break;

                    case 3:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 4:
                        Assert.AreEqual(message, "on Fired $x in App");
                        break;

                    case 5:
                        Assert.AreEqual(message, "#b");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            var factId = npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #b) :}");

            Thread.Sleep(1000);

            npc.RemoveFact(factId);

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case5()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{ 
    on {: see(I, $x) & barrel($x) & distance(I, $x, $y) :} ($y >> @y, $x >> @x) => 
	{
	    'I see!!' >> @>log;
        @x >> @>log;
        @y >> @>log;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "I see!!");
                        break;

                    case 2:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 3:
                        Assert.AreEqual(message, "14.71526");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");
            npc.InsertFact("{: barrel (#a) :}");
            npc.InsertFact("distance(I, #a, 14.71526)");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case6()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{ 
    on {: see(I, $x) & $x is barrel & distance(I, $x, $y) :} ($y >> @y, $x >> @x) => 
	{
	    'I see!!' >> @>log;
        @x >> @>log;
        @y >> @>log;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "I see!!");
                        break;

                    case 2:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 3:
                        Assert.AreEqual(message, "14.71526");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");
            npc.InsertFact("{: barrel (#a) :}");
            npc.InsertFact("distance(I, #a, 14.71526)");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case7()
        {
            var text = @"app PeaceKeeper
{
    @a = #`gun 1`;

    on Init =>
    {
        'Begin' >> @>log;
        insert {: see(I, #`gun 1`) :};
    }

    on {: see(I, @a) :} => 
    {
	    'D' >> @>log;
	}
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "D");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case7_a()
        {
            var text = @"app PeaceKeeper
{
    @a = #`gun 1`;

    on Init =>
    {
        'Begin' >> @>log;
        insert {: see(I, #`gun 2`) :};
    }

    on {: see(I, @a) :} => 
    {
	    'D' >> @>log;
	}
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case7_b()
        {
            var text = @"app PeaceKeeper
{
    @a = #`gun 1`;

    on Init =>
    {
        'Begin' >> @>log;
        insert {: see(I, @a) :};
    }

    on {: see(I, @a) :} => 
    {
	    'D' >> @>log;
	}
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "D");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }
    }
}

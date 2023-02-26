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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Tests
{
    public class AddingFactTriggers_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    {: gun(M16) :}

	on add fact ($_ >> @x)
	{
	    'on add fact ($_ >> @x)' >> @>log;
	    @x >> @>log;
	}
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual("on add fact ($_ >> @x)", message);
                        break;

                    case 2:
                        Assert.AreEqual(message.Contains(">: { $x = {:"), true);
                        Assert.AreEqual(message.Contains(">: { act(m16,shoot) } :} & hear(i,$x) & distance(i,$x,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) } :}"), true);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            npc.EngineContext.Storage.InsertListenedFact(factStr);

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case1_a()
        {
            var text = @"app PeaceKeeper
{
    {: gun(M16) :}

	on Enter => {
	    'Begin' >> @>log;
		'End' >> @>log;
	}

	on add fact ($_ >> @x)
	{
	    'on add fact ($_ >> @x)' >> @>log;
	    @x >> @>log;
	}
}";

            Assert.AreEqual(true, BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case1_b()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    {: gun(M16) :}

	on add fact ($_ >> @x)
	{
	    'on add fact ($_ >> @x)' >> @>log;
	    @x >> @>log;
	}

	on {: hear(I, $x) & gun($x) & distance(I, $x, $y) :} ($x >> @x, $y >> @y)
    {
        @x >> @>log;
        @y >> @>log;
        '!!!M16!!!!' >> @>log;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual("on add fact ($_ >> @x)", message);
                        break;

                    case 2:
                        Assert.AreEqual(true, message.Contains(">: { $x = {:"));
                        Assert.AreEqual(true, message.Contains(">: { act(m16,shoot) } :} & hear(i,$x) & distance(i,$x,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) } :}"));
                        break;

                    case 3:
                        Assert.AreEqual("m16", message);
                        break;

                    case 4:
                        Assert.AreEqual("15.588457107543945", message);
                        break;

                    case 5:
                        Assert.AreEqual("!!!M16!!!!", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            npc.EngineContext.Storage.InsertListenedFact(factStr);

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case1_с()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    {: gun(M16) :}

	on add fact ($_ >> @x)
	{
	    'on add fact ($_ >> @x)' >> @>log;
	    @x >> @>log;
        reject;
	}

	on {: hear(I, $x) & gun($x) & distance(I, $x, $y) :} ($x >> @x, $y >> @y)
    {
        @x >> @>log;
        @y >> @>log;
        '!!!M16!!!!' >> @>log;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual("on add fact ($_ >> @x)", message);
                        break;

                    case 2:
                        Assert.AreEqual(true, message.Contains(">: { $x = {:"));
                        Assert.AreEqual(message.Contains(">: { act(m16,shoot) } :} & hear(i,$x) & distance(i,$x,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) } :}"), true);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            npc.EngineContext.Storage.InsertListenedFact(factStr);

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    {: gun(M16) :}

	on add fact
	{
	    'on add fact ($_ >> @x)' >> @>log;
	}
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual("on add fact ($_ >> @x)", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            npc.EngineContext.Storage.InsertListenedFact(factStr);

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            var text = @"app PeaceKeeper
{
    {: gun(M16) :}

	on Enter => {
	    'Begin' >> @>log;
		'End' >> @>log;
	}

	on add fact
	{
	    'on add fact ($_ >> @x)' >> @>log;
	}
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("End", message);
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
            using var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    {: gun(M16) :}

	on add fact
	{
	    'on add fact ($_ >> @x)' >> @>log;
	}

	on {: hear(I, $x) & gun($x) & distance(I, $x, $y) :} ($x >> @x, $y >> @y)
    {
        @x >> @>log;
        @y >> @>log;
        '!!!M16!!!!' >> @>log;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual("on add fact ($_ >> @x)", message);
                        break;

                    case 2:
                        Assert.AreEqual("m16", message);
                        break;

                    case 3:
                        Assert.AreEqual("15.588457107543945", message);
                        break;

                    case 4:
                        Assert.AreEqual("!!!M16!!!!", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            npc.EngineContext.Storage.InsertListenedFact(factStr);

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_c()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    {: gun(M16) :}

	on add fact
	{
	    'on add fact ($_ >> @x)' >> @>log;
        reject;
	}

	on {: hear(I, $x) & gun($x) & distance(I, $x, $y) :} ($x >> @x, $y >> @y)
    {
        @x >> @>log;
        @y >> @>log;
        '!!!M16!!!!' >> @>log;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual("on add fact ($_ >> @x)", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            npc.EngineContext.Storage.InsertListenedFact(factStr);

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    {: gun(M16) :}

	on add fact ($_ >> @x)
	{
	    'on add fact ($_ >> @x)' >> @>log;
		@x.so = 1;
	}

	on {: hear(I, $x) & gun($x) & distance(I, $x, $y) so: 1 :} ($x >> @x, $y >> @y)
    {
        @x >> @>log;
        @y >> @>log;
        '!!!M16!!!!' >> @>log;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual("on add fact ($_ >> @x)", message);
                        break;

                    case 2:
                        Assert.AreEqual("m16", message);
                        break;

                    case 3:
                        Assert.AreEqual("15.588457107543945", message);
                        break;

                    case 4:
                        Assert.AreEqual("!!!M16!!!!", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            npc.EngineContext.Storage.InsertListenedFact(factStr);

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case3_a()
        {
            var text = @"app PeaceKeeper
{
    {: gun(M16) :}

    on Enter => {
	    'Begin' >> @>log;
		'End' >> @>log;
	}

	on {: hear(I, $x) & gun($x) & distance(I, $x, $y) so: 1 :} ($x >> @x, $y >> @y)
    {
        @x >> @>log;
        @y >> @>log;
        '!!!M16!!!!' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case4()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    {: gun(M16) :}

    on add fact {: hear(I, $x) & gun($x) :} ($_ >> @x)
	{
	    'on add fact ($_ >> @x)' >> @>log;
		@x.so = 1;
	}

	on {: hear(I, $x) & gun($x) & distance(I, $x, $y) so: 1 :} ($x >> @x, $y >> @y)
    {
        @x >> @>log;
        @y >> @>log;
        '!!!M16!!!!' >> @>log;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual("on add fact ($_ >> @x)", message);
                        break;

                    case 2:
                        Assert.AreEqual("m16", message);
                        break;

                    case 3:
                        Assert.AreEqual("15.588457107543945", message);
                        break;

                    case 4:
                        Assert.AreEqual("!!!M16!!!!", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            npc.EngineContext.Storage.InsertListenedFact(factStr);

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case4_a()
        {
            var text = @"app PeaceKeeper
{
    {: cat(M16) :}

    on Enter => {
	    'Begin' >> @>log;
		'End' >> @>log;
	}

    on add fact {: hear(I, $x) & gun($x) :} ($_ >> @x)
	{
	    'on add fact ($_ >> @x)' >> @>log;
		@x.so = 1;
	}

	on {: hear(I, $x) & gun($x) & distance(I, $x, $y) so: 1 :} ($x >> @x, $y >> @y)
    {
        @x >> @>log;
        @y >> @>log;
        '!!!M16!!!!' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));
        }
    }
}

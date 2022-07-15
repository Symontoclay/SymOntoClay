using NUnit.Framework;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests
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
                        Assert.AreEqual(message.Contains(">: { $x = {:"), true);
                        Assert.AreEqual(message.Contains(">: { act(m16,shoot) } :} & hear(i,$x) & distance(i,$x,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) } :}"), true);
                        break;

                    case 3:
                        Assert.AreEqual(message, "m16");
                        break;

                    case 4:
                        Assert.AreEqual(message, "15.588457107543945");
                        break;

                    case 5:
                        Assert.AreEqual(message, "!!!M16!!!!");
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
                        Assert.AreEqual(message, "m16");
                        break;

                    case 3:
                        Assert.AreEqual(message, "15.588457107543945");
                        break;

                    case 4:
                        Assert.AreEqual(message, "!!!M16!!!!");
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
                        Assert.AreEqual(message, "m16");
                        break;

                    case 3:
                        Assert.AreEqual(message, "15.588457107543945");
                        break;

                    case 4:
                        Assert.AreEqual(message, "!!!M16!!!!");
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
                        Assert.AreEqual(message, "m16");
                        break;

                    case 3:
                        Assert.AreEqual(message, "15.588457107543945");
                        break;

                    case 4:
                        Assert.AreEqual(message, "!!!M16!!!!");
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
    }
}

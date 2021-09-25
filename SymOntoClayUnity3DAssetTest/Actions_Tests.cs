﻿using NUnit.Framework;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class Actions_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action Go 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        complete action;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "on Fired");
                        break;

                    case 4:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End Go");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case1_a()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        go();

        'End' >> @>log;
    }
}

action Go 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        complete action;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "on Fired");
                        break;

                    case 4:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End Go");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case1_b()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action Go 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        'End Go' >> @>log;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Init Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 4:
                        Assert.AreEqual(message, "End Go");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `my Go 1` alias `Go` 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        complete action;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "on Fired");
                        break;

                    case 4:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End Go");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `my Go 1` alias `Go`, Run 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        complete action;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "on Fired");
                        break;

                    case 4:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End Go");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_b()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `my Go 1` alias `Go`, `Run` 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        complete action;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "on Fired");
                        break;

                    case 4:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End Go");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_c()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `my Go 1` alias `Go` is `base app` 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        complete action;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "on Fired");
                        break;

                    case 4:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End Go");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_d()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `my Go 1` alias `Go`, Run is `base app` 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        complete action;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "on Fired");
                        break;

                    case 4:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End Go");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_e()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `my Go 1` alias `Go`, `Run` is `base app` 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        complete action;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "on Fired");
                        break;

                    case 4:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End Go");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_f()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `Go` is `base app` 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        complete action;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "on Fired");
                        break;

                    case 4:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End Go");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_g()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action Go is `base app` 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        complete action;
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "on Fired");
                        break;

                    case 4:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End Go");
                        break;

                    case 6:
                        Assert.AreEqual(message, "End");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action Go 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        break action {: attack(I, enemy) :};
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "on Fired");
                        break;

                    case 4:
                        Assert.AreEqual(message, "#a");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case3_a()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        try
        {
            Go();
        }
        catch(@e)
        {
            'catch(@e)' >> @>log;
            @e >> @>log;
        }

        'End' >> @>log;
    }
}

action Go 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        break action {: attack(I, enemy) :};
    }
}";

            instance.WriteFile(text);

            var npc = instance.CreateAndStartNPC((n, message) => {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "on Fired");
                        break;

                    case 4:
                        Assert.AreEqual(message, "#a");
                        break;

                    case 5:
                        Assert.AreEqual(message, "catch(@e)");
                        break;

                    case 6:
                        Assert.AreEqual(true, message.Contains("ERROR:"));
                        Assert.AreEqual(true, message.Contains("{:"));
                        Assert.AreEqual(true, message.Contains("attack(i,enemy)"));
                        Assert.AreEqual(true, message.Contains(":}"));
                        break;

                    case 7:
                        Assert.AreEqual(message, "End");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, #a) :}");

            Thread.Sleep(1000);
        }
    }
}
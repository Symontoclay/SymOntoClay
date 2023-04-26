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
using SymOntoClay.BaseTestLib;
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
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action Go 
{
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
    on Enter =>
    {
        'Begin' >> @>log;
 
        go();

        'End' >> @>log;
    }
}

action Go 
{
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
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action Go 
{
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
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "End Go");
                        break;

                    case 4:
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
        public void Case1_c()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"synonym go for walk;

app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
 
        walk();

        'End' >> @>log;
    }
}

action Go 
{
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
                        Assert.AreEqual(message, "Begin Go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "End Go");
                        break;

                    case 4:
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
        public void Case1_d()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go()[:on complete { 'on complete' >> @>log; } :];

        'End' >> @>log;
    }
}

action Go 
{
    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
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
                        Assert.AreEqual(message, "on complete");
                        break;

                    case 4:
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
        public void Case2()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `my Go 1` alias `Go` 
{
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
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `my Go 1` alias `Go`, Run 
{
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
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `my Go 1` alias `Go`, `Run` 
{
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
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `my Go 1` alias `Go` is `base app` 
{
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
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `my Go 1` alias `Go`, Run is `base app` 
{
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
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `my Go 1` alias `Go`, `Run` is `base app` 
{
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
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action `Go` is `base app` 
{
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
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action Go is `base app` 
{
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
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action Go 
{
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
    on Enter =>
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

        [Test]
        [Parallelizable]
        public void Case4()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action Go 
{
    on Enter =>
    {
        'Enter Go' >> @>log;
    }

    on Leave
    {
        'Leave Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        'End Go' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(true, message == "Enter Go" || message == "Begin Go");
                            break;

                        case 3:
                            Assert.AreEqual(true, message == "Enter Go" || message == "Begin Go");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End Go");
                            break;

                        case 5:
                            Assert.AreEqual(message, "Leave Go");
                            break;

                        case 6:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case5()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;

        Go();

        'End' >> @>log;
    }

    fun b()
    {
        '`b` has been called!' >> @>log;
    }
}

action Go
{
    op () => 
    {
        'Begin Go' >> @>log;
        
        a();
        b();

        'End Go' >> @>log;
    }

    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "Begin Go");
                            break;

                        case 3:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 4:
                            Assert.AreEqual(message, "`b` has been called!");
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
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case6()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go()[: on weak cancel { 'on weak cancel' >> @>log; } :];

        'End' >> @>log;
    }
}

action Go 
{
    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        weak cancel action;
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
                        Assert.AreEqual(message, "on weak cancel");
                        break;

                    case 4:
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
        public void Case7()
        {
            var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
 
        Go()[: on weak cancel { 'on weak cancel' >> @>log; } :];

        'End' >> @>log;
    }
}

action Go 
{
    op () => 
    {
        'Begin Go' >> @>log;
        await;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        cancel action;
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

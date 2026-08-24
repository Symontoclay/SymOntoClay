/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class Actions_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on Fired", message);
                        break;

                    case 4:
                        Assert.AreEqual("#a", message);
                        break;

                    case 5:
                        Assert.AreEqual("End", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "DF58F0EA-1AA5-4D80-B78A-C5D76E1D8B42");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case1_a()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on Fired", message);
                        break;

                    case 4:
                        Assert.AreEqual("#a", message);
                        break;

                    case 5:
                        Assert.AreEqual("End", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "BB79FB9D-8983-48A6-9262-ED1ED8872C6B");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case1_b()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("End Go", message);
                        break;

                    case 4:
                        Assert.AreEqual("End", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "3A6BDAA8-F1BE-4109-B3E5-09DA4FBA9CD1");
                }
            });

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case1_c()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("End Go", message);
                        break;

                    case 4:
                        Assert.AreEqual("End", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "CBE3B888-0324-4D12-9F55-D53A956D0BFA");
                }
            });

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case1_d()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on complete", message);
                        break;

                    case 4:
                        Assert.AreEqual("End", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "D8A8773B-063D-4CFB-AD6B-E420C841DAA6");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on Fired", message);
                        break;

                    case 4:
                        Assert.AreEqual("#a", message);
                        break;

                    case 5:
                        Assert.AreEqual("End", message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "DBFDFEAB-5002-4A4B-8063-4A96C6022116");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on Fired", message);
                        break;

                    case 4:
                        Assert.AreEqual("#a", message);
                        break;

                    case 5:
                        Assert.AreEqual("End", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "33B11294-6AE9-4617-A9CA-55055F6A578C");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_b()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on Fired", message);
                        break;

                    case 4:
                        Assert.AreEqual("#a", message);
                        break;

                    case 5:
                        Assert.AreEqual("End", message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "8FC8E6D0-A147-45BE-9AF5-79EFF865B6E5");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_c()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on Fired", message);
                        break;

                    case 4:
                        Assert.AreEqual("#a", message);
                        break;

                    case 5:
                        Assert.AreEqual("End", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "A4939A90-C277-450E-B6D0-463EDD83EE34");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_d()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on Fired", message);
                        break;

                    case 4:
                        Assert.AreEqual("#a", message);
                        break;

                    case 5:
                        Assert.AreEqual("End", message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "05BA61F5-3483-4ECD-8437-F2286B9D786C");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_e()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on Fired", message);
                        break;

                    case 4:
                        Assert.AreEqual("#a", message);
                        break;

                    case 5:
                        Assert.AreEqual("End", message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "B054CA40-D1F9-44CC-9A05-F7954E6B93A2");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_f()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on Fired", message);
                        break;

                    case 4:
                        Assert.AreEqual("#a", message);
                        break;

                    case 5:
                        Assert.AreEqual("End", message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "62A94C41-7D0E-4421-BBF9-6E3E2AF26697");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case2_g()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on Fired", message);
                        break;

                    case 4:
                        Assert.AreEqual("#a", message);
                        break;

                    case 5:
                        Assert.AreEqual("End", message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "731614F5-34AD-485F-AF16-7236E542C123");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on Fired", message);
                        break;

                    case 4:
                        Assert.AreEqual("#a", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "FCD6FB52-2610-4830-8710-74E8BFDB47A4");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case3_a()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on Fired", message);
                        break;

                    case 4:
                        Assert.AreEqual("#a", message);
                        break;

                    case 5:
                        Assert.AreEqual("catch(@e)", message);
                        break;

                    case 6:
                        Assert.AreEqual(true, message.Contains("ERROR"));
                        Assert.AreEqual(true, message.Contains("{:"));
                        Assert.AreEqual(true, message.Contains("attack(i,enemy)"));
                        Assert.AreEqual(true, message.Contains(":}"));
                        break;

                    case 7:
                        Assert.AreEqual("End", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "4C0A8857-0E58-4501-B6B4-B32DF6F54940");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

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

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, message == "Enter Go" || message == "Begin Go");
                            break;

                        case 3:
                            Assert.AreEqual(true, message == "Enter Go" || message == "Begin Go");
                            break;

                        case 4:
                            Assert.AreEqual("End Go", message);
                            break;

                        case 5:
                            Assert.AreEqual(true, (message == "Leave Go" || message == "End"));
                            break;

                        case 6:
                            Assert.AreEqual(true, (message == "Leave Go" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "FF0B786A-D8BA-486F-B64A-E53503B802F7");
                    }
                }));
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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("Begin Go", message);
                            return true;

                        case 3:
                            Assert.AreEqual("`a` has been called!", message);
                            return true;

                        case 4:
                            Assert.AreEqual("`b` has been called!", message);
                            return true;

                        case 5:
                            Assert.AreEqual("End Go", message);
                            return true;

                        case 6:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "266BC1A3-24D4-43C1-A19B-694E1E65A9C5");
                    }
                }));

            Assert.AreEqual(6, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case6()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("on weak cancel", message);
                        break;

                    case 4:
                        Assert.AreEqual("End", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "3474E699-E9DF-4DFE-A6FC-ADBADC427371");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case7()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "1E3EA520-EEEE-4756-B861-9B1FED444EA9");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case8()
        {
            var text = @"app PeaceKeeper
{
    on Enter
	{
	    'Begin' >> @>log;

	    move(1);
		kill(2);

		'End' >> @>log;
	}
}

action move
{
    op(@target)
	{
	    'Begin move' >> @>log;
		@target >> @>log;
		@_target = @target;
		@_target >> @>log;
		'End move' >> @>log;
	}

	private:
	    var @_target;
}

action kill
{
    op(@target)
	{
	    'Begin kill' >> @>log;
		@target >> @>log;
		@_target = @target;
		@_target >> @>log;
		'End kill' >> @>log;
	}

	private:
	    var @_target;
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual("Begin move", message);
                            return true;

                        case 3:
                            Assert.AreEqual("1", message);
                            return true;

                        case 4:
                            Assert.AreEqual("1", message);
                            return true;

                        case 5:
                            Assert.AreEqual("End move", message);
                            return true;

                        case 6:
                            Assert.AreEqual("Begin kill", message);
                            return true;

                        case 7:
                            Assert.AreEqual("2", message);
                            return true;

                        case 8:
                            Assert.AreEqual("2", message);
                            return true;

                        case 9:
                            Assert.AreEqual("End kill", message);
                            return true;

                        case 10:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "FF06D500-85BC-4C7F-9590-16A26FC13E24");
                    }
                }));

            Assert.AreEqual(10, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case8_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter
	{
	    'Begin' >> @>log;

		`go and kill`(1);

		'End' >> @>log;
	}
}

action `go and kill`
{
    op(@target)
	{
	    'Begin go and kill' >> @>log;

		move(@target);
		kill(2);

		'End go and kill' >> @>log;
	}
}

action move
{
    op(@target)
	{
	    'Begin move' >> @>log;
		'move: ' + @target >> @>log;
		'End move' >> @>log;
	}

	private:
	    var @_target;
}

action kill
{
    op(@target)
	{
	    'Begin kill' >> @>log;
		'kill: ' + @target >> @>log;
		'End kill' >> @>log;
	}

	private:
	    var @_target;
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("Begin go and kill", message);
                            return true;

                        case 3:
                            Assert.AreEqual("Begin move", message);
                            return true;

                        case 4:
                            Assert.AreEqual("move: 1", message);
                            return true;

                        case 5:
                            Assert.AreEqual("End move", message);
                            return true;

                        case 6:
                            Assert.AreEqual("Begin kill", message);
                            return true;

                        case 7:
                            Assert.AreEqual("kill: 2", message);
                            return true;

                        case 8:
                            Assert.AreEqual("End kill", message);
                            return true;

                        case 9:
                            Assert.AreEqual("End go and kill", message);
                            return true;

                        case 10:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "25983344-B8F7-437C-A49C-480A95B8802B");
                    }
                }));

            Assert.AreEqual(10, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case8_a_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter
	{
	    'Begin' >> @>log;

		`go and kill`(1);

		'End' >> @>log;
	}
}

action `go and kill`
{
    op(@target)
	{
	    'Begin go and kill' >> @>log;

		move(@target);
		kill(2);

		'End go and kill' >> @>log;
	}
}

action move
{
    op(@target)
	{
	    'Begin move' >> @>log;
		'move: ' + @target >> @>log;
		@_target = @target;
		'move (1): ' + @_target >> @>log;
		'End move' >> @>log;
	}

	private:
	    var @_target;
}

action kill
{
    op(@target)
	{
	    'Begin kill' >> @>log;
		'kill: ' + @target >> @>log;
		@_target = @target;
		'kill (1): ' + @_target >> @>log;
		'End kill' >> @>log;
	}

	private:
	    var @_target;
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("Begin go and kill", message);
                            return true;

                        case 3:
                            Assert.AreEqual("Begin move", message);
                            return true;

                        case 4:
                            Assert.AreEqual("move: 1", message);
                            return true;

                        case 5:
                            Assert.AreEqual("move (1): 1", message);
                            return true;

                        case 6:
                            Assert.AreEqual("End move", message);
                            return true;

                        case 7:
                            Assert.AreEqual("Begin kill", message);
                            return true;

                        case 8:
                            Assert.AreEqual("kill: 2", message);
                            return true;

                        case 9:
                            Assert.AreEqual("kill (1): 2", message);
                            return true;

                        case 10:
                            Assert.AreEqual("End kill", message);
                            return true;

                        case 11:
                            Assert.AreEqual("End go and kill", message);
                            return true;

                        case 12:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "83CB9BA8-90A5-4DDB-A263-68DF2A0A8FA7");
                    }
                }));

            Assert.AreEqual(12, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case9()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

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
		Run()[: on complete { 'on complete Run' >> @>log; complete action;} :];
		'After Run' >> @>log;
        await;
        'End Go' >> @>log;
    }
}

action Run
{
    op () => 
    {
        'Begin Run' >> @>log;
		Swim()[: on complete { 'on complete Swim' >> @>log; complete action;} :];
		'After Swim' >> @>log;
        await;
        'End Run' >> @>log;
    }
}

action Swim
{
    op () => 
    {
        'Begin Swim' >> @>log;
        await;
        'End Swim' >> @>log;
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
                        Assert.AreEqual("Begin", message);
                        break;

                    case 2:
                        Assert.AreEqual("Begin Go", message);
                        break;

                    case 3:
                        Assert.AreEqual("Begin Run", message);
                        break;

                    case 4:
                        Assert.AreEqual("Begin Swim", message);
                        break;

                    case 5:
                        Assert.AreEqual("on Fired", message);
                        break;

                    case 6:
                        Assert.AreEqual("#a", message);
                        break;

                    case 7:
                        Assert.AreEqual("on complete Swim", message);
                        break;

                    case 8:
                        Assert.AreEqual("on complete Run", message);
                        break;

                    case 9:
                        Assert.AreEqual("End", message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, "3872C3F7-CBD8-4AF5-BC72-9D20C48B87EE");
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact(null, "{: see(I, #a) :}");

            Thread.Sleep(1000);
        }
    }
}

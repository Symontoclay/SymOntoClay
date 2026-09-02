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
using SymOntoClay.BaseTestLib.HostListeners;
using System;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class AnnotationSystemEvent_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[:on complete { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("`a` has been called!", message);
                            return true;

                        case 3:
                            Assert.AreEqual("on complete", message);
                            return true;

                        case 4:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "064ED321-C811-4E8B-B981-057F7A8E9A4F");
                    }
                }));

            Assert.AreEqual(4, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case1_a()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[:on complete => { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("`a` has been called!", message);
                            return true;

                        case 3:
                            Assert.AreEqual("on complete", message);
                            return true;

                        case 4:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "664DD2AA-DD2D-4B96-99ED-E6BD6487C0D8");
                    }
                }));

            Assert.AreEqual(4, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case1_b()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[:on completed => { 'on completed' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("`a` has been called!", message);
                            return true;

                        case 3:
                            Assert.AreEqual("on completed", message);
                            return true;

                        case 4:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "AFCD5E61-4527-4DE4-AAA0-69E253232145");
                    }
                }));

            Assert.AreEqual(4, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case1_c()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[:on completed { 'on completed' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("`a` has been called!", message);
                            return true;

                        case 3:
                            Assert.AreEqual("on completed", message);
                            return true;

                        case 4:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "9F1DEAFD-5F6D-4B44-9E64-196C69EC190C");
                    }
                }));

            Assert.AreEqual(4, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[:on complete ~ { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("`a` has been called!", message);
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on complete" || message == "End")); 
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "00FEE4C2-69F7-4600-96DF-8FEED104F5FD");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[:on complete ~ => { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("`a` has been called!", message);
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "34294BB3-14C2-4E65-8C3C-0675125058EE");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case2_b()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[:on complete ~=> { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("`a` has been called!", message);
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "A2B92DCB-AA55-42F7-9CC0-34565278EF1F");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[:on complete { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("`a` has been called!", message);
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "50FBEC23-5732-4011-9AC4-96FA5C178252");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case3_a()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[:on complete => { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "`a` has been called!" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "3B2205EB-E7EA-4C10-B798-1B2B6F657B5B");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case3_b()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[:on complete ~ { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "`a` has been called!" || message == "on complete" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "`a` has been called!" || message == "on complete" || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == "`a` has been called!" || message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "E02ACAB4-065A-41F0-83AC-CB36BA986661");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case3_c()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[:on complete ~ => { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "`a` has been called!" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "FC863E5D-E377-44C2-B6AD-8E0BA46DAF72");
                    }
                }));
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
    op () => 
    {
        'Begin Go' >> @>log;
        
        @a = 10;

        repeat
        {
            @a >> @>log;
            @a = @a - 1;

            a()[:on complete { complete action; } :];

            if(@a > 5)
            {
                continue;
            }

            'End of while iteration' >> @>log;

            break;
        }

        'End Go' >> @>log;
    }

    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
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
                            Assert.AreEqual("10", message);
                            return true;

                        case 4:
                            Assert.AreEqual("`a` has been called!", message);
                            return true;

                        case 5:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "F3319412-9A37-4367-8850-E7707F7F8DA0");
                    }
                }));

            Assert.AreEqual(5, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case4_a()
        {
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
        'End Go' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
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
                            Assert.AreEqual("End Go", message);
                            return true;

                        case 4:
                            Assert.AreEqual("on complete", message);
                            return true;

                        case 5:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "ED495D52-78A5-4645-B700-D4726672957B");
                    }
                }));

            Assert.AreEqual(5, maxN);
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
        @@host.`rotate`(30)[:on complete { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new FullGeneralized_Tests_HostListener();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("methodName = '`rotate`'", message);
                            return true;

                        case 3:
                            Assert.AreEqual("isNamedParameters = False", message);
                            return true;

                        case 4:
                            Assert.AreEqual("on complete", message);
                            return true;

                        case 5:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "072B4A1F-6F69-4A57-8E4F-34F8C795730F");
                    }
                }, hostListener));

            Assert.AreEqual(5, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case5_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.`rotate`(30)[:on complete ~ { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new FullGeneralized_Tests_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("methodName = '`rotate`'", message);
                            break;

                        case 3:
                            Assert.AreEqual("isNamedParameters = False", message);
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        case 5:
                            Assert.AreEqual(true, (message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "6541ED9E-1BF0-49C6-A8C2-365642C3C4F7");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case5_b()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.`rotate`~(30)[:on complete { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new FullGeneralized_Tests_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "methodName = '`rotate`'" || message == "on complete" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "methodName = '`rotate`'" || message == "isNamedParameters = False" || message == "on complete" || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == "methodName = '`rotate`'" || message == "isNamedParameters = False" || message == "on complete" || message == "End"));
                            break;

                        case 5:
                            Assert.AreEqual(true, (message == "methodName = '`rotate`'" || message == "isNamedParameters = False" || message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "9E099F58-39BC-4297-8F50-3B1811C6D089");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case5_с()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.`rotate`~(30)[:on complete ~ { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new FullGeneralized_Tests_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "methodName = '`rotate`'" || message == "on complete" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "methodName = '`rotate`'" || message == "isNamedParameters = False" || message == "on complete" || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == "methodName = '`rotate`'" || message == "isNamedParameters = False" || message == "on complete" || message == "End"));
                            break;

                        case 5:
                            Assert.AreEqual(true, (message == "methodName = '`rotate`'" || message == "isNamedParameters = False" || message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "BC6EB954-A9F1-44B5-9376-0A7EA8F5ADF3");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case6()
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
    op () => 
    {
        'Begin Go' >> @>log;
        
        @a = 10;

        repeat
        {
            @a >> @>log;
            @a = @a - 1;

            @@host.`rotate`(30)[:on complete { complete action; } :];

            if(@a > 5)
            {
                continue;
            }

            'End of while iteration' >> @>log;

            break;
        }

        'End Go' >> @>log;
    }
}";

            var hostListener = new FullGeneralized_Tests_HostListener();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
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
                            Assert.AreEqual("10", message);
                            return true;

                        case 4:
                            Assert.AreEqual("methodName = '`rotate`'", message);
                            return true;

                        case 5:
                            Assert.AreEqual("isNamedParameters = False", message);
                            return true;

                        case 6:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "91F6CAC8-2973-4843-B77D-839FD0DCA695");
                    }
                }, hostListener));

            Assert.AreEqual(6, maxN);
        }


        [Test]
        [Parallelizable]
        public void Case6_a()
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
    op () => 
    {
        'Begin Go' >> @>log;
        
        @a = 10;

        repeat
        {
            @a >> @>log;
            @a = @a - 1;

            @@host.`rotate`(30)[: timeout=1, on complete { complete action; } :];

            if(@a > 5)
            {
                continue;
            }

            'End of while iteration' >> @>log;

            break;
        }

        'End Go' >> @>log;
    }
}";

            var hostListener = new FullGeneralized_Tests_HostListener();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
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
                            Assert.AreEqual("10", message);
                            return true;

                        case 4:
                            Assert.AreEqual("methodName = '`rotate`'", message);
                            return true;

                        case 5:
                            Assert.AreEqual("isNamedParameters = False", message);
                            return true;

                        case 6:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "22E0C824-D2EE-4EE9-966B-8954675407FC");
                    }
                }, hostListener));

            Assert.AreEqual(6, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case7()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[: timeout = 0.1, on weak cancel { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("on weak cancel", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "B55ED60C-6D57-447C-A12F-9A7F55931793");
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case7_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[: timeout = 0.1, on weak canceled { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("on weak canceled", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "2C0B41FD-48BF-4006-9048-C16C3F3A9ED5");
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case7_a()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[: timeout = 0.1, on weak cancel => { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("on weak cancel", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "F8CD1E5B-F8B6-452D-AAFF-2E533F94DFA3");
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case7_a_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[: timeout = 0.1, on weak canceled => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("on weak canceled", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "1405A4A8-0191-4971-91E5-92CF1FE38EA1");
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case7_b()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[: timeout = 0.1, on weak cancel ~ { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "F006D083-257B-43A0-8C29-E65DCEB43EA6");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case7_b_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[: timeout = 0.1, on weak canceled ~ { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "9636EC26-E5B8-4893-B2E0-AA5B87B8D2AD");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case7_c()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[: timeout = 0.1, on weak cancel ~ => { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "E5396297-95DD-4582-99AA-2B35574AC25A");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case7_c_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[: timeout = 0.1, on weak canceled ~ => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "0ED755DF-5F1E-4D6C-A0A2-77ECA7167E9E");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case8()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~~()[: timeout = 0.1, on weak cancel { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "E7C121A0-D4A9-4B04-B7FF-6EB22AC7060C");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case8_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~~()[: timeout = 0.1, on weak canceled { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "5CD9F9E7-1EBB-47D5-8FE2-A620D5D75C7B");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case8_a()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~~()[: timeout = 0.1, on weak cancel => { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "27806A4D-E46E-4281-B8B1-F4DC007DCB8D");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case8_a_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~~()[: timeout = 0.1, on weak canceled => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "42E2261D-8B9D-46B6-B878-EBB7FC5D3CD1");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case8_b()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~~()[: timeout = 0.1, on weak cancel ~ { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "ADF81588-806A-4092-BC76-F999E8C452BA");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case8_b_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~~()[: timeout = 0.1, on weak canceled ~ { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "42EBDE85-49AC-45C2-9C86-6F8A4CC3AC56");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case8_c()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~~()[: timeout = 0.1, on weak cancel ~ => { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "E9F9ECD4-3D60-499E-8BA1-915C83717419");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case8_c_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~~()[: timeout = 0.1, on weak canceled ~ => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "D40A75E7-5B9B-4C79-AA6C-99A792569E83");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case9()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[: timeout = 0.1, on weak cancel { 'on weak cancel' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "FEEB8561-C077-499D-99BB-6DA16BF7302D");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case9_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[: timeout = 0.1, on weak canceled { 'on weak canceled' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "05A1E2BE-911C-4AE0-A43F-A43671796B74");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case9_a()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[: timeout = 0.1, on weak cancel => { 'on weak cancel' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "48CDB6AC-C43F-4228-B764-BF95B116D00B");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case9_a_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[: timeout = 0.1, on weak canceled => { 'on weak canceled' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "3426BCF6-8B3B-4B7E-A98B-AEF93DDD908B");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case9_b()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[: timeout = 0.1, on weak cancel ~ { 'on weak cancel' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "73207C97-F9E8-4B4B-B938-3B8F9A43C077");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case9_b_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[: timeout = 0.1, on weak canceled ~ { 'on weak canceled' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "64672339-A914-4CD3-99EE-737FB1ED6022");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case9_c()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[: timeout = 0.1, on weak cancel ~ => { 'on weak cancel' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "501FFE32-C9AF-4CA8-BABF-9E3CF8EA6B7F");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case9_c_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[: timeout = 0.1, on weak canceled ~ => { 'on weak canceled' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "DD828037-55CF-42C6-A77F-535B315AB310");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case10()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun()[: timeout = 0.1, on weak cancel { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("on weak cancel", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "E88A0116-33EB-4396-8B5B-91B849551824");
                    }
                }, hostListener));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case10_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun()[: timeout = 0.1, on weak canceled { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("on weak canceled", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "8675BF69-4F8E-4A92-A905-25917B151432");
                    }
                }, hostListener));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case10_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun()[: timeout = 0.1, on weak cancel => { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("on weak cancel", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "092A8439-735D-4BF0-ADA3-2C9A5155034A");
                    }
                }, hostListener));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case10_a_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun()[: timeout = 0.1, on weak canceled => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListener(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("on weak canceled", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "0D8440E2-0166-48FA-B087-570B0AF05FA3");
                    }
                }, hostListener));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case10_b()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun()[: timeout = 0.1, on weak cancel ~ { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "25FA4FCD-DDC4-46FF-A069-0907EC7464F6");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case10_b_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun()[: timeout = 0.1, on weak canceled ~ { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "78CC150A-2F57-4616-B51E-FD54ED7B67A4");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case10_c()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun()[: timeout = 0.1, on weak cancel ~ => { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "B6C5A931-AF2A-45D9-BCD0-FAE43D104168");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case10_c_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun()[: timeout = 0.1, on weak canceled ~ => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "84C52A38-DD42-4C4E-B008-4C9717874E94");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case11()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~~()[: timeout = 0.1, on weak cancel { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "061A8B78-6639-41CA-82CB-42E1F8BE632A");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case11_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~~()[: timeout = 0.1, on weak canceled { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "E74E2C23-B038-4D7E-A477-8C552D1D65CC");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case11_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~~()[: timeout = 0.1, on weak cancel => { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "42CA581A-199D-4D58-AF1C-43CD775ECFB1");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case11_a_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~~()[: timeout = 0.1, on weak canceled => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "B23D35BE-FAED-48DC-97A2-E302BBD903CF");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case11_b()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~~()[: timeout = 0.1, on weak cancel ~ { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "FBE7B4B3-3C78-468C-B79B-4945835DE24C");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case11_b_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~~()[: timeout = 0.1, on weak canceled ~ { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "65657FF8-699B-4302-AE02-B53F6BCFB028");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case11_c()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~~()[: timeout = 0.1, on weak cancel ~ => { 'on weak cancel' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "271B19B5-95B4-4AB7-B572-7EC188D0102F");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case11_c_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~~()[: timeout = 0.1, on weak canceled ~ => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "0CF50CF0-7C0A-4D2C-8569-B1113A87D70E");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case12()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~()[: timeout = 0.1, on weak cancel { 'on weak cancel' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "44BD3F98-9A5A-4EC4-BC92-05F590063503");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case12_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~()[: timeout = 0.1, on weak canceled { 'on weak canceled' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "004782BD-FF1A-4065-80C3-85C7AD1EFB6B");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case12_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~()[: timeout = 0.1, on weak cancel => { 'on weak cancel' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "E84E6972-CAF8-42B2-8920-27349D65BCB3");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case12_a_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~()[: timeout = 0.1, on weak canceled => { 'on weak canceled' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "6A8BB629-7C40-46E9-9A15-323EABB3CD42");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case12_b()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~()[: timeout = 0.1, on weak cancel ~ { 'on weak cancel' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "E10CC0B3-E1CF-40C0-914F-37CBB762FAA4");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case12_b_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~()[: timeout = 0.1, on weak canceled ~ { 'on weak canceled' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "454B40D4-F703-4D8C-938B-811D08C1421F");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case12_c()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~()[: timeout = 0.1, on weak cancel ~ => { 'on weak cancel' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak cancel" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "56876F38-78AB-402C-AB0F-8D9D5C3EB867");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case12_c_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~()[: timeout = 0.1, on weak canceled ~ => { 'on weak canceled' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "901F2339-F4EE-413E-9BA5-083201E10A38");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case13()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~~()[: timeout = 0.1, weak cancel, on weak canceled { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "0AC84F01-AA08-44EF-98B9-839AEF131643");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case13_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun~~()[: timeout = 0.1, cancel, on weak canceled { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "064C946D-B9D7-4731-886D-3BA2A8E26D3A");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case13_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun()[: timeout = 0.1, weak cancel, on weak canceled { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "FDAEB096-CE0A-45DE-A6A2-4490A9546192");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case13_a_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.SomeVeryLongSilentFun()[: timeout = 0.1, cancel, on weak canceled { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new VeryLongMethod_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBasedWithPlatformListener(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "258F0813-4DDC-42BC-A834-76CEEA32E7EF");
                    }
                }, hostListener));
        }

        [Test]
        [Parallelizable]
        public void Case13_b()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[: timeout = 0.1, weak cancel, on weak canceled => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("on weak canceled", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "144FDF20-AFA5-4833-940B-FD6575057F7B");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case13_b_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a()[: timeout = 0.1, cancel, on weak canceled => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "34585B7C-694D-447C-91FC-CACDA6F79080");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case13_c()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~~()[: timeout = 0.1, weak cancel, on weak canceled => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "4F2FC86F-EB9F-4EDA-A041-436DA00FD771");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case13_c_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~~()[: timeout = 0.1, cancel, on weak canceled => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "675400DE-539E-4A4F-AAAB-C712354F00AE");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case13_d()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[: timeout = 0.1, weak cancel, on weak canceled => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
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
                            Assert.AreEqual(true, message == "End" || message == "on weak canceled");
                            break;

                        case 3:
                            Assert.AreEqual(true, message == "End" || message == "on weak canceled");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "F5A488E2-83FA-4E30-B890-D8F17ED69A44");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case13_d_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~~()[: timeout = 0.1, cancel, on weak canceled => { 'on weak canceled' >> @>log; } :];
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "4819DCC7-D107-4B13-9AA7-18F1C2E35CB8");
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case13_d_2()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {       
        wait 1;
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a~()[: timeout = 0.1, weak cancel, on weak canceled => { 'on weak canceled' >> @>log; } :];
        wait 2;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "on weak canceled" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "4D097767-BDFA-4085-A420-6B49F0F2E7E8");
                    }
                }));
        }
    }
}

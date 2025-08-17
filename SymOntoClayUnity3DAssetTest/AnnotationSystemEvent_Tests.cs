/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            Assert.AreEqual("methodName = 'rotate'", message);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            Assert.AreEqual(true, (message == "methodName = 'rotate'" || message == "on complete" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "methodName = 'rotate'" || message == "isNamedParameters = False" || message == "on complete" || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == "methodName = 'rotate'" || message == "isNamedParameters = False" || message == "on complete" || message == "End"));
                            break;

                        case 5:
                            Assert.AreEqual(true, (message == "methodName = 'rotate'" || message == "isNamedParameters = False" || message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            Assert.AreEqual(true, (message == "methodName = 'rotate'" || message == "on complete" || message == "End"));
                            break;

                        case 3:
                            Assert.AreEqual(true, (message == "methodName = 'rotate'" || message == "isNamedParameters = False" || message == "on complete" || message == "End"));
                            break;

                        case 4:
                            Assert.AreEqual(true, (message == "methodName = 'rotate'" || message == "isNamedParameters = False" || message == "on complete" || message == "End"));
                            break;

                        case 5:
                            Assert.AreEqual(true, (message == "methodName = 'rotate'" || message == "isNamedParameters = False" || message == "on complete" || message == "End"));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            Assert.AreEqual("methodName = 'rotate'", message);
                            return true;

                        case 5:
                            Assert.AreEqual("isNamedParameters = False", message);
                            return true;

                        case 6:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
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
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));
        }
    }
}

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
using SymOntoClay.BaseTestLib.HostListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            Assert.AreEqual("on complete", message);
                            break;

                        case 4:
                            Assert.AreEqual("End", message);
                            break;

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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            Assert.AreEqual("on complete", message);
                            break;

                        case 4:
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            Assert.AreEqual("on completed", message);
                            break;

                        case 4:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            Assert.AreEqual("on completed", message);
                            break;

                        case 4:
                            Assert.AreEqual("End", message);
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            Assert.AreEqual((message == "on complete" || message == "End"), true); 
                            break;

                        case 4:
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

                        case 4:
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

                        case 4:
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

                        case 4:
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

                        case 4:
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

                        case 4:
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
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
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

                        case 4:
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("Begin Go", message);
                            break;

                        case 3:
                            Assert.AreEqual("10", message);
                            break;

                        case 4:
                            Assert.AreEqual("`a` has been called!", message);
                            break;

                        case 5:
                            Assert.AreEqual("End", message);
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
        @@host.`rotate`(30)[:on complete { 'on complete' >> @>log; } :];
        'End' >> @>log;
    }
}";

            var hostListener = new FullGeneralized_Tests_HostListener();

            Assert.AreEqual(true, BehaviorTestEngineInstance.Run(text,
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
                            Assert.AreEqual("on complete", message);
                            break;

                        case 5:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));
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

            Assert.AreEqual(true, BehaviorTestEngineInstance.Run(text,
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
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

                        case 5:
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
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

            Assert.AreEqual(true, BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual((message == "methodName = 'rotate'" || message == "End"), true);
                            break;

                        case 3:
                            Assert.AreEqual((message == "isNamedParameters = False" || message == "End"), true);
                            break;

                        case 4:
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

                        case 5:
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
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

            Assert.AreEqual(true, BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual((message == "methodName = 'rotate'" || message == "End"), true);
                            break;

                        case 3:
                            Assert.AreEqual((message == "isNamedParameters = False" || message == "End"), true);
                            break;

                        case 4:
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
                            break;

                        case 5:
                            Assert.AreEqual((message == "on complete" || message == "End"), true);
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

            Assert.AreEqual(true, BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("Begin Go", message);
                            break;

                        case 3:
                            Assert.AreEqual("10", message);
                            break;

                        case 4:
                            Assert.AreEqual("methodName = 'rotate'", message);
                            break;

                        case 5:
                            Assert.AreEqual("isNamedParameters = False", message);
                            break;

                        case 6:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));
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

            @@host.`rotate`(30)[: timeout=1000, on complete { complete action; } :];

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

            Assert.AreEqual(true, BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("Begin Go", message);
                            break;

                        case 3:
                            Assert.AreEqual("10", message);
                            break;

                        case 4:
                            Assert.AreEqual("methodName = 'rotate'", message);
                            break;

                        case 5:
                            Assert.AreEqual("isNamedParameters = False", message);
                            break;

                        case 6:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));
        }
    }
}

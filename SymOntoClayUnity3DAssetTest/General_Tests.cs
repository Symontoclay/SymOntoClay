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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class General_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @bx >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "NULL");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        /*@r = @b = 1;
        @bx >> @>log;*/
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(2, maxN);
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
        @r = @b = 1;
        @b >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "1");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;
                            
                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
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
        @r = @b = 1;
        @bx >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "NULL");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case6()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = #@[10, 20];
        @a >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "#@[10, 20]");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case7()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @a: number = 2;
        @a >> @>log;
        var @b: number;
        @b >> @>log;
        var @c;
        @c >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "2");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "NULL");
                            return true;

                        case 4:
                            Assert.AreEqual(message, "NULL");
                            return true;

                        case 5:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(5, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case8()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @a = 2;
        @a >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "2");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case9_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @b: boolean = {: enemy($_) & see(I, $_) & alive($_, true) :};
        @b >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "0");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case9_b()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @b: boolean;
        @b = {: enemy($_) & see(I, $_) & alive($_, true) :};
        @b >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "0");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case9_c()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @b: boolean = true;
        @b >> @>log;
        @b = {: enemy($_) & see(I, $_) & alive($_, true) :};
        @b >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "1");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "0");
                            return true;

                        case 4:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(4, maxN);
        }

        /*
app PeaceKeeper
{
    on Enter =>
    {
    }
}
         */
    }
}

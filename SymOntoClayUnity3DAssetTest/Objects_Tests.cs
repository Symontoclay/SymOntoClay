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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class Objects_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;        
        var @a = {};
        'End' >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "End");
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
    on Enter
    {
        'Begin' >> @>log;        
        var @a = {
            @b = 1;
        };
        'End' >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "End");
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
    on Enter
    {
        'Begin' >> @>log;        
        var @a = {
            @b = 1;
        };
        @a.@b >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
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
    on Enter
    {
        'Begin' >> @>log;        
        var @a = {
            @b = 1;
        };
        @a.@b = 2;
        @a.@b >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "2");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
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
    on Enter
    {
        'Begin' >> @>log;
        var @c = SomeFun();
        @c.@b >> @>log;
        'End' >> @>log;
    }
    fun SomeFun()
    {
        var @a = {
            @b = 1;
        };
        return @a;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
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
    on Enter
    {
        'Begin' >> @>log;
        SomeFun().@b >> @>log;
        'End' >> @>log;
    }
    fun SomeFun()
    {
        var @a = {
            @b = 1;
        };
        return @a;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
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
    on Enter
    {
        'Begin' >> @>log;
        var @c = SomeFun();
        @c.@b = 2;
        @c.@b >> @>log;
        'End' >> @>log;
    }
    fun SomeFun()
    {
        var @a = {
            @b = 1;
        };
        return @a;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "2");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
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
    on Enter
    {
        'Begin' >> @>log;
        OtherFun({
            @b = 1;
        });
        'End' >> @>log;
    }
    fun OtherFun(@param)
    {
        @param.@b >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case4_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        OtherFun(param: {
            @b = 1;
        });
        'End' >> @>log;
    }
    fun OtherFun(@param)
    {
        @param.@b >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case4_b()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        @a = 1;

        'Begin' >> @>log;
        OtherFun(param: {
            @b = @a;
        });
        'End' >> @>log;
    }
    fun OtherFun(@param)
    {
        @param.@b >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
                            break;

                        case 3:
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
            var text = @"class class1
{
   @b = 1;
}
app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = new class1;
        @a.@b >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
                            break;

                        case 3:
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
            var text = @"class class1
{
   @b = 1;
}
app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @c = class1;
        @a = new @c;
        @a.@b >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case6_a()
        {
            var text = @"class class1
{
   @b = 1;
}
app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @b = class1;
        @a = new @b;
        @a.@b >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case7()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @c = {
            @b = 1;
        };

        @a = new @c;
        @a.@b >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
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
    on Enter
    {
        'Begin' >> @>log;
        @b = {
            @b = 1;
        };

        @a = new @b;
        @a.@b >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case8()
        {
            var text = @"class class1
{
   @b = 1;
}

app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;

        OtherFun(param: new class1);
        'End' >> @>log;
    }
    fun OtherFun(@param)
    {
        @param.@b >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case8_a()
        {
            var text = @"class class1
{
   @b = 1;
}

app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;

        @a = class1;

        OtherFun(param: new @a);
        'End' >> @>log;
    }
    fun OtherFun(@param)
    {
        @param.@b >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }
    }
}

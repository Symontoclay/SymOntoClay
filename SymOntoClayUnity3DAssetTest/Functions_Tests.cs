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
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class Functions_Tests
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
        a();
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
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
        public void Case1_a()
        {
            var text = @"synonym b for a;

app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        b();
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
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
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case2_a_1()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        @a = 1;
        a(@a);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
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
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a('Hi');
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "Hi");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case2_c()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1) 
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(dog);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "dog");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case2_c_1()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(`dog`);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "dog");
                            break;

                        case 4:
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
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(@param_1: 1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
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
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        @a = 1;
        a(@param_1: @a);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
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
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
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
            var text = @"synonym param_1 for param_2;

app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(param_2: 1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
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
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
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
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (1) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1, @param_2)
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (1) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
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
    fun a(@param_1)
    {
        '`a` (1) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1, @param_2)
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        a(3, 'Hi');
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (1) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "`a` (2) has been called!");
                            break;

                        case 5:
                            Assert.AreEqual(message, "3");
                            break;

                        case 6:
                            Assert.AreEqual(message, "Hi");
                            break;

                        case 7:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case7_a()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (1) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1, @param_2)
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        a(@param_1: 3, @param_2: 'Hi');
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (1) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "`a` (2) has been called!");
                            break;

                        case 5:
                            Assert.AreEqual(message, "3");
                            break;

                        case 6:
                            Assert.AreEqual(message, "Hi");
                            break;

                        case 7:
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
    fun a(@param_1)
    {
        '`a` (1) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1, @param_2)
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        a(param_1: 3, param_2: 'Hi');
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (1) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "`a` (2) has been called!");
                            break;

                        case 5:
                            Assert.AreEqual(message, "3");
                            break;

                        case 6:
                            Assert.AreEqual(message, "Hi");
                            break;

                        case 7:
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
            var text = @"app PeaceKeeper
{
    fun a(@param_1, @param_2 = 'Hi')
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (2) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "Hi");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case8_b()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1, @param_2 = 15)
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (2) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "15");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case8_c()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1, @param_2 = dog)
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (2) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "dog");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case8_c_1()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1, @param_2 = `dog`)
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (2) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "dog");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case8_d()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1 = 2)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case8_e()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1, @param_2 = 42)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "42");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case9_a_1()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
    }

    fun a(@param_1: string)
    {
        '`a` (string) has been called!' >> @>log;
    }

    fun a(@param_1: number)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case9_a_2()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
    }

    fun a(@param_1: string)
    {
        '`a` (string) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: number)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(param_1: 'Hi');
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (string) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "Hi");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case9_a_3()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: string)
    {
        '`a` (string) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: number)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(param_1: dog);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (any) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "dog");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case9_a_4()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: string)
    {
        '`a` (string) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: number)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(param_1: `dog`);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (any) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "dog");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case10_a_1()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
    }

    fun a(@param_1: string)
    {
        '`a` (string) has been called!' >> @>log;
    }

    fun a(@param_1: number)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case10_a_2()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
    }

    fun a(@param_1: string)
    {
        '`a` (string) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: number)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a('Hi');
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (string) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "Hi");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case10_a_3()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: string)
    {
        '`a` (string) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: number)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(dog);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (any) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "dog");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case10_a_4()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: string)
    {
        '`a` (string) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: number)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(`dog`);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (any) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "dog");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case10_a_5()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: string)
    {
        '`a` (string) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: number)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: fuzzy)
    {
        '`a` (fuzzy) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (fuzzy) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case11()
        {
            var text = @"fun a(@param_1)
{
    '`a` (any) has been called!' >> @>log;
    @param_1 >> @>log;
}

app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (any) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case11_a()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }
    on Enter =>
    {
        'Begin' >> @>log;
        @param_1 = 12;
        a(@param_1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case12_a()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: (number | string))
    {
        '`a` (number | string) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        a(param_1: 'Hi');
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (number | string) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "`a` (number | string) has been called!");
                            break;

                        case 5:
                            Assert.AreEqual(message, "Hi");
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
        public void Case12_b()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: number | string)
    {
        '`a` (number | string) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        a(param_1: 'Hi');
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (number | string) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "`a` (number | string) has been called!");
                            break;

                        case 5:
                            Assert.AreEqual(message, "Hi");
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
        public void Case12_c()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: number | string)
    {
        '`a` (number | string) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: fuzzy)
    {
        '`a` (fuzzy) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        a(param_1: 'Hi');
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (fuzzy) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "`a` (number | string) has been called!");
                            break;

                        case 5:
                            Assert.AreEqual(message, "Hi");
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
        public void Case13()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        a~();
        wait 1000;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case14()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        a~~();        
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case15()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;

        @a = 10;

        while (@a > 0)
        {
            @a >> @>log;
            @a = @a - 1;
        }

        'End `a`' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;

        @a = a~();

        wait @a;

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
                            Assert.AreEqual("10", message);
                            break;

                        case 4:
                            Assert.AreEqual("9", message);
                            break;

                        case 5:
                            Assert.AreEqual("8", message);
                            break;

                        case 6:
                            Assert.AreEqual("7", message);
                            break;

                        case 7:
                            Assert.AreEqual("6", message);
                            break;

                        case 8:
                            Assert.AreEqual("5", message);
                            break;

                        case 9:
                            Assert.AreEqual("4", message);
                            break;

                        case 10:
                            Assert.AreEqual("3", message);
                            break;

                        case 11:
                            Assert.AreEqual("2", message);
                            break;

                        case 12:
                            Assert.AreEqual("1", message);
                            break;

                        case 13:
                            Assert.AreEqual("End `a`", message);
                            break;

                        case 14:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case16()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;

        @a = 10;

        while (@a > 0)
        {
            @a >> @>log;
            @a = @a - 1;
        }

        'End `a`' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;

        @a = a~~()[: priority = 1 :];

        wait @a;

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
                            Assert.AreEqual("10", message);
                            break;

                        case 4:
                            Assert.AreEqual("9", message);
                            break;

                        case 5:
                            Assert.AreEqual("8", message);
                            break;

                        case 6:
                            Assert.AreEqual("7", message);
                            break;

                        case 7:
                            Assert.AreEqual("6", message);
                            break;

                        case 8:
                            Assert.AreEqual("5", message);
                            break;

                        case 9:
                            Assert.AreEqual("4", message);
                            break;

                        case 10:
                            Assert.AreEqual("3", message);
                            break;

                        case 11:
                            Assert.AreEqual("2", message);
                            break;

                        case 12:
                            Assert.AreEqual("1", message);
                            break;

                        case 13:
                            Assert.AreEqual("End `a`", message);
                            break;

                        case 14:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case16_a()
        {
            var text = @"app PeaceKeeper
{
    fun a() with priority = 1 => 
    {
        '`a` has been called!' >> @>log;

        @a = 10;

        while (@a > 0)
        {
            @a >> @>log;
            @a = @a - 1;
        }

        'End `a`' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;

        @a = a~~();

        wait @a;

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
                            Assert.AreEqual("10", message);
                            break;

                        case 4:
                            Assert.AreEqual("9", message);
                            break;

                        case 5:
                            Assert.AreEqual("8", message);
                            break;

                        case 6:
                            Assert.AreEqual("7", message);
                            break;

                        case 7:
                            Assert.AreEqual("6", message);
                            break;

                        case 8:
                            Assert.AreEqual("5", message);
                            break;

                        case 9:
                            Assert.AreEqual("4", message);
                            break;

                        case 10:
                            Assert.AreEqual("3", message);
                            break;

                        case 11:
                            Assert.AreEqual("2", message);
                            break;

                        case 12:
                            Assert.AreEqual("1", message);
                            break;

                        case 13:
                            Assert.AreEqual("End `a`", message);
                            break;

                        case 14:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case16_a_1()
        {
            var text = @"app PeaceKeeper
{
    fun a() with priority = 1 
    {
        '`a` has been called!' >> @>log;

        @a = 10;

        while (@a > 0)
        {
            @a >> @>log;
            @a = @a - 1;
        }

        'End `a`' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;

        @a = a~~();

        wait @a;

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
                            Assert.AreEqual("10", message);
                            break;

                        case 4:
                            Assert.AreEqual("9", message);
                            break;

                        case 5:
                            Assert.AreEqual("8", message);
                            break;

                        case 6:
                            Assert.AreEqual("7", message);
                            break;

                        case 7:
                            Assert.AreEqual("6", message);
                            break;

                        case 8:
                            Assert.AreEqual("5", message);
                            break;

                        case 9:
                            Assert.AreEqual("4", message);
                            break;

                        case 10:
                            Assert.AreEqual("3", message);
                            break;

                        case 11:
                            Assert.AreEqual("2", message);
                            break;

                        case 12:
                            Assert.AreEqual("1", message);
                            break;

                        case 13:
                            Assert.AreEqual("End `a`", message);
                            break;

                        case 14:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case16_a_2()
        {
            var text = @"app PeaceKeeper
{
    fun a() with priority 1 
    {
        '`a` has been called!' >> @>log;

        @a = 10;

        while (@a > 0)
        {
            @a >> @>log;
            @a = @a - 1;
        }

        'End `a`' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;

        @a = a~~();

        wait @a;

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
                            Assert.AreEqual("10", message);
                            break;

                        case 4:
                            Assert.AreEqual("9", message);
                            break;

                        case 5:
                            Assert.AreEqual("8", message);
                            break;

                        case 6:
                            Assert.AreEqual("7", message);
                            break;

                        case 7:
                            Assert.AreEqual("6", message);
                            break;

                        case 8:
                            Assert.AreEqual("5", message);
                            break;

                        case 9:
                            Assert.AreEqual("4", message);
                            break;

                        case 10:
                            Assert.AreEqual("3", message);
                            break;

                        case 11:
                            Assert.AreEqual("2", message);
                            break;

                        case 12:
                            Assert.AreEqual("1", message);
                            break;

                        case 13:
                            Assert.AreEqual("End `a`", message);
                            break;

                        case 14:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case17()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = fun(){return 1;};
        @a() >> @>log;
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
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case17_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        fun(){return 1;}() >> @>log;
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
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case17_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = SomeFun();
        @a() >> @>log;
        'End' >> @>log;
    }

    fun SomeFun()
    {
        return fun(){return 1;};
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
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case17_b()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = OtherFun(SomeFun());
        @a >> @>log;
        'End' >> @>log;
    }

    fun SomeFun()
    {
        return fun(){return 1;};
    }

    fun OtherFun(@param)
    {
        return @param();
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
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case17_b_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = OtherFun(param: SomeFun());
        @a >> @>log;
        'End' >> @>log;
    }

    fun SomeFun()
    {
        return fun(){return 1;};
    }

    fun OtherFun(@param)
    {
        return @param();
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
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case17_b_2()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = OtherFun(@param: SomeFun());
        @a >> @>log;
        'End' >> @>log;
    }

    fun SomeFun()
    {
        return fun(){return 1;};
    }

    fun OtherFun(@param)
    {
        return @param();
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
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case17_c()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = OtherFun(fun(){return 1;});
        @a >> @>log;
        'End' >> @>log;
    }

    fun OtherFun(@param)
    {
        return @param();
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
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case17_c_1()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = OtherFun(param: fun(){return 1;});
        @a >> @>log;
        'End' >> @>log;
    }

    fun OtherFun(@param)
    {
        return @param();
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
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case17_c_2()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = OtherFun(@param: fun(){return 1;});
        @a >> @>log;
        'End' >> @>log;
    }

    fun OtherFun(@param)
    {
        return @param();
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
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case18()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = fun(@param1){return @param1 + 1;};
        @a(2) >> @>log;
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
                            Assert.AreEqual("3", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case19()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        SomeFun()() >> @>log;
        'End' >> @>log;
    }

    fun SomeFun()
    {
        return fun(){return 1;};
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
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case20()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        SomeFun()() >> @>log;
        'End' >> @>log;
    }

    fun SomeFun()
    {
        @a = 2;
        return fun(){return @a + 1;};
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
                            Assert.AreEqual("3", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case20_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        SomeFun(2)() >> @>log;
        'End' >> @>log;
    }

    fun SomeFun(@a)
    {
        return fun(){return @a + 1;};
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
                            Assert.AreEqual("3", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case21()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        OtherFun()() >> @>log;
        'End' >> @>log;
    }

    fun OtherFun()
    {
        return SomeFun;
    }

    fun SomeFun()
    {
        return 1;
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
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case22()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        OtherFun(SomeFun);
        'End' >> @>log;
    }

    fun OtherFun(@fn)
    {
        @fn() >> @>log;
    }

    fun SomeFun()
    {
        return 1;
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
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case23()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        fun SomeFun()
        {
            return 1;
        }

        'Begin' >> @>log;
        SomeFun() >> @>log;
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
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }
    }
}

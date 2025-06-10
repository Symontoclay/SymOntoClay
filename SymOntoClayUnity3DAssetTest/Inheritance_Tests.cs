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
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class Inheritance_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"app PeaceKeeper is [0.5] exampleClass
{
    on Enter =>
    {
        'Begin' >> @>log;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        set exampleClass is [0.5] human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        set exampleClass is not human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        set @@self is linux;
        @@self is linux >> @>log;
        'End' >> @>log;
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
                            Assert.AreEqual("0", message);
                            return true;

                        case 3:
                            Assert.AreEqual("1", message);
                            return true;

                        case 4:
                            Assert.AreEqual("0.5", message);
                            return true;

                        case 5:
                            Assert.AreEqual("0.5", message);
                            return true;

                        case 6:
                            Assert.AreEqual("0.5", message);
                            return true;

                        case 7:
                            Assert.AreEqual("0.5", message);
                            return true;

                        case 8:
                            Assert.AreEqual("0", message);
                            return true;

                        case 9:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(9, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case1_a()
        {
            var text = @"app PeaceKeeper is [0.5] `exampleClass`
{
    on Enter =>
    {
        'Begin' >> @>log;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        set exampleClass is [0.5] human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        set exampleClass is not human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        set @@self is linux;
        @@self is linux >> @>log;
        'End' >> @>log;
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
                            Assert.AreEqual(,message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(,message, "0");
                            return true;

                        case 3:
                            Assert.AreEqual(,message, "1");
                            return true;

                        case 4:
                            Assert.AreEqual(,message, "0.5");
                            return true;

                        case 5:
                            Assert.AreEqual(,message, "0.5");
                            return true;

                        case 6:
                            Assert.AreEqual(,message, "0.5");
                            return true;

                        case 7:
                            Assert.AreEqual(,message, "0.5");
                            return true;

                        case 8:
                            Assert.AreEqual(,message, "0");
                            return true;

                        case 9:
                            Assert.AreEqual(,message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(9, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case1_b()
        {
            var text = @"app PeaceKeeper is [0.5] exampleClass, [0.6] humanoid
{
    on Enter =>
    {
        'Begin' >> @>log;
        'End' >> @>log;
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
                            Assert.AreEqual(,message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(,message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case1_c()
        {
            var text = @"app PeaceKeeper is exampleClass, humanoid
{
    on Enter =>
    {
        'Begin' >> @>log;
        'End' >> @>log;
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
                            Assert.AreEqual(,message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(,message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case1_d()
        {
            var text = @"app PeaceKeeper is `exampleClass`, `humanoid`
{
    on Enter =>
    {
        'Begin' >> @>log;
        'End' >> @>log;
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
                            Assert.AreEqual(,message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(,message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var text = @"linvar logic for range [0, 1]
{
    constraints:
	    for inheritance;

	terms:
		minimal = L(0, 0.1);
		low = Trapezoid(0, 0.05, 0.3, 0.45);
		middle = Trapezoid(0.3, 0.4, 0.6, 0.7);
		high = Trapezoid(0.55, 0.7, 0.95, 1);
		maximal = S(0.9, 1);
}

app PeaceKeeper is [middle] exampleClass
{
    on Enter =>
    {
        'Begin' >> @>log;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        set exampleClass is [middle] human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        set exampleClass is not human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        set @@self is linux;
        @@self is linux >> @>log;
        'End' >> @>log;
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
                            Assert.AreEqual(,message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(,message, "0");
                            return true;

                        case 3:
                            Assert.AreEqual(,message, "1");
                            return true;

                        case 4:
                            Assert.AreEqual(,message, "0.5");
                            return true;

                        case 5:
                            Assert.AreEqual(,message, "0.5");
                            return true;

                        case 6:
                            Assert.AreEqual(,message, "0.5");
                            return true;

                        case 7:
                            Assert.AreEqual(,message, "0.5");
                            return true;

                        case 8:
                            Assert.AreEqual(,message, "0");
                            return true;

                        case 9:
                            Assert.AreEqual(,message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(9, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            var text = @"linvar logic for range [0, 1]
{
    constraints:
	    for inheritance;

	terms:
		minimal = L(0, 0.1);
		low = Trapezoid(0, 0.05, 0.3, 0.45);
		middle = Trapezoid(0.3, 0.4, 0.6, 0.7);
		high = Trapezoid(0.55, 0.7, 0.95, 1);
		maximal = S(0.9, 1);
}

app PeaceKeeper is [very middle] exampleClass
{
    on Enter =>
    {
        'Begin' >> @>log;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        set exampleClass is [very middle] human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        set exampleClass is not human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        set @@self is linux;
        @@self is linux >> @>log;
        'End' >> @>log;
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
                            Assert.AreEqual(,message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(,message, "0");
                            return true;

                        case 3:
                            Assert.AreEqual(,message, "1");
                            return true;

                        case 4:
                            Assert.AreEqual(,message, "0.5");
                            return true;

                        case 5:
                            Assert.AreEqual(,message, "0.5");
                            return true;

                        case 6:
                            Assert.AreEqual(,message, "0.5");
                            return true;

                        case 7:
                            Assert.AreEqual(,message, "0.5");
                            return true;

                        case 8:
                            Assert.AreEqual(,message, "0");
                            return true;

                        case 9:
                            Assert.AreEqual(,message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(9, maxN);
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

         set @@self is linux;
         set exampleClass is human;
         set exampleClass is [0.5] human;
         set #`Alisa 12` is [0.6] human;

        'End' >> @>log;
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
                            Assert.AreEqual(,message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(,message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case5()
        {
            var categories = new List<string>() { "elf" };

            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;

        @@self is elf >> @>log;

        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithCategories(text, categories, (n, message) => 
            {
                maxN = n;

                switch (n)
                {
                    case 1:
                        Assert.AreEqual(,message, "Begin");
                        return true;

                    case 2:
                        Assert.AreEqual(,message, "1");
                        return true;

                    case 3:
                        Assert.AreEqual(,message, "End");
                        return false;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            }));

            Assert.AreEqual(3, maxN);
        }
    }
}

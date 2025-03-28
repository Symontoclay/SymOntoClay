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

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "0");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 5:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 6:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 7:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 8:
                            Assert.AreEqual(message, "0");
                            break;

                        case 9:
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

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "0");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 5:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 6:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 7:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 8:
                            Assert.AreEqual(message, "0");
                            break;

                        case 9:
                            Assert.AreEqual(message, "End");
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
            var text = @"app PeaceKeeper is [0.5] exampleClass, [0.6] humanoid
{
    on Enter =>
    {
        'Begin' >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
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

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
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

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
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

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "0");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 5:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 6:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 7:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 8:
                            Assert.AreEqual(message, "0");
                            break;

                        case 9:
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

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "0");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 5:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 6:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 7:
                            Assert.AreEqual(message, "0.5");
                            break;

                        case 8:
                            Assert.AreEqual(message, "0");
                            break;

                        case 9:
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

         set @@self is linux;
         set exampleClass is human;
         set exampleClass is [0.5] human;
         set #`Alisa 12` is [0.6] human;

        'End' >> @>log;
    }
}";

            Assert.AreEqual(OldBehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
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
        public void Case5()
        {
            var settings = new AdvancedBehaviorTestEngineInstanceSettings()
            {
                Categories = new List<string>() { "elf" },
                EnableCategories = true
            };

            var instance = new AdvancedBehaviorTestEngineInstance(settings);

            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;

        @@self is elf >> @>log;

        'End' >> @>log;
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
                        Assert.AreEqual(message, "1");
                        break;

                    case 3:
                        Assert.AreEqual(message, "End");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            });

            Thread.Sleep(2000);
        }
    }
}

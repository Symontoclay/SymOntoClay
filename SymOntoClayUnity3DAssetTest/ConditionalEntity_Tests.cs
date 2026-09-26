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

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class ConditionalEntity_Tests
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
        @a = #@(barrel);
        @a >> @>log;
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
                            Assert.AreEqual("#@(`barrel`)", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "D0D98106-AE62-4ADD-A3EB-1AA9C4D81556");
                    }
                }));

            Assert.AreEqual(3, maxN);
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
        @a = #@{: barrel($_) :};
        @a >> @>log;
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
                            Assert.AreEqual(true, message.StartsWith("#@{:"));
                            Assert.AreEqual(true, message.EndsWith(">: { `barrel`($`_`) } :}"));
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "634A1953-229C-48AF-A1A5-283BCC69F1C3");
                    }
                }));

            Assert.AreEqual(3, maxN);
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
        @a = #@{: >: { barrel($_) } :};
        @a >> @>log;
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
                            Assert.AreEqual(true, message.StartsWith("#@{:"));
                            Assert.AreEqual(true, message.EndsWith(">: { `barrel`($`_`) } :}"));
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "8296E747-3453-49D1-BEAC-5E5430FE32DF");
                    }
                }));

            Assert.AreEqual(3, maxN);
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
        @a = #@(hold(I, this) & weapon);
        @a >> @>log;
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
                            Assert.AreEqual("#@(`hold`(`I`,`this`) & `weapon`)", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "A2380ACF-603C-4556-863F-4E263FD53C72");
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case5()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = #@(hold(I, this) & (weapon & dog) );
        @a >> @>log;
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
                            Assert.AreEqual("#@(`hold`(`I`,`this`) & (`weapon` & `dog`))", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "2E041F22-4FCC-4858-9DEA-BBD7F6E0D54F");
                    }
                }));

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
        @a = #@(hold(#a, this) & (weapon & dog) );
        @a >> @>log;
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
                            Assert.AreEqual("#@(`hold`(#`a`,`this`) & (`weapon` & `dog`))", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "85305851-EBCC-4977-ACD5-C2FE68F44E9D");
                    }
                }));

            Assert.AreEqual(3, maxN);
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
        @a = #@(color = black);
        @a >> @>log;
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
                            Assert.AreEqual("#@(`color` = `black`)", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "A93A2D81-00A8-4885-B84B-BA469E913FFA");
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
    on Enter
    {
        'Begin' >> @>log;
        #@(color = black) >> @>log;
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
                            Assert.AreEqual("#@(`color` = `black`)", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "28263102-B4F4-4C8E-9F8D-FE81E80C131D");
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case8()
        {
            var text = @"app PeaceKeeper
{
    {: like(i, #@{: >: { cat($_) & possess(i, $_) } :}) :}

    on Enter =>
    {
        'Begin' >> @>log;

        select {: like(i, $x) :} >> @>log;

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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$`x` = #@{:"));
                            Assert.AreEqual(true, message.Contains(">: { `cat`($`_`) & `possess`(`i`,$`_`) } :}"));
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "A84003D2-1190-4D30-A522-0E66120ADB6F");
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case9()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = #@(barrel & random);
        @a >> @>log;
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
                            Assert.AreEqual("#@(`barrel` & `random`)", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "01D04864-762B-496A-9A5D-BB74BCC5D10C");
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case9_a()
        {
            var text = @"app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = #@(barrel & nearest);
        @a >> @>log;
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
                            Assert.AreEqual("#@(`barrel` & `nearest`)", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "3377D034-92C6-4569-BE2F-C6367AE94E4A");
                    }
                }));

            Assert.AreEqual(3, maxN);
        }
    }
}

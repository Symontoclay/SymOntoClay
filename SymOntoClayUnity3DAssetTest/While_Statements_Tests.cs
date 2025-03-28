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

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class While_Statements_Tests
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
        
        @a = 10;

        while (@a > 0)
        {
            @a >> @>log;
            @a = @a - 1;
        }

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
                            Assert.AreEqual(message, "10");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "9");
                            return true;

                        case 4:
                            Assert.AreEqual(message, "8");
                            return true;

                        case 5:
                            Assert.AreEqual(message, "7");
                            return true;

                        case 6:
                            Assert.AreEqual(message, "6");
                            return true;

                        case 7:
                            Assert.AreEqual(message, "5");
                            return true;

                        case 8:
                            Assert.AreEqual(message, "4");
                            return true;

                        case 9:
                            Assert.AreEqual(message, "3");
                            return true;

                        case 10:
                            Assert.AreEqual(message, "2");
                            return true;

                        case 11:
                            Assert.AreEqual(message, "1");
                            return true;

                        case 12:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(12, maxN);
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
        
        @a = 10;

        while (@a > 0)
        {
            @a >> @>log;
            @a = @a - 1;

            if(@a > 5)
            {
                break;
            }
        }

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
                            Assert.AreEqual(message, "10");
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
        
        @a = 10;

        while (@a > 0)
        {
            @a >> @>log;
            @a = @a - 1;

            if(@a > 5)
            {
                continue;
            }

            'End of while iteration' >> @>log;
        }

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
                            Assert.AreEqual(message, "10");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "9");
                            return true;

                        case 4:
                            Assert.AreEqual(message, "8");
                            return true;

                        case 5:
                            Assert.AreEqual(message, "7");
                            return true;

                        case 6:
                            Assert.AreEqual(message, "6");
                            return true;

                        case 7:
                            Assert.AreEqual(message, "End of while iteration");
                            return true;

                        case 8:
                            Assert.AreEqual(message, "5");
                            return true;

                        case 9:
                            Assert.AreEqual(message, "End of while iteration");
                            return true;

                        case 10:
                            Assert.AreEqual(message, "4");
                            return true;

                        case 11:
                            Assert.AreEqual(message, "End of while iteration");
                            return true;

                        case 12:
                            Assert.AreEqual(message, "3");
                            return true;

                        case 13:
                            Assert.AreEqual(message, "End of while iteration");
                            return true;

                        case 14:
                            Assert.AreEqual(message, "2");
                            return true;

                        case 15:
                            Assert.AreEqual(message, "End of while iteration");
                            return true;

                        case 16:
                            Assert.AreEqual(message, "1");
                            return true;

                        case 17:
                            Assert.AreEqual(message, "End of while iteration");
                            return true;

                        case 18:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(18, maxN);
        }
    }
}

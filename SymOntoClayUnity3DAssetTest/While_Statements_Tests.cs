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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "10");
                            break;

                        case 3:
                            Assert.AreEqual(message, "9");
                            break;

                        case 4:
                            Assert.AreEqual(message, "8");
                            break;

                        case 5:
                            Assert.AreEqual(message, "7");
                            break;

                        case 6:
                            Assert.AreEqual(message, "6");
                            break;

                        case 7:
                            Assert.AreEqual(message, "5");
                            break;

                        case 8:
                            Assert.AreEqual(message, "4");
                            break;

                        case 9:
                            Assert.AreEqual(message, "3");
                            break;

                        case 10:
                            Assert.AreEqual(message, "2");
                            break;

                        case 11:
                            Assert.AreEqual(message, "1");
                            break;

                        case 12:
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "10");
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "10");
                            break;

                        case 3:
                            Assert.AreEqual(message, "9");
                            break;

                        case 4:
                            Assert.AreEqual(message, "8");
                            break;

                        case 5:
                            Assert.AreEqual(message, "7");
                            break;

                        case 6:
                            Assert.AreEqual(message, "6");
                            break;

                        case 7:
                            Assert.AreEqual(message, "End of while iteration");
                            break;

                        case 8:
                            Assert.AreEqual(message, "5");
                            break;

                        case 9:
                            Assert.AreEqual(message, "End of while iteration");
                            break;

                        case 10:
                            Assert.AreEqual(message, "4");
                            break;

                        case 11:
                            Assert.AreEqual(message, "End of while iteration");
                            break;

                        case 12:
                            Assert.AreEqual(message, "3");
                            break;

                        case 13:
                            Assert.AreEqual(message, "End of while iteration");
                            break;

                        case 14:
                            Assert.AreEqual(message, "2");
                            break;

                        case 15:
                            Assert.AreEqual(message, "End of while iteration");
                            break;

                        case 16:
                            Assert.AreEqual(message, "1");
                            break;

                        case 17:
                            Assert.AreEqual(message, "End of while iteration");
                            break;

                        case 18:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }
    }
}

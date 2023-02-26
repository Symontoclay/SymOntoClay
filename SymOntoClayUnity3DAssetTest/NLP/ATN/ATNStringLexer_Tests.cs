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
using SymOntoClay.NLP.Internal.ATN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.NLP.ATN
{
    public class ATNStringLexer_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = "I like my cat.()!.?,...:;-1234567890 M1$nrg, #erty3, @maror, 3% can't \"";

            var lexer = new ATNStringLexer(text);
            (string, int, int) item;

            var n = 0;

            while ((item = lexer.GetItem()).Item1 != null)
            {
                n++;

                switch(n)
                {
                    case 1:
                        Assert.AreEqual("I", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(1, item.Item3);
                        break;

                    case 2:
                        Assert.AreEqual("like", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(3, item.Item3);
                        break;

                    case 3:
                        Assert.AreEqual("my", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(8, item.Item3);
                        break;

                    case 4:
                        Assert.AreEqual("cat", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(11, item.Item3);
                        break;

                    case 5:
                        Assert.AreEqual(".", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(14, item.Item3);
                        break;

                    case 6:
                        Assert.AreEqual("(", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(15, item.Item3);
                        break;

                    case 7:
                        Assert.AreEqual(")", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(16, item.Item3);
                        break;

                    case 8:
                        Assert.AreEqual("!", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(17, item.Item3);
                        break;

                    case 9:
                        Assert.AreEqual(".", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(18, item.Item3);
                        break;

                    case 10:
                        Assert.AreEqual("?", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(19, item.Item3);
                        break;

                    case 11:
                        Assert.AreEqual(",", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(20, item.Item3);
                        break;

                    case 12:
                        Assert.AreEqual("...", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(21, item.Item3);
                        break;

                    case 13:
                        Assert.AreEqual(":", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(24, item.Item3);
                        break;

                    case 14:
                        Assert.AreEqual(";", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(25, item.Item3);
                        break;

                    case 15:
                        Assert.AreEqual("-", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(26, item.Item3);
                        break;

                    case 16:
                        Assert.AreEqual("1234567890", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(27, item.Item3);
                        break;

                    case 17:
                        Assert.AreEqual("M1$nrg", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(38, item.Item3);
                        break;

                    case 18:
                        Assert.AreEqual(",", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(44, item.Item3);
                        break;

                    case 19:
                        Assert.AreEqual("#erty3", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(46, item.Item3);
                        break;

                    case 20:
                        Assert.AreEqual(",", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(52, item.Item3);
                        break;

                    case 21:
                        Assert.AreEqual("@maror", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(54, item.Item3);
                        break;

                    case 22:
                        Assert.AreEqual(",", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(60, item.Item3);
                        break;

                    case 23:
                        Assert.AreEqual("3%", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(62, item.Item3);
                        break;

                    case 24:
                        Assert.AreEqual("can't", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(65, item.Item3);
                        break;

                    case 25:
                        Assert.AreEqual("\"", item.Item1);
                        Assert.AreEqual(1, item.Item2);
                        Assert.AreEqual(71, item.Item3);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(n), n, null);
                }
            }
        }
    }
}

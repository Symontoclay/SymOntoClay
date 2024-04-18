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
    public class Constructors_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
    ctor()
    {
        'Begin' >> @>log;
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
            var text = @"class cls0
{
    ctor(@param: string)
    {
        'Begin ctor 1 of cls0' >> @>log;
        @param >> @>log;
        @b >> @>log;
        'End ctor 1 of cls0' >> @>log;
    }

    ctor(@param: number)
    {
        'Begin ctor 2 of cls0' >> @>log;
        @param >> @>log;
        @b >> @>log;
        'End ctor 2 of cls0' >> @>log;
    }

    private:
        @b = 0;
}

class cls1 is cls0
{
    ctor(@param: string)
        : cls0('Cool!')
    {
        'Begin ctor of cls1' >> @>log;
        @param >> @>log;
        @b >> @>log;
        'End ctor of cls1' >> @>log;
    }

    private:
        @b = 1;
}

class cls2 is cls0
{
    ctor(@param: number)
        : cls0(16)
    {
        'Begin ctor of cls2' >> @>log;
        @param >> @>log;
        @b >> @>log;
        'End ctor of cls2' >> @>log;
    }

    private:
        @b = 2;
}

app PeaceKeeper is cls1, cls2
{
    ctor()
        : ('Hi')
    {
        'Begin ctor 1 of PeaceKeeper' >> @>log;
        @b >> @>log;
        'End ctor 1 of PeaceKeeper' >> @>log;
    }

    ctor(@param: string)
        : cls1('The Beatles!'),
        cls2(12)
    {
        'Begin ctor 2 of PeaceKeeper' >> @>log;
        @param >> @>log;
        @b >> @>log;
        'End ctor 2 of PeaceKeeper' >> @>log;
    }

    private:
        @b = 3;
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin ctor 1 of cls0", message);
                            break;

                        case 2:
                            Assert.AreEqual("Cool!", message);
                            break;

                        case 3:
                            Assert.AreEqual("0", message);
                            break;

                        case 4:
                            Assert.AreEqual("End ctor 1 of cls0", message);
                            break;

                        case 5:
                            Assert.AreEqual("Begin ctor of cls1", message);
                            break;

                        case 6:
                            Assert.AreEqual("The Beatles!", message);
                            break;

                        case 7:
                            Assert.AreEqual("1", message);
                            break;

                        case 8:
                            Assert.AreEqual("End ctor of cls1", message);
                            break;

                        case 9:
                            Assert.AreEqual("Begin ctor 2 of cls0", message);
                            break;

                        case 10:
                            Assert.AreEqual("16", message);
                            break;

                        case 11:
                            Assert.AreEqual("0", message);
                            break;

                        case 12:
                            Assert.AreEqual("End ctor 2 of cls0", message);
                            break;

                        case 13:
                            Assert.AreEqual("Begin ctor of cls2", message);
                            break;

                        case 14:
                            Assert.AreEqual("12", message);
                            break;

                        case 15:
                            Assert.AreEqual("2", message);
                            break;

                        case 16:
                            Assert.AreEqual("End ctor of cls2", message);
                            break;

                        case 17:
                            Assert.AreEqual("Begin ctor 2 of PeaceKeeper", message);
                            break;

                        case 18:
                            Assert.AreEqual("Hi", message);
                            break;

                        case 19:
                            Assert.AreEqual("3", message);
                            break;

                        case 20:
                            Assert.AreEqual("End ctor 2 of PeaceKeeper", message);
                            break;

                        case 21:
                            Assert.AreEqual("Begin ctor 1 of PeaceKeeper", message);
                            break;

                        case 22:
                            Assert.AreEqual("3", message);
                            break;

                        case 23:
                            Assert.AreEqual("End ctor 1 of PeaceKeeper", message);
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
            var text = @"class cls0
{
    ctor(@param: string)
    {
        'Begin ctor 1 of cls0' >> @>log;
        @param >> @>log;
        @b >> @>log;
        'End ctor 1 of cls0' >> @>log;
    }

    ctor(@param: number)
    {
        'Begin ctor 2 of cls0' >> @>log;
        @param >> @>log;
        @b >> @>log;
        'End ctor 2 of cls0' >> @>log;
    }

    private:
        @b = 0;
}

class cls1 is cls0
{
    ctor(@param: string)
        : cls0('Cool!')
    {
        'Begin ctor of cls1' >> @>log;
        @param >> @>log;
        @b >> @>log;
        'End ctor of cls1' >> @>log;
    }

    private:
        @b = 1;
}

class cls2 is cls0
{
    ctor(@param: number)
        : cls0(16)
    {
        'Begin ctor of cls2' >> @>log;
        @param >> @>log;
        @b >> @>log;
        'End ctor of cls2' >> @>log;
    }

    private:
        @b = 2;
}

class cls3
{
    ctor()
    {
        'Begin ctor of cls3' >> @>log;
        @b >> @>log;
        'End ctor of cls3' >> @>log;
    }

    private:
        @b = 3;
}

app PeaceKeeper is cls1, cls2, cls3
{
    ctor()
        : ('Hi')
    {
        'Begin ctor 1 of PeaceKeeper' >> @>log;
        @b >> @>log;
        'End ctor 1 of PeaceKeeper' >> @>log;
    }

    ctor(@param: string)
        : cls1('The Beatles!'),
        cls2(12)
    {
        'Begin ctor 2 of PeaceKeeper' >> @>log;
        @param >> @>log;
        @b >> @>log;
        'End ctor 2 of PeaceKeeper' >> @>log;
    }

    private:
        @b = 4;
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin ctor 1 of cls0", message);
                            break;

                        case 2:
                            Assert.AreEqual("Cool!", message);
                            break;

                        case 3:
                            Assert.AreEqual("0", message);
                            break;

                        case 4:
                            Assert.AreEqual("End ctor 1 of cls0", message);
                            break;

                        case 5:
                            Assert.AreEqual("Begin ctor of cls1", message);
                            break;

                        case 6:
                            Assert.AreEqual("The Beatles!", message);
                            break;

                        case 7:
                            Assert.AreEqual("1", message);
                            break;

                        case 8:
                            Assert.AreEqual("End ctor of cls1", message);
                            break;

                        case 9:
                            Assert.AreEqual("Begin ctor 2 of cls0", message);
                            break;

                        case 10:
                            Assert.AreEqual("16", message);
                            break;

                        case 11:
                            Assert.AreEqual("0", message);
                            break;

                        case 12:
                            Assert.AreEqual("End ctor 2 of cls0", message);
                            break;

                        case 13:
                            Assert.AreEqual("Begin ctor of cls2", message);
                            break;

                        case 14:
                            Assert.AreEqual("12", message);
                            break;

                        case 15:
                            Assert.AreEqual("2", message);
                            break;

                        case 16:
                            Assert.AreEqual("End ctor of cls2", message);
                            break;

                        case 17:
                            Assert.AreEqual("Begin ctor of cls3", message);
                            break;

                        case 18:
                            Assert.AreEqual("3", message);
                            break;

                        case 19:
                            Assert.AreEqual("End ctor of cls3", message);
                            break;

                        case 20:
                            Assert.AreEqual("Begin ctor 2 of PeaceKeeper", message);
                            break;

                        case 21:
                            Assert.AreEqual("Hi", message);
                            break;

                        case 22:
                            Assert.AreEqual("4", message);
                            break;

                        case 23:
                            Assert.AreEqual("End ctor 2 of PeaceKeeper", message);
                            break;

                        case 24:
                            Assert.AreEqual("Begin ctor 1 of PeaceKeeper", message);
                            break;

                        case 25:
                            Assert.AreEqual("4", message);
                            break;

                        case 26:
                            Assert.AreEqual("End ctor 1 of PeaceKeeper", message);
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
            var text = @"class cls0
{
    ctor(@param: string)
    {
        'Begin ctor of cls0' >> @>log;
        @param >> @>log;
        @b >> @>log;
        'End ctor of cls0' >> @>log;
    }

    private:
        @b = 0;
}

class cls1 is cls0
{
    ctor(@param: string)
        : cls0('Cool!')
    {
        'Begin ctor of cls1' >> @>log;
        @param >> @>log;
        @b >> @>log;
        'End ctor of cls1' >> @>log;
    }

    private:
        @b = 1;
}

app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = new cls1('Hi!');
        @a.@b >> @>log;
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
                            Assert.AreEqual("Begin ctor of cls0", message);
                            break;

                        case 3:
                            Assert.AreEqual("Cool!", message);
                            break;

                        case 4:
                            Assert.AreEqual("0", message);
                            break;

                        case 5:
                            Assert.AreEqual("End ctor of cls0", message);
                            break;

                        case 6:
                            Assert.AreEqual("Begin ctor of cls1", message);
                            break;

                        case 7:
                            Assert.AreEqual("Hi!", message);
                            break;

                        case 8:
                            Assert.AreEqual("1", message);
                            break;

                        case 9:
                            Assert.AreEqual("End ctor of cls1", message);
                            break;

                        case 10:
                            Assert.AreEqual("1", message);
                            break;

                        case 11:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }
    }
}

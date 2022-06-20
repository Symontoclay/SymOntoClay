/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class If_else_elif_Statements_Tests
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
        
        @a = 1;

        if(@a)
        {
            'Yes!' >> @>log;
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
                            Assert.AreEqual(message, "Yes!");
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
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        
        @a = 0;

        if(@a)
        {
            'Yes!' >> @>log;
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
            var text = @"app PeaceKeeper
{
    {: >: { see(I, #`Barel 1`) } :}

    on Enter =>
    {
        'Begin' >> @>log;
        
        if({: >: { see(I, #`Barel 1`) } :})
        {
            'Yes!' >> @>log;
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
                            Assert.AreEqual(message, "Yes!");
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
        public void Case1_c()
        {
            var text = @"app PeaceKeeper
{
    {: >: { see(I, #`Barel 2`) } :}

    on Enter =>
    {
        'Begin' >> @>log;
        
        if({: >: { see(I, #`Barel 1`) } :})
        {
            'Yes!' >> @>log;
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
    {: >: { see(I, #`Barel 1`) } :}

    on Enter =>
    {
        'Begin' >> @>log;
        
        if({: >: { see(I, #`Barel 1`) } :})
        {
            'Yes!' >> @>log;
        } else {
            'Else Yes!' >> @>log;
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
                            Assert.AreEqual(message, "Yes!");
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
    {: >: { see(I, #`Barel 2`) } :}

    on Enter =>
    {
        'Begin' >> @>log;
        
        if({: >: { see(I, #`Barel 1`) } :})
        {
            'Yes!' >> @>log;
        } else {
            'Else Yes!' >> @>log;
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
                            Assert.AreEqual(message, "Else Yes!");
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
    {: >: { see(I, #`Barel 0`) } :}

    on Enter =>
    {
        'Begin' >> @>log;
        
        if({: >: { see(I, #`Barel 0`) } :})
        {
            'Yes!' >> @>log;
        } elif ({: >: { see(I, #`Barel 1`) } :}) {
            'Elif 1 Yes!' >> @>log;
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
                            Assert.AreEqual(message, "Yes!");
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
    {: >: { see(I, #`Barel 1`) } :}

    on Enter =>
    {
        'Begin' >> @>log;
        
        if({: >: { see(I, #`Barel 0`) } :})
        {
            'Yes!' >> @>log;
        } elif ({: >: { see(I, #`Barel 1`) } :}) {
            'Elif 1 Yes!' >> @>log;
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
                            Assert.AreEqual(message, "Elif 1 Yes!");
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
    {: >: { see(I, #`Barel 2`) } :}

    on Enter =>
    {
        'Begin' >> @>log;
        
        if({: >: { see(I, #`Barel 0`) } :})
        {
            'Yes!' >> @>log;
        } elif ({: >: { see(I, #`Barel 1`) } :}) {
            'Elif 1 Yes!' >> @>log;
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
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case3_c()
        {
            var text = @"app PeaceKeeper
{
    {: >: { see(I, #`Barel 2`) } :}

    on Enter =>
    {
        'Begin' >> @>log;
        
        if({: >: { see(I, #`Barel 0`) } :})
        {
            'Yes!' >> @>log;
        } elif ({: >: { see(I, #`Barel 1`) } :}) {
            'Elif 1 Yes!' >> @>log;
        }elif ({: >: { see(I, #`Barel 2`) } :}) {
            'Elif 2 Yes!' >> @>log;
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
                            Assert.AreEqual(message, "Elif 2 Yes!");
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
        public void Case3_d()
        {
            var text = @"app PeaceKeeper
{
    {: >: { see(I, #`Barel 3`) } :}

    on Enter =>
    {
        'Begin' >> @>log;
        
        if({: >: { see(I, #`Barel 0`) } :})
        {
            'Yes!' >> @>log;
        } elif ({: >: { see(I, #`Barel 1`) } :}) {
            'Elif 1 Yes!' >> @>log;
        }elif ({: >: { see(I, #`Barel 2`) } :}) {
            'Elif 2 Yes!' >> @>log;
        }else{
            'Else Yes!' >> @>log;
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
                            Assert.AreEqual(message, "Else Yes!");
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

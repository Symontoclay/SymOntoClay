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
using SymOntoClay.BaseTestLib.HostListeners;
using System;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class Exec_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	on Enter => {
	    exec {: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 :};
	}

	fun go(@direction)
	{
	    'go!!!!' >> @>log;
	    @direction >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("go!!!!", message);
                            return true;

                        case 2:
                            Assert.AreEqual(true, message.Contains("#@{:"));
                            Assert.AreEqual(true, message.Contains(">: { `color`($`_`,$`x1`) & `place`($`_`) & `green`($`x1`) } :}"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case1_a()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	on Enter => {
	    exec {: >: { direction($x1, #@(place & color = green)) & $x1 = go(someone,self) } o: 1 :};
	}

	fun go(@direction)
	{
	    'go!!!!' >> @>log;
	    @direction >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("go!!!!", message);
                            return true;

                        case 2:
                            Assert.AreEqual("#@(`place` & `color` = `green`)", message);
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
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	on Enter => {
	    exec {: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 :};
	}
}";

            var hostListener = new Exec_Tests_HostListener1();

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListenerAndImportStandardLibrary(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl_2", message);
                            return true;

                        case 2:
                            Assert.AreEqual("<0, 0, 0>", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            var text = @"synonym to for direction;

app PeaceKeeper
{
    import 'stdlib';

	on Enter => {
	    exec {: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 :};
	}
}
";

            var hostListener = new Exec_Tests_HostListener2();

            var maxN = 0;

            Assert.AreEqual(true,BehaviorTestEngineRunner.RunMinimalInstanceWithPlatformListenerAndImportStandardLibrary(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl", message);
                            return true;

                        case 2:
                            Assert.AreEqual("<0, 0, 0>", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, hostListener));

            Assert.AreEqual(2, maxN);
        }
    }
}

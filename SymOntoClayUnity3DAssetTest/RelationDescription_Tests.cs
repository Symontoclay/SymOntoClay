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
    public class RelationDescription_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"rel like ($x1 [:experiencer:], $x2 [:object:]) is state;
rel possess ($x1 [:owner:], $x2 [:possessions:]) is state;

app PeaceKeeper
{
	on Enter => {
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
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("End", message);
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
            var text = @"rel like ($x1 [:experiencer:]) is state, act;
rel possess ($x1 [:owner, object:], $x2 [:possessions:]) is state;

app PeaceKeeper
{
	on Enter => {
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
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("End", message);
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
    rel like ($x1 [:experiencer:], $x2 [:object:]) is state;
    rel possess ($x1 [:owner:], $x2 [:possessions:]) is state;

	on Enter => {
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
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            var text = @"app PeaceKeeper
{
    rel like ($x1 [:experiencer:]) is state, act;
    rel possess ($x1 [:owner, object:], $x2 [:possessions:]) is state;

	on Enter => {
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
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(2, maxN);
        }
    }
}

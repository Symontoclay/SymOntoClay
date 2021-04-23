using NUnit.Framework;
using SymOntoClay.Unity3DAsset.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test
{
    public class Inheritance_Tests
    {
        [Test]
        public void Case1()
        {
            var text = @"app PeaceKeeper is [0.5] exampleClass
{
    on Init =>
    {
        'Begin' >> @>log;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        use exampleClass is [0.5] human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        use exampleClass is not human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        use @@self is linux;
        @@self is linux >> @>log;
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
                            Assert.AreEqual(message, "0");
                            break;

                        case 7:
                            Assert.AreEqual(message, "1");
                            break;

                        case 8:
                            Assert.AreEqual(message, "1");
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
    on Init =>
    {
        'Begin' >> @>log;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        use exampleClass is [middle] human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        use exampleClass is not human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        use @@self is linux;
        @@self is linux >> @>log;
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
                            Assert.AreEqual(message, "0");
                            break;

                        case 7:
                            Assert.AreEqual(message, "1");
                            break;

                        case 8:
                            Assert.AreEqual(message, "1");
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
    on Init =>
    {
        'Begin' >> @>log;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        use exampleClass is [very middle] human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        use exampleClass is not human;
        exampleClass is human >> @>log;
        exampleClass is not human >> @>log;
        use @@self is linux;
        @@self is linux >> @>log;
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
                            Assert.AreEqual(message, "0");
                            break;

                        case 7:
                            Assert.AreEqual(message, "1");
                            break;

                        case 8:
                            Assert.AreEqual(message, "1");
                            break;

                        case 9:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }
    }
}

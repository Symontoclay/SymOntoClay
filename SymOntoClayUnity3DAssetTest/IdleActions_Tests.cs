using NUnit.Framework;
using SymOntoClay.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class IdleActions_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
    idle actions
    {
        go();
    }

    fun go()
    {
        'GO()' >> @>log;
        wait 10000;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GO()", message);
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
    idle actions
    {
        go()[: timeout=1000 :];
    }

    fun go()
    {
        'GO()' >> @>log;
        wait 10000;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    Assert.AreEqual("GO()", message);
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            var text = @"app PeaceKeeper
{
    idle actions
    {
        {: >: { direction($x1,#@(place & color = green)) & $x1 = go(someone,self) } o: 1 :};
    }

    fun go(@direction)
    {
        'GO()' >> @>log;
        @direction >> @>log;
        wait 10000;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GO()", message);
                            break;

                        case 2:
                            Assert.AreEqual("#@(place & color = green)", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }
    }
}

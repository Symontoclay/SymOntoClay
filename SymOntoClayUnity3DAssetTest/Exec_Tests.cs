using NUnit.Framework;
using SymOntoClay.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	on Enter => {
	    exec {: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 :};
	}

	fun go(@direction)
	{
	    'go!!!!' >> @>log;
	    @direction >> @>log;
	}
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "go!!!!");
                            break;

                        case 2:
                            Assert.AreEqual(message.Contains("#@{:"), true);
                            Assert.AreEqual(message.Contains(">: { color($_,$x1) & place($_) & green($x1) } :}"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }
    }
}

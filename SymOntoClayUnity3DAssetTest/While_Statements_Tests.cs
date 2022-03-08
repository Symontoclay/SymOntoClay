using NUnit.Framework;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
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
    on Init =>
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
    }
}

using NUnit.Framework;
using SymOntoClay.BaseTestLib;
using System;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class Concepts_Tests
    {
        [Test]
        [Parallelizable]
        public void Concept_Case1()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        
        ##`Begin` >> @>log;

        'End' >> @>log;
    }
}";

            throw new NotImplementedException();

            var maxN = 0;

            Assert.AreEqual(BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            return true;

                        case 2:
                            Assert.AreEqual(message, "`Begin`");
                            return true;

                        case 3:
                            Assert.AreEqual(message, "End");
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);

            Assert.AreEqual(3, maxN);
        }
    }
}

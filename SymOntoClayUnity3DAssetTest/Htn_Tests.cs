using NUnit.Framework;
using SymOntoClay.BaseTestLib;
using System;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class Htn_Tests
    {
        [Test]
        [Parallelizable]
        public void MinimalCase1()
        {
            var text = @"app PeaceKeeper
{
    root task `SomeCompoundTask`;

    fun SomeOperator()
    {
       'SomeOperator' >> @>log;
       wait 1;
    }
}

compound task SomeCompoundTask
{
   case
   {
       SomePrimitiveTask;
   }
}

primitive task SomePrimitiveTask
{
    operator SomeOperator();
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => 
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                        case 2:
                            Assert.AreEqual("SomeOperator", message);
                            return true;

                        case 3:
                            Assert.AreEqual("SomeOperator", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(3, maxN);
        }
    }
}

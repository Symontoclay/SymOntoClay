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
       'Run SomeOperator' >> @>log;
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
                            Assert.AreEqual("Run SomeOperator", message);
                            return true;

                        case 3:
                            Assert.AreEqual("Run SomeOperator", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void MinimalCase2()
        {
            var text = @"app PeaceKeeper
{
    root task `SomeTacticalTask`;

    fun SomeOperator()
    {
       'Run SomeOperator' >> @>log;
       wait 1;
    }
}

tactical task SomeTacticalTask
{
   case
   {
       SomeCompoundTask;
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
                            Assert.AreEqual("Run SomeOperator", message);
                            return true;

                        case 3:
                            Assert.AreEqual("Run SomeOperator", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void MinimalCase3()
        {
            var text = @"app PeaceKeeper
{
    root task `SomeStrategicTask`;

    fun SomeOperator()
    {
       'Run SomeOperator' >> @>log;
       wait 1;
    }
}

strategic task SomeStrategicTask
{
   case
   {
       SomeTacticalTask;
   }
}

tactical task SomeTacticalTask
{
   case
   {
       SomeCompoundTask;
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
                            Assert.AreEqual("Run SomeOperator", message);
                            return true;

                        case 3:
                            Assert.AreEqual("Run SomeOperator", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void MinimalCase4()
        {
            var text = @"app PeaceKeeper
{
    root task `SomeRootTask`;

    fun SomeOperator()
    {
       'Run SomeOperator' >> @>log;
       wait 1;
    }
}

root task SomeRootTask
{
   case
   {
       SomeStrategicTask;
   }
}

strategic task SomeStrategicTask
{
   case
   {
       SomeTacticalTask;
   }
}

tactical task SomeTacticalTask
{
   case
   {
       SomeCompoundTask;
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
                            Assert.AreEqual("Run SomeOperator", message);
                            return true;

                        case 3:
                            Assert.AreEqual("Run SomeOperator", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void MinimalCase5()
        {
            var text = @"app PeaceKeeper
{
    root task `SomeRootTask`;

    fun SomeOperator()
    {
       'Run SomeOperator' >> @>log;
       wait 1;
    }

    fun SomeOperator2()
    {
       'Run SomeOperator' >> @>log;
       wait 1;
    }
}

root task SomeRootTask
{
   case
   {
       SomeStrategicTask;
   }
}

root task SomeRootTask2
{
   case
   {
       SomeStrategicTask2;
   }
}

strategic task SomeStrategicTask
{
   case
   {
       SomeTacticalTask;
   }
}

strategic task SomeStrategicTask2
{
   case
   {
       SomeTacticalTask2;
   }
}

tactical task SomeTacticalTask
{
   case
   {
       SomeCompoundTask;
   }
}

tactical task SomeTacticalTask2
{
   case
   {
       SomeCompoundTask2;
   }
}

compound task SomeCompoundTask
{
   case
   {
       SomePrimitiveTask;
   }
}

compound task SomeCompoundTask2
{
   case
   {
       SomePrimitiveTask2;
   }
}

primitive task SomePrimitiveTask
{
    operator SomeOperator();
}

primitive task SomePrimitiveTask2
{
    operator SomeOperator2();
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
                            Assert.AreEqual("Run SomeOperator", message);
                            return true;

                        case 3:
                            Assert.AreEqual("Run SomeOperator", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void MinimalCase6()
        {
            var text = @"app PeaceKeeper
{
    fun SomeOperator()
    {
       'Run SomeOperator' >> @>log;
       wait 1;
    }
}

root task SomeRootTask
{
   case
   {
       SomeStrategicTask;
   }
}

strategic task SomeStrategicTask
{
   case
   {
       SomeTacticalTask;
   }
}

tactical task SomeTacticalTask
{
   case
   {
       SomeCompoundTask;
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
                            Assert.AreEqual("Run SomeOperator", message);
                            return true;

                        case 3:
                            Assert.AreEqual("Run SomeOperator", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(3, maxN);
        }
    }
}

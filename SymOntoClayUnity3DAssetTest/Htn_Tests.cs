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

        [Test]
        [Parallelizable]
        public void MinimalRecursionCase1()
        {
            var text = @"app PeaceKeeper
{
    fun SomeOperator()
    {
       'Run SomeOperator' >> @>log;
       wait 1;
    }

    fun SomeOperator2()
    {
       'Run SomeOperator2' >> @>log;
       wait 1;
    }

    fun SomeOperator3()
    {
       'Run SomeOperator3' >> @>log;
       wait 1;
    }

    fun SomeOperator4()
    {
       'Run SomeOperator4' >> @>log;
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
       SomeStrategicTask;
   }
}

tactical task SomeTacticalTask
{
   case
   {
       SomeCompoundTask;
       SomeCompoundTask2;
   }
}

compound task SomeCompoundTask
{
   case
   {
       SomePrimitiveTask;
       SomePrimitiveTask2;
   }
}

compound task SomeCompoundTask2
{
   case
   {
       SomePrimitiveTask3;
       SomePrimitiveTask4;
   }
}

primitive task SomePrimitiveTask
{
    operator SomeOperator();
}

primitive task SomePrimitiveTask2
{
    operator SomeOperator2();
}

primitive task SomePrimitiveTask3
{
    operator SomeOperator3();
}

primitive task SomePrimitiveTask4
{
    operator SomeOperator4();
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Run SomeOperator", message);
                            return true;

                        case 2:
                            Assert.AreEqual("Run SomeOperator2", message);
                            return true;

                        case 3:
                            Assert.AreEqual("Run SomeOperator3", message);
                            return true;

                        case 4:
                            Assert.AreEqual("Run SomeOperator4", message);
                            return true;

                        case 5:
                            Assert.AreEqual("Run SomeOperator", message);
                            return true;

                        case 6:
                            Assert.AreEqual("Run SomeOperator2", message);
                            return true;

                        case 7:
                            Assert.AreEqual("Run SomeOperator3", message);
                            return true;

                        case 8:
                            Assert.AreEqual("Run SomeOperator4", message);
                            return true;

                        case 9:
                            Assert.AreEqual("Run SomeOperator", message);
                            return true;

                        case 10:
                            Assert.AreEqual("Run SomeOperator2", message);
                            return true;

                        case 11:
                            Assert.AreEqual("Run SomeOperator3", message);
                            return true;

                        case 12:
                            Assert.AreEqual("Run SomeOperator4", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(12, maxN);
        }

        [Test]
        [Parallelizable]
        public void AutoPropertyCase1()
        {
            var text = @"app PeaceKeeper
{
    root task `DestroyEnemy`;

    fun ChooseEnemyOperator()
    {
       'Run ChooseEnemyOperator' >> @>log;
       wait 1;
    }

    fun NavToEnemyOperator(@target)
    {
       'Run NavToEnemyOperator' >> @>log;
       @target >> @>log;
       wait 1;
    }

    fun KillEnemyOperator(@target)
    {
       'Run KillEnemyOperator' >> @>log;
       @target >> @>log;
       wait 1;
    }

    prop TargetEnemy: number = 22;
}

compound task DestroyEnemy
{
   case
   {
       ChooseEnemy;
       NavToEnemy;
       KillEnemy;
   }
}

primitive task ChooseEnemy
{
    operator ChooseEnemyOperator();
}

primitive task NavToEnemy
{
    operator NavToEnemyOperator(TargetEnemy);
}

primitive task KillEnemy
{
    operator KillEnemyOperator(TargetEnemy);
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Run ChooseEnemyOperator", message);
                            return true;

                        case 2:
                            Assert.AreEqual("Run NavToEnemyOperator", message);
                            return true;

                        case 3:
                            Assert.AreEqual("22", message);
                            return true;

                        case 4:
                            Assert.AreEqual("Run KillEnemyOperator", message);
                            return true;

                        case 5:
                            Assert.AreEqual("22", message);
                            return true;

                        case 6:
                            Assert.AreEqual("Run ChooseEnemyOperator", message);
                            return true;

                        case 7:
                            Assert.AreEqual("Run NavToEnemyOperator", message);
                            return true;

                        case 8:
                            Assert.AreEqual("22", message);
                            return true;

                        case 9:
                            Assert.AreEqual("Run KillEnemyOperator", message);
                            return true;

                        case 10:
                            Assert.AreEqual("22", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(10, maxN);
        }

        [Test]
        [Parallelizable]
        public void ReadOnlyPropertyCase1()
        {
            var text = @"app PeaceKeeper
{
    root task `DestroyEnemy`;

    fun ChooseEnemyOperator()
    {
       'Run ChooseEnemyOperator' >> @>log;
       @targetEnemy = 16;
       wait 1;
    }

    fun NavToEnemyOperator(@target)
    {
       'Run NavToEnemyOperator' >> @>log;
       @target >> @>log;
       wait 1;
    }

    fun KillEnemyOperator(@target)
    {
       'Run KillEnemyOperator' >> @>log;
       @target >> @>log;
       wait 1;
    }

    prop TargetEnemy => @targetEnemy;

	private:
	    var @targetEnemy;
}

compound task DestroyEnemy
{
   case
   {
       ChooseEnemy;
       NavToEnemy;
       KillEnemy;
   }
}

primitive task ChooseEnemy
{
    operator ChooseEnemyOperator();
}

primitive task NavToEnemy
{
    operator NavToEnemyOperator(TargetEnemy);
}

primitive task KillEnemy
{
    operator KillEnemyOperator(TargetEnemy);
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Run ChooseEnemyOperator", message);
                            return true;

                        case 2:
                            Assert.AreEqual("Run NavToEnemyOperator", message);
                            return true;

                        case 3:
                            Assert.AreEqual("16", message);
                            return true;

                        case 4:
                            Assert.AreEqual("Run KillEnemyOperator", message);
                            return true;

                        case 5:
                            Assert.AreEqual("16", message);
                            return true;

                        case 6:
                            Assert.AreEqual("Run ChooseEnemyOperator", message);
                            return true;

                        case 7:
                            Assert.AreEqual("Run NavToEnemyOperator", message);
                            return true;

                        case 8:
                            Assert.AreEqual("16", message);
                            return true;

                        case 9:
                            Assert.AreEqual("Run KillEnemyOperator", message);
                            return true;

                        case 10:
                            Assert.AreEqual("16", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(10, maxN);
        }

        [Test]
        [Parallelizable]
        public void CrossFunctionsProperty_Case1()
        {
            var text = @"app PeaceKeeper
{
    root task `DestroyEnemy`;

    fun ChooseEnemyOperator()
    {
       'Run ChooseEnemyOperator' >> @>log;
       @targetEnemy = 16;
       'Before SomeUnknownProp:' >> @>log;
       SomeUnknownProp >> @>log;
       SomeUnknownProp = 22;
       'After SomeUnknownProp:' >> @>log;
       SomeUnknownProp >> @>log;
       wait 1;
    }

    fun NavToEnemyOperator(@target)
    {
       'Run NavToEnemyOperator' >> @>log;
       @target >> @>log;
       'SomeUnknownProp:' >> @>log;
       SomeUnknownProp >> @>log;
       wait 1;
    }

    fun KillEnemyOperator(@target)
    {
       'Run KillEnemyOperator' >> @>log;
       @target >> @>log;
       'SomeUnknownProp:' >> @>log;
       SomeUnknownProp >> @>log;
       wait 1;
    }

    prop TargetEnemy => @targetEnemy;

	private:
	    var @targetEnemy;
}

compound task DestroyEnemy
{
   case
   {
       ChooseEnemy;
       NavToEnemy;
       KillEnemy;
   }
}

primitive task ChooseEnemy
{
    operator ChooseEnemyOperator();
}

primitive task NavToEnemy
{
    operator NavToEnemyOperator(TargetEnemy);
}

primitive task KillEnemy
{
    operator KillEnemyOperator(TargetEnemy);
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Run ChooseEnemyOperator", message);
                            return true;

                        case 2:
                            Assert.AreEqual("Before SomeUnknownProp:", message);
                            return true;

                        case 3:
                            Assert.AreEqual("someunknownprop", message);
                            return true;

                        case 4:
                            Assert.AreEqual("After SomeUnknownProp:", message);
                            return true;

                        case 5:
                            Assert.AreEqual("22", message);
                            return true;

                        case 6:
                            Assert.AreEqual("Run NavToEnemyOperator", message);
                            return true;

                        case 7:
                            Assert.AreEqual("16", message);
                            return true;

                        case 8:
                            Assert.AreEqual("SomeUnknownProp:", message);
                            return true;

                        case 9:
                            Assert.AreEqual("22", message);
                            return true;

                        case 10:
                            Assert.AreEqual("Run KillEnemyOperator", message);
                            return true;

                        case 11:
                            Assert.AreEqual("16", message);
                            return true;

                        case 12:
                            Assert.AreEqual("SomeUnknownProp:", message);
                            return true;

                        case 13:
                            Assert.AreEqual("22", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(13, maxN);
        }

        [Test]
        [Parallelizable]
        public void CompoundHtnTaskCaseCondition_Case1()
        {
            var text = @"app PeaceKeeper
{
    root task `SomeCompoundTask`;

    fun SomeOperator()
    {
       'Run SomeOperator' >> @>log;
       wait 1;
    }

    fun SomeOperator2()
    {
       'Run SomeOperator2' >> @>log;
       wait 1;
    }

    fun SomeOperator3()
    {
       'Run SomeOperator3' >> @>log;
       wait 1;
    }

    prop SomeAutoProp: number = 16;
}

compound task SomeCompoundTask
{
   case SomeAutoProp is 16
   {
       SomePrimitiveTask;
   }

   case SomeAutoProp > 16
   {
       SomePrimitiveTask2;
   }

   case 
   {
       SomePrimitiveTask3;
   }
}

primitive task SomePrimitiveTask
{
    operator SomeOperator();
}

primitive task SomePrimitiveTask2
{
    operator SomeOperator2();
}

primitive task SomePrimitiveTask3
{
    operator SomeOperator3();
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Run SomeOperator", message);
                            return true;

                        case 2:
                            Assert.AreEqual("Run SomeOperator", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void CompoundHtnTaskCaseCondition_Case1_a()
        {
            var text = @"app PeaceKeeper
{
    root task `SomeCompoundTask`;

    fun SomeOperator()
    {
       'Run SomeOperator' >> @>log;
       wait 1;
    }

    fun SomeOperator2()
    {
       'Run SomeOperator2' >> @>log;
       wait 1;
    }

    fun SomeOperator3()
    {
       'Run SomeOperator3' >> @>log;
       wait 1;
    }

    prop SomeAutoProp: number = 16;
}

compound task SomeCompoundTask
{
   case 
   {
       SomePrimitiveTask3;
   }

   case SomeAutoProp is 16
   {
       SomePrimitiveTask;
   }

   case SomeAutoProp > 16
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
}

primitive task SomePrimitiveTask3
{
    operator SomeOperator3();
}
";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Run SomeOperator", message);
                            return true;

                        case 2:
                            Assert.AreEqual("Run SomeOperator", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void CompoundHtnTaskCaseCondition_Case2()
        {
            var text = @"app PeaceKeeper
{
    root task `SomeCompoundTask`;

    fun SomeOperator()
    {
       'Run SomeOperator' >> @>log;
       wait 1;
    }

    fun SomeOperator2()
    {
       'Run SomeOperator2' >> @>log;
       wait 1;
    }

    fun SomeOperator3()
    {
       'Run SomeOperator3' >> @>log;
       wait 1;
    }

    prop SomeAutoProp: number = 16;
    prop OtherAutoProp: number = 17;
}

compound task SomeCompoundTask
{
   case SomeAutoProp is 16
   {
       SomePrimitiveTask;
   }

   case OtherAutoProp > 16
   {
       SomePrimitiveTask2;
   }

   case 
   {
       SomePrimitiveTask3;
   }
}

primitive task SomePrimitiveTask
{
    operator SomeOperator();
}

primitive task SomePrimitiveTask2
{
    operator SomeOperator2();
}

primitive task SomePrimitiveTask3
{
    operator SomeOperator3();
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Run SomeOperator", message);
                            return true;

                        case 2:
                            Assert.AreEqual("Run SomeOperator", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void PrimitiveHtnTaskPrecondition_Case1()
        {
            var text = @"app PeaceKeeper
{
    root task `SomeCompoundTask`;

    fun SomeOperator()
    {
       'Run SomeOperator' >> @>log;
       wait 1;
    }

    fun SomeOperator2()
    {
       'Run SomeOperator2' >> @>log;
       wait 1;
    }

    fun SomeOperator3()
    {
       'Run SomeOperator3' >> @>log;
       wait 1;
    }

    prop SomeAutoProp: number = 16;
    prop OtherAutoProp: number = 17;
    prop ThirdAutoProp: number = 12;
}

compound task SomeCompoundTask
{
   case SomeAutoProp is 16
   {
       SomePrimitiveTask;
   }

   case OtherAutoProp > 16
   {
       SomePrimitiveTask2;
   }

   case 
   {
       SomePrimitiveTask3;
   }
}

primitive task SomePrimitiveTask
{
    preconditions ThirdAutoProp is 12;
    operator SomeOperator();
}

primitive task SomePrimitiveTask2
{
    operator SomeOperator2();
}

primitive task SomePrimitiveTask3
{
    operator SomeOperator3();
}
";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Run SomeOperator", message);
                            return true;

                        case 2:
                            Assert.AreEqual("Run SomeOperator", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void PrimitiveHtnTaskPrecondition_Case2()
        {
            var text = @"app PeaceKeeper
{
    root task `SomeCompoundTask`;

    fun SomeOperator()
    {
       'Run SomeOperator' >> @>log;
       wait 1;
    }

    fun SomeOperator2()
    {
       'Run SomeOperator2' >> @>log;
       wait 1;
    }

    fun SomeOperator3()
    {
       'Run SomeOperator3' >> @>log;
       wait 1;
    }

    prop SomeAutoProp: number = 16;
    prop OtherAutoProp: number = 17;
    prop ThirdAutoProp: number = 12;
}

compound task SomeCompoundTask
{
   case SomeAutoProp is 16
   {
       SomePrimitiveTask;
   }

   case OtherAutoProp > 16
   {
       SomePrimitiveTask2;
   }

   case 
   {
       SomePrimitiveTask3;
   }
}

primitive task SomePrimitiveTask
{
    preconditions ThirdAutoProp is 15;
    operator SomeOperator();
}

primitive task SomePrimitiveTask2
{
    operator SomeOperator2();
}

primitive task SomePrimitiveTask3
{
    operator SomeOperator3();
}
";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Run SomeOperator2", message);
                            return true;

                        case 2:
                            Assert.AreEqual("Run SomeOperator2", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(2, maxN);
        }
    }
}

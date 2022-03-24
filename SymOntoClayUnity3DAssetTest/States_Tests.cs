using NUnit.Framework;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class States_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
    set Idling as default state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }
}

state Idling
{
    on Enter
    {
        'Begin Enter' >> @>log;
        'End Enter' >> @>log;
    }
}";

            var initN = 0;
            var enterN = 0;

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (message) => {
                    if(message.EndsWith(" Enter"))
                    {
                        enterN++;

                        switch (enterN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin Enter");
                                break;

                            case 2:
                                Assert.AreEqual(message, "End Enter");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(enterN), enterN, null);
                        }
                    }
                    else
                    {
                        initN++;

                        switch (initN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin");
                                break;

                            case 2:
                                Assert.AreEqual(message, "End");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(initN), initN, null);
                        }
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var text = @"app PeaceKeeper
{
    set Idling as state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }
}

state Idling
{
    on Enter
    {
        'Begin Enter' >> @>log;
        'End Enter' >> @>log;
    }
}";

            var initN = 0;
            var enterN = 0;

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (message) => {
                    if (message.EndsWith(" Enter"))
                    {
                        enterN++;

                        switch (enterN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin Enter");
                                break;

                            case 2:
                                Assert.AreEqual(message, "End Enter");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(enterN), enterN, null);
                        }
                    }
                    else
                    {
                        initN++;

                        switch (initN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin");
                                break;

                            case 2:
                                Assert.AreEqual(message, "End");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(initN), initN, null);
                        }
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            var text = @"app PeaceKeeper
{
    set Idling as default state;
    set Patrolling as state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }
}

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        'End Idling Enter' >> @>log;
    }
}

state Patrolling
{
    on Enter
    {
        'Begin Patrolling Enter' >> @>log;
        'End Patrolling Enter' >> @>log;
    }
}";

            var initN = 0;
            var enterN = 0;

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (message) => {
                    if (message.EndsWith(" Enter"))
                    {
                        enterN++;

                        switch (enterN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin Patrolling Enter");
                                break;

                            case 2:
                                Assert.AreEqual(message, "End Patrolling Enter");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(enterN), enterN, null);
                        }
                    }
                    else
                    {
                        initN++;

                        switch (initN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin");
                                break;

                            case 2:
                                Assert.AreEqual(message, "End");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(initN), initN, null);
                        }
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case4()
        {
            var text = @"app PeaceKeeper
{
    set Idling as default state;
    set Patrolling as state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }
}

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        'End Idling Enter' >> @>log;
    }
}

state Patrolling
{
    on Enter
    {
        'Begin Patrolling Enter' >> @>log;

        set Idling as state;

        'End Patrolling Enter' >> @>log;
    }
}";

            var initN = 0;
            var patrollingN = 0;
            var idlingN = 0;

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (message) => {
                    if (message.Contains(" Patrolling "))
                    {
                        patrollingN++;

                        switch (patrollingN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin Patrolling Enter");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(patrollingN), patrollingN, null);
                        }
                    }
                    else
                    {
                        if(message.Contains(" Idling "))
                        {
                            idlingN++;

                            switch (idlingN)
                            {
                                case 1:
                                    Assert.AreEqual(message, "Begin Idling Enter");
                                    break;

                                case 2:
                                    Assert.AreEqual(message, "End Idling Enter");
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(idlingN), idlingN, null);
                            }
                        }
                        else
                        {
                            initN++;

                            switch (initN)
                            {
                                case 1:
                                    Assert.AreEqual(message, "Begin");
                                    break;

                                case 2:
                                    Assert.AreEqual(message, "End");
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(initN), initN, null);
                            }
                        }
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case5()
        {
            var text = @"app PeaceKeeper
{
    set Idling as default state;
    set Patrolling as state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }
}

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        'End Idling Enter' >> @>log;
    }
}

state Patrolling
{
    on Enter
    {
        'Begin Patrolling Enter' >> @>log;

        complete state;

        'End Patrolling Enter' >> @>log;
    }
}";

            var initN = 0;
            var patrollingN = 0;
            var idlingN = 0;

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (message) => {
                    if (message.Contains(" Patrolling "))
                    {
                        patrollingN++;

                        switch (patrollingN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin Patrolling Enter");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(patrollingN), patrollingN, null);
                        }
                    }
                    else
                    {
                        if (message.Contains(" Idling "))
                        {
                            idlingN++;

                            switch (idlingN)
                            {
                                case 1:
                                    Assert.AreEqual(message, "Begin Idling Enter");
                                    break;

                                case 2:
                                    Assert.AreEqual(message, "End Idling Enter");
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(idlingN), idlingN, null);
                            }
                        }
                        else
                        {
                            initN++;

                            switch (initN)
                            {
                                case 1:
                                    Assert.AreEqual(message, "Begin");
                                    break;

                                case 2:
                                    Assert.AreEqual(message, "End");
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(initN), initN, null);
                            }
                        }
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case5_a()
        {
            var text = @"app PeaceKeeper
{
    set Patrolling as state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }
}

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        'End Idling Enter' >> @>log;
    }
}

state Patrolling
{
    on Enter
    {
        'Begin Patrolling Enter' >> @>log;

        complete state;

        'End Patrolling Enter' >> @>log;
    }
}";

            var initN = 0;
            var enterN = 0;

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (message) => {
                    if (message.EndsWith(" Enter"))
                    {
                        enterN++;

                        switch (enterN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin Patrolling Enter");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(enterN), enterN, null);
                        }
                    }
                    else
                    {
                        initN++;

                        switch (initN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin");
                                break;

                            case 2:
                                Assert.AreEqual(message, "End");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(initN), initN, null);
                        }
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case6()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }
}

state Attacking
{
    enter on:
        {: see(I, enemy) :}

    on Enter
    {
        'Begin Attacking Enter' >> @>log;

        'End Attacking Enter' >> @>log;
    }
}";

            instance.WriteFile(text);

            var initN = 0;
            var enterN = 0;

            var npc = instance.CreateAndStartNPC((n, message) => {
                if (message.EndsWith(" Enter"))
                {
                    enterN++;

                    switch (enterN)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin Attacking Enter");
                            break;

                        case 2:
                            Assert.AreEqual(message, "End Attacking Enter");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(enterN), enterN, null);
                    }
                }
                else
                {
                    initN++;

                    switch (initN)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(initN), initN, null);
                    }
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, enemy) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case7()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    set Idling as default state;
    set Patrolling as state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }
}

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        'End Idling Enter' >> @>log;
    }
}

state Patrolling
{
    on Enter
    {
        'Begin Patrolling Enter' >> @>log;
        'End Patrolling Enter' >> @>log;
    }
}

state Attacking
{
    enter on:
        {: see(I, enemy) :}

    leave on:
        {: see(I, barrel) :}

    on Enter
    {
        'Begin Attacking Enter' >> @>log;

        'End Attacking Enter' >> @>log;
    }
}";

            instance.WriteFile(text);

            var initN = 0;
            var patrollingN = 0;
            var idlingN = 0;
            var attackingN = 0;

            var warInit = false;
            var wasPatrolling = false;
            var wasAttacking = false;

            var npc = instance.CreateAndStartNPC((n, message) => {
                if(message.Contains(" Idling "))
                {
                    idlingN++;

                    switch (idlingN)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin Idling Enter");
                            Assert.AreEqual(wasAttacking, true);
                            break;

                        case 2:
                            Assert.AreEqual(message, "End Idling Enter");                            
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(idlingN), idlingN, null);
                    }
                }
                else
                {
                    if (message.Contains(" Patrolling "))
                    {
                        patrollingN++;

                        switch (patrollingN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin Patrolling Enter");
                                break;

                            case 2:
                                Assert.AreEqual(message, "End Patrolling Enter");
                                wasPatrolling = true;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(patrollingN), patrollingN, null);
                        }
                    }
                    else
                    {
                        if (message.Contains(" Attacking "))
                        {
                            attackingN++;

                            switch (attackingN)
                            {
                                case 1:
                                    Assert.AreEqual(message, "Begin Attacking Enter");
                                    Assert.AreEqual(warInit, true);
                                    Assert.AreEqual(wasPatrolling, true);
                                    break;

                                case 2:
                                    Assert.AreEqual(message, "End Attacking Enter");
                                    wasAttacking = true;
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(attackingN), attackingN, null);
                            }
                        }
                        else
                        {
                            initN++;

                            switch (initN)
                            {
                                case 1:
                                    Assert.AreEqual(message, "Begin");
                                    break;

                                case 2:
                                    Assert.AreEqual(message, "End");
                                    warInit = true;
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(initN), initN, null);
                            }
                        }
                    }
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, enemy) :}");

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, barrel) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case8()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            var text = @"app PeaceKeeper
{
    set Idling as default state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }
}

states { Idling, Attacking }

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        'End Idling Enter' >> @>log;
    }
}

state Attacking
{
    enter on:
        {: see(I, enemy) :}

    leave on:
        {: see(I, barrel) :}

    on Enter
    {
        'Begin Attacking Enter' >> @>log;

        'End Attacking Enter' >> @>log;
    }
}";

            instance.WriteFile(text);

            var initN = 0;
            var idlingN = 0;
            var attackingN = 0;

            var warInit = false;
            var wasAttaking = false;

            var npc = instance.CreateAndStartNPC((n, message) => {
                if (message.Contains(" Idling "))
                {
                    idlingN++;

                    switch (idlingN)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin Idling Enter");
                            break;

                        case 2:
                            Assert.AreEqual(message, "End Idling Enter");
                            break;

                        case 3:
                            Assert.AreEqual(message, "Begin Idling Enter");
                            Assert.AreEqual(wasAttaking, true);
                            break;

                        case 4:
                            Assert.AreEqual(message, "End Idling Enter");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(idlingN), idlingN, null);
                    }
                }
                else
                {
                    if (message.Contains(" Attacking "))
                    {
                        attackingN++;

                        switch (attackingN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin Attacking Enter");
                                Assert.AreEqual(warInit, true);
                                break;

                            case 2:
                                Assert.AreEqual(message, "End Attacking Enter");
                                wasAttaking = true;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(attackingN), attackingN, null);
                        }
                    }
                    else
                    {
                        initN++;

                        switch (initN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin");
                                break;

                            case 2:
                                Assert.AreEqual(message, "End");
                                warInit = true;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(initN), initN, null);
                        }
                    }
                }
            });

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, enemy) :}");

            Thread.Sleep(1000);

            npc.InsertFact("{: see(I, barrel) :}");

            Thread.Sleep(1000);
        }

        [Test]
        [Parallelizable]
        public void Case9()
        {
            var text = @"app PeaceKeeper
{
    set Idling as default state;
}

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        ? {: bird ($x) :} >> @>log;
        insert {: >: { bird (#1234) } :};
        ? {: bird ($x) :} >> @>log;
        'End Idling Enter' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin Idling Enter");
                            break;

                        case 2:
                            Assert.AreEqual(message, "<no>");
                            break;

                        case 3:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            Assert.AreEqual(message.Contains("$x = #1234"), true);
                            break;

                        case 4:
                            Assert.AreEqual(message, "End Idling Enter");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case10()
        {
            var text = @"app PeaceKeeper
{
    set Idling as default state;
    set Patrolling as state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }

    on {: attack(I, enemy) :}
    {
        'D' >> @>log;
    }
}

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        'End Idling Enter' >> @>log;
    }
}

state Patrolling
{
    on Enter
    {
        'Begin Patrolling Enter' >> @>log;

        break state {: attack(I, enemy) :};

        'End Patrolling Enter' >> @>log;
    }
}";

            var initN = 0;
            var idlingN = 0;
            var patrollingN = 0;
            var triggerN = 0;

            var wasPatrolling = false;

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (message) => {
                    if(message.Contains(" Idling "))
                    {
                        idlingN++;

                        switch (idlingN)
                        {
                            case 1:
                                Assert.AreEqual(message, "Begin Idling Enter");
                                Assert.AreEqual(wasPatrolling, true);
                                break;

                            case 2:
                                Assert.AreEqual(message, "End Idling Enter");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(idlingN), idlingN, null);
                        }
                    }
                    else
                    {
                        if (message.Contains(" Patrolling "))
                        {
                            patrollingN++;

                            switch (patrollingN)
                            {
                                case 1:
                                    Assert.AreEqual(message, "Begin Patrolling Enter");
                                    wasPatrolling = true;
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(patrollingN), patrollingN, null);
                            }
                        }
                        else
                        {
                            if (message.Contains("D"))
                            {
                                triggerN++;

                                switch (triggerN)
                                {
                                    case 1:
                                        Assert.AreEqual(message, "D");
                                        Assert.AreEqual(wasPatrolling, true);
                                        break;

                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(triggerN), triggerN, null);
                                }
                            }
                            else
                            {
                                initN++;

                                switch (initN)
                                {
                                    case 1:
                                        Assert.AreEqual(message, "Begin");
                                        break;

                                    case 2:
                                        Assert.AreEqual(message, "End");
                                        break;

                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(initN), initN, null);
                                }
                            }
                        }
                    }
                }), true);
        }
    }
}

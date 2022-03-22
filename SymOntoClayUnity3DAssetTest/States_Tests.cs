using NUnit.Framework;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}

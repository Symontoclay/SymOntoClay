using NUnit.Framework;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class Functions_Tests
    {
        [Test]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a();
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case2_a()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1) => 
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case2_b()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1) => 
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a('Hi');
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "Hi");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case2_c()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1) => 
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(dog);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "dog");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case2_c_1()//concept identifier
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1) => 
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(`dog`);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "dog");
                            break;

                        case 4:
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
            var text = @"app PeaceKeeper
{
    fun a(@param_1) => 
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(@param_1: 1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case4()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1) => 
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case5()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1) => 
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case6()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1) => 
    {
        '`a` (1) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1, @param_2) => 
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (1) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case7()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1) => 
    {
        '`a` (1) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1, @param_2) => 
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        a(3, 'Hi');
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (1) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "`a` (2) has been called!");
                            break;

                        case 5:
                            Assert.AreEqual(message, "3");
                            break;

                        case 6:
                            Assert.AreEqual(message, "Hi");
                            break;

                        case 7:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case8_a()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1, @param_2 = 'Hi') => 
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (2) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "Hi");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case8_b()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1, @param_2 = 15) => 
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (2) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "15");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case8_c()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1, @param_2 = dog) => 
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (2) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "dog");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case8_c_1()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1, @param_2 = `dog`) => 
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` (2) has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "12");
                            break;

                        case 4:
                            Assert.AreEqual(message, "dog");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case8_d()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1 = 2) => 
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case8_e()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1, @param_2 = 42) => 
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "42");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case9_a_1()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1) => 
    {
        '`a` (any) has been called!' >> @>log;
    }

    fun a(@param_1: string) => 
    {
        '`a` (string) has been called!' >> @>log;
    }

    fun a(@param_1: number) => 
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }
    }
}

﻿using NUnit.Framework;
using SymOntoClay.Unity3DAsset.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test
{
    public class LogicQueries_Tests
    {
        [Test]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
	{: male(#Tom) :}
	{: parent(#Piter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Init => {
	    select {: son($x, $y) :} >> @>log;
	}
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            Assert.AreEqual(message.Contains("$y = #piter"), true);
                            Assert.AreEqual(message.Contains("$x = #tom"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case2()
        {
            var text = @"app PeaceKeeper
{
	{: male(#Tom) :}
	{: parent(#Piter, #Tom) :}
	
	on Init => {
	    select {: son($x, $y) :} >> @>log;
	}
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<no>"), true);
                            Assert.AreEqual(message.Contains("<yes>"), false);
                            Assert.AreEqual(message.Contains("$y = #piter"), false);
                            Assert.AreEqual(message.Contains("$x = #tom"), false);
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
	{: male(#Tom) :}
	{: parent(#Piter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Init => {
	    select {: son(#Tom, $y) :} >> @>log;
	}
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            Assert.AreEqual(message.Contains("$y = #piter"), true);
                            Assert.AreEqual(message.Contains("$x = #tom"), false);
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
	{: male(#Tom) :}
	{: parent(#Piter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Init => {
	    ? {: son(#Tom, $y) :} >> @>log;
	}
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            Assert.AreEqual(message.Contains("$y = #piter"), true);
                            Assert.AreEqual(message.Contains("$x = #tom"), false);
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
    {: can(bird, fly) :}
    {: bird(#Alisa_12) :}

    on Init =>
    {
        ? {: can(#Alisa_12, $x) :} >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            Assert.AreEqual(message.Contains("$x = fly"), true);
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
        {: can(bird, fly) :}
        {: bird(#Alisa_12) :}

    on Init =>
    {
        ? {: can(#Alisa_12, fly) :} >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<yes>"), true);
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
    {: can(bird, fly) :}
    {: bird(#Alisa_12) :}

    on Init =>
    {
        ? {: $z(#Alisa_12, $x) :} >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            Assert.AreEqual(message.Contains("$z = can(bird,fly)"), true);
                            Assert.AreEqual(message.Contains("$x"), false);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case8()
        {
            var text = @"app PeaceKeeper
{
    {: >: { see(I, #`Barel 1`) } :}

    on Init =>
    {
        ? {: see(I, #`Barel 1`) :} >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case9()
        {
            var text = @"app PeaceKeeper
{
    {: barrel(#a) :}
    {: see(I, #a) :}

    on Init =>
    {
        select {: see(I, barrel) :} >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case9_a()
        {
            var text = @"app PeaceKeeper
{
    {: is (#a, barrel) :}
    {: see(I, #a) :}

    on Init =>
    {
        select {: see(I, barrel) :} >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case10()
        {
            var text = @"app PeaceKeeper
{
    {: animal(cat) :}

    on Init =>
    {
        select {: { cat is animal } :} >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case10_a()
        {
            var text = @"app PeaceKeeper
{
    {: { animal(cat) } :}

    on Init =>
    {
        select {: { cat is animal } :} >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case10_b()
        {
            var text = @"app PeaceKeeper
{
    {: animal(cat) :}

    on Init =>
    {
        select {: cat is animal :} >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case10_c()
        {
            var text = @"app PeaceKeeper
{
    {: animal(cat) :}

    on Init =>
    {
        select {: >: { cat is animal } :} >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        public void Case11()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
    }
}";

            throw new NotImplementedException();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        /*
        select {: $z($x, $y) :} >> @>log;
*/

        [Test]
        public void Case12()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
    }
}";

            throw new NotImplementedException();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        /*
        select {: age(#Tom, `teenager`) :} >> @>log;
*/

        [Test]
        public void Case13()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
    }
}";

            throw new NotImplementedException();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        /*
        select {: age(#Tom, $x) & distance(#Tom, $y) & $x is not $y :} >> @>log;
*/

        [Test]
        public void Case14()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
    }
}";

            throw new NotImplementedException();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        /*
        select {: distance(I, #Tom, $x) :} >> @>log;
*/

        [Test]
        public void Case15()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
    }
}";

            throw new NotImplementedException();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        /*
        select {: distance(#Tom, $x) & $x is 12 :} >> @>log;
*/

        [Test]
        public void Case16()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
    }
}";

            throw new NotImplementedException();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        /*
        select {: distance(#Tom, $x) & $x > 5 :} >> @>log;
*/

        [Test]
        public void Case17()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
    }
}";

            throw new NotImplementedException();

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        /*
        insert {: >: { bird (#1234) } :};
*/
    }
}

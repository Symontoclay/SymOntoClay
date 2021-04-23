using NUnit.Framework;
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

        //[Test]
        //public void Case5()
        //{
        //    var text = @"";

        //    Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
        //        (n, message) => {
        //            switch (n)
        //            {
        //                default:
        //                    throw new ArgumentOutOfRangeException(nameof(n), n, null);
        //            }
        //        }), true);
        //}

        /*
{: can(#Alisa_12, ?x) :} 
*/

        //[Test]
        //public void Case6()
        //{
        //    var text = @"";

        //    Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
        //        (n, message) => {
        //            switch (n)
        //            {
        //                default:
        //                    throw new ArgumentOutOfRangeException(nameof(n), n, null);
        //            }
        //        }), true);
        //}

        /*
{: can(#Alisa_12, fly) :} 
*/

        //[Test]
        //public void Case7()
        //{
        //    var text = @"";

        //    Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
        //        (n, message) => {
        //            switch (n)
        //            {
        //                default:
        //                    throw new ArgumentOutOfRangeException(nameof(n), n, null);
        //            }
        //        }), true);
        //}

        //[Test]
        //public void Case8()
        //{
        //    var text = @"";

        //    Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
        //        (n, message) => {
        //            switch (n)
        //            {
        //                default:
        //                    throw new ArgumentOutOfRangeException(nameof(n), n, null);
        //            }
        //        }), true);
        //}

        //[Test]
        //public void Case9()
        //{
        //    var text = @"";

        //    Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
        //        (n, message) => {
        //            switch (n)
        //            {
        //                default:
        //                    throw new ArgumentOutOfRangeException(nameof(n), n, null);
        //            }
        //        }), true);
        //}

        //[Test]
        //public void Case10()
        //{
        //    var text = @"";

        //    Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
        //        (n, message) => {
        //            switch (n)
        //            {
        //                default:
        //                    throw new ArgumentOutOfRangeException(nameof(n), n, null);
        //            }
        //        }), true);
        //}

        //[Test]
        //public void Case11()
        //{
        //    var text = @"";

        //    Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
        //        (n, message) => {
        //            switch (n)
        //            {
        //                default:
        //                    throw new ArgumentOutOfRangeException(nameof(n), n, null);
        //            }
        //        }), true);
        //}





        /*
        {: ?z(#Alisa_12, ?x) :} 
        */

        /*
        {: see(I, #`Barel 1`) :}
*/

        /*
        select {: see(I, barrel) :} >> @>log;
*/

        /*
        select {: { cat is animal } :} >> @>log;
*/

        /*
        select {: $z($x, $y) :} >> @>log;
*/

        /*
        select {: age(#Tom, `teenager`) :} >> @>log;
*/

        /*
        select {: age(#Tom, $x) & distance(#Tom, $y) & $x is not $y :} >> @>log;
*/

        /*
        select {: distance(I, #Tom, $x) :} >> @>log;
*/

        /*
        select {: distance(#Tom, $x) & $x is 12 :} >> @>log;
*/

        /*
        select {: distance(#Tom, $x) & $x > 5 :} >> @>log;
*/

        /*
        insert {: >: { bird (#1234) } :};
*/
    }
}

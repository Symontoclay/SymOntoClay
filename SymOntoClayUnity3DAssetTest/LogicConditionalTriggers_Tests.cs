using NUnit.Framework;
using SymOntoClay.Unity3DAsset.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test
{
    public class LogicConditionalTriggers_Tests
    {
        [Test]
        public void Case1()
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
	on {: see(I, #`gun 1`) :} => {
	     'D' >> @>log;
	} 
*/

        [Test]
        public void Case2()
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
//	//on {: see(I, $x) & barrel($x) & !focus(I, friend) :} ($x >> @x) => {
//	//     @x >> @>log;
//	//} 
*/

        [Test]
        public void Case3()
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
//	//on {: see(I, $x) & barrel($x) & !focus(I, friend) :} => {
//	//     @x >> @>log;
//	//} 
*/

//        [Test]
//        public void Case4()
//        {
//            var text = @"app PeaceKeeper
//{
//    on Init =>
//    {
//    }
//}";

//            throw new NotImplementedException();

//            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
//                (n, message) =>
//                {
//                    switch (n)
//                    {
//                        default:
//                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
//                    }
//                }), true);
//        }

        /*
 
*/

//        [Test]
//        public void Case5()
//        {
//            var text = @"app PeaceKeeper
//{
//    on Init =>
//    {
//    }
//}";

//            throw new NotImplementedException();

//            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
//                (n, message) =>
//                {
//                    switch (n)
//                    {
//                        default:
//                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
//                    }
//                }), true);
//        }

        /*
 
*/
    }
}

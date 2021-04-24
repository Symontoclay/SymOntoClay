using NUnit.Framework;
using SymOntoClay.Unity3DAsset.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test
{
    public class General_Tests
    {
        [Test]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        'End' >> @>log;
        }
    }
";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "End");
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
    on Init =>
    {
        'Begin' >> @>log;
        //@r = @b = 1;
        @bx >> @>log;
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
                            Assert.AreEqual(message, "NULL");
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
        public void Case3()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        /*@r = @b = 1;
        @bx >> @>log;*/
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
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
    on Init =>
    {
        'Begin' >> @>log;
        @r = @b = 1;
        @b >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "1");
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
        public void Case5()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        @r = @b = 1;
        @bx >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "NULL");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

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
        //    var text = @"app PeaceKeeper
//{
//    on Init =>
//    {
//    }
//}";

        //    throw new NotImplementedException();

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
app PeaceKeeper
{
    on Init =>
    {
    }
}
         */
    }
}

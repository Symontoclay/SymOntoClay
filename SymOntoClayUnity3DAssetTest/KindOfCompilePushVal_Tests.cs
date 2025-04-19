using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using NUnit.Framework;
using SymOntoClay.Core.Internal.Compiling.Internal;
using SymOntoClay.Core.Internal.Compiling.Internal.Helpers;
using System;
using System.Linq;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class KindOfCompilePushVal_Tests
    {
        [Test]
        [Parallelizable]
        public void Trivial_Single_Case1()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.DirectVar;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.DirectVar));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Single_Case2()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.SetVar;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.SetVar));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Single_Case3()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.GetVar;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.GetVar));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Single_Case4()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.DirectProp;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.DirectProp));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Single_Case5()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.SetProp;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.SetProp));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Single_Case6()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.GetProp;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.GetProp));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Success_Case1()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.DirectVar | KindOfCompilePushVal.DirectProp;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.DirectVar));
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.DirectProp));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Success_Case2()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.DirectVar | KindOfCompilePushVal.SetProp;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.DirectVar));
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.SetProp));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Success_Case3()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.DirectVar | KindOfCompilePushVal.GetProp;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.DirectVar));
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.GetProp));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Success_Case4()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.SetVar | KindOfCompilePushVal.DirectProp;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.SetVar));
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.DirectProp));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Success_Case5()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.SetVar | KindOfCompilePushVal.SetProp;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.SetVar));
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.SetProp));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Success_Case6()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.SetVar | KindOfCompilePushVal.GetProp;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.SetVar));
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.GetProp));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Success_Case7()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.GetVar | KindOfCompilePushVal.DirectProp;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.GetVar));
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.DirectProp));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Success_Case8()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.GetVar | KindOfCompilePushVal.SetProp;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.GetVar));
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.SetProp));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Success_Case9()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.GetVar | KindOfCompilePushVal.GetProp;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.GetVar));
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.GetProp));
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Fail_Case1()
        {
            var e = Assert.Throws<Exception>(() => {
                var kindOfCompilePushVal = KindOfCompilePushVal.DirectVar | KindOfCompilePushVal.GetVar;

                KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);
            });

            Assert.AreEqual("The options DirectVar, GetVar can not be used together.", e.Message);
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Fail_Case2()
        {
            var e = Assert.Throws<Exception>(() => {
                var kindOfCompilePushVal = KindOfCompilePushVal.DirectVar | KindOfCompilePushVal.SetVar;

                KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);
            });

            Assert.AreEqual("The options DirectVar, SetVar can not be used together.", e.Message);
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Fail_Case3()
        {
            var e = Assert.Throws<Exception>(() => {
                var kindOfCompilePushVal = KindOfCompilePushVal.GetVar | KindOfCompilePushVal.SetVar;

                KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);
            });

            Assert.AreEqual("The options SetVar, GetVar can not be used together.", e.Message);
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Fail_Case4()
        {
            var e = Assert.Throws<Exception>(() => {
                var kindOfCompilePushVal = KindOfCompilePushVal.DirectProp | KindOfCompilePushVal.SetProp;

                KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);
            });

            Assert.AreEqual("The options DirectProp, SetProp can not be used together.", e.Message);
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Fail_Case5()
        {
            var e = Assert.Throws<Exception>(() => {
                var kindOfCompilePushVal = KindOfCompilePushVal.DirectProp | KindOfCompilePushVal.GetProp;

                KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);
            });

            Assert.AreEqual("The options DirectProp, GetProp can not be used together.", e.Message);
        }

        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Fail_Case6()
        {
            var e = Assert.Throws<Exception>(() => {
                var kindOfCompilePushVal = KindOfCompilePushVal.SetProp | KindOfCompilePushVal.GetProp;

                KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);
            });

            Assert.AreEqual("The options SetProp, GetProp can not be used together.", e.Message);
        }

        [Test]
        [Parallelizable]
        public void AllCases_Single_Case1()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.DirectAllCases;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.DirectVar));
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.DirectProp));
        }

        [Test]
        [Parallelizable]
        public void AllCases_Single_Case2()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.SetAllCases;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.SetVar));
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.SetProp));
        }

        [Test]
        [Parallelizable]
        public void AllCases_Single_Case3()
        {
            var kindOfCompilePushVal = KindOfCompilePushVal.GetAllCases;

            var result = KindOfCompilePushValHelper.ConvertToInternalItems(kindOfCompilePushVal);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.GetVar));
            Assert.AreEqual(true, result.Contains(InternalKindOfCompilePushVal.GetProp));
        }

        /*
        [Test]
        [Parallelizable]
        public void AllCases_Single_Case()
        {
            
        }*/
    }
}

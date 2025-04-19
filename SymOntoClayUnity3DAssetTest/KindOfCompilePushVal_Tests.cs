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

        /*
        [Test]
        [Parallelizable]
        public void Trivial_Multiple_Success_Case()
        {
            throw new NotImplementedException();
        }*/
    }
}

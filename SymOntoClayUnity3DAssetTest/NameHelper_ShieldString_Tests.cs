using NUnit.Framework;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class NameHelper_ShieldString_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            Assert.AreEqual(string.Empty, NameHelper.ShieldString(string.Empty));
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            Assert.AreEqual("dog", NameHelper.ShieldString("dog"));
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            Assert.AreEqual("`dog`", NameHelper.ShieldString("`dog`"));
        }

        [Test]
        [Parallelizable]
        public void Case2_b()
        {
            Assert.AreEqual("`yellow dog`", NameHelper.ShieldString("yellow dog"));
        }

        [Test]
        [Parallelizable]
        public void Case2_c()
        {
            Assert.AreEqual("`yellow-dog`", NameHelper.ShieldString("yellow-dog"));
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            Assert.AreEqual("@>log", NameHelper.ShieldString("@>log"));
        }

        [Test]
        [Parallelizable]
        public void Case3_a()
        {
            Assert.AreEqual("@>`log`", NameHelper.ShieldString("@>`log`"));
        }

        [Test]
        [Parallelizable]
        public void Case3_b()
        {
            Assert.AreEqual("@>`example log`", NameHelper.ShieldString("@>example log"));
        }

        [Test]
        [Parallelizable]
        public void Case3_c()
        {
            Assert.AreEqual("@>`example-log`", NameHelper.ShieldString("@>example-log"));
        }

        [Test]
        [Parallelizable]
        public void Case4()
        {
            Assert.AreEqual("@@self", NameHelper.ShieldString("@@self"));
        }

        [Test]
        [Parallelizable]
        public void Case4_a()
        {
            Assert.AreEqual("@@`self`", NameHelper.ShieldString("@@`self`"));
        }

        [Test]
        [Parallelizable]
        public void Case4_b()
        {
            Assert.AreEqual("@@`example var`", NameHelper.ShieldString("@@example var"));
        }

        [Test]
        [Parallelizable]
        public void Case4_c()
        {
            Assert.AreEqual("@@`example-var`", NameHelper.ShieldString("@@example-var"));
        }

        [Test]
        [Parallelizable]
        public void Case5()
        {
            Assert.AreEqual("#@", NameHelper.ShieldString("#@"));
        }

        [Test]
        [Parallelizable]
        public void Case6()
        {
            Assert.AreEqual("#@example", NameHelper.ShieldString("#@example"));
        }

        [Test]
        [Parallelizable]
        public void Case6_a()
        {
            Assert.AreEqual("#@`example`", NameHelper.ShieldString("#@`example`"));
        }

        [Test]
        [Parallelizable]
        public void Case6_b()
        {
            Assert.AreEqual("#@`long example`", NameHelper.ShieldString("#@long example"));
        }

        [Test]
        [Parallelizable]
        public void Case6_c()
        {
            Assert.AreEqual("#@`long-example`", NameHelper.ShieldString("#@long-example"));
        }

        [Test]
        [Parallelizable]
        public void Case7()
        {
            Assert.AreEqual("##example", NameHelper.ShieldString("##example"));
        }

        [Test]
        [Parallelizable]
        public void Case7_a()
        {
            Assert.AreEqual("##`example`", NameHelper.ShieldString("##`example`"));
        }

        [Test]
        [Parallelizable]
        public void Case7_b()
        {
            Assert.AreEqual("##`long example`", NameHelper.ShieldString("##long example"));
        }

        [Test]
        [Parallelizable]
        public void Case7_c()
        {
            Assert.AreEqual("##`long-example`", NameHelper.ShieldString("##long-example"));
        }

        [Test]
        [Parallelizable]
        public void Case8()
        {
            Assert.AreEqual("#^example", NameHelper.ShieldString("#^example"));
        }

        [Test]
        [Parallelizable]
        public void Case8_a()
        {
            Assert.AreEqual("#^`example`", NameHelper.ShieldString("#^`example`"));
        }

        [Test]
        [Parallelizable]
        public void Case8_b()
        {
            Assert.AreEqual("#^`long example`", NameHelper.ShieldString("#^long example"));
        }

        [Test]
        [Parallelizable]
        public void Case8_c()
        {
            Assert.AreEqual("#^`long-example`", NameHelper.ShieldString("#^long-example"));
        }

        [Test]
        [Parallelizable]
        public void Case9()
        {
            Assert.AreEqual("@example", NameHelper.ShieldString("@example"));
        }

        [Test]
        [Parallelizable]
        public void Case9_a()
        {
            Assert.AreEqual("@`example`", NameHelper.ShieldString("@`example`"));
        }

        [Test]
        [Parallelizable]
        public void Case9_b()
        {
            Assert.AreEqual("@`long example`", NameHelper.ShieldString("@long example"));
        }

        [Test]
        [Parallelizable]
        public void Case9_c()
        {
            Assert.AreEqual("@`long-example`", NameHelper.ShieldString("@long-example"));
        }

        [Test]
        [Parallelizable]
        public void Case10()
        {
            Assert.AreEqual("#example", NameHelper.ShieldString("#example"));
        }

        [Test]
        [Parallelizable]
        public void Case10_a()
        {
            Assert.AreEqual("#`example`", NameHelper.ShieldString("#`example`"));
        }

        [Test]
        [Parallelizable]
        public void Case10_b()
        {
            Assert.AreEqual("#`long example`", NameHelper.ShieldString("#long example"));
        }

        [Test]
        [Parallelizable]
        public void Case10_c()
        {
            Assert.AreEqual("#`long-example`", NameHelper.ShieldString("#long-example"));
        }

        [Test]
        [Parallelizable]
        public void Case11()
        {
            Assert.AreEqual("$example", NameHelper.ShieldString("$example"));
        }

        [Test]
        [Parallelizable]
        public void Case11_a()
        {
            Assert.AreEqual("$`example`", NameHelper.ShieldString("$`example`"));
        }

        [Test]
        [Parallelizable]
        public void Case11_b()
        {
            Assert.AreEqual("$`long example`", NameHelper.ShieldString("$long example"));
        }

        [Test]
        [Parallelizable]
        public void Case11_c()
        {
            Assert.AreEqual("$`long-example`", NameHelper.ShieldString("$long-example"));
        }
    }
}

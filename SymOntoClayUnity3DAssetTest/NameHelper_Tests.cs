/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using NUnit.Framework;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class NameHelper_Tests
    {
        [SetUp]
        public void Setup()
        {
            _mainStorageContext = new UnityTestMainStorageContext();
        }

        private IMainStorageContext _mainStorageContext;

        [Test]
        [Parallelizable]
        public void NameHelper_Tests_Case_Empty()
        {
            var name = new StrongIdentifierValue();

            Assert.AreEqual(name.IsEmpty, true);
            Assert.AreEqual(name.NameValue, string.Empty);
            Assert.AreEqual(name.NormalizedNameValue, string.Empty);
            Assert.AreEqual(name.KindOfName, KindOfName.Unknown);

            Assert.AreEqual(name.KindOfValue, KindOfValue.StrongIdentifierValue);
            Assert.AreEqual(name.IsStrongIdentifierValue, true);
            Assert.AreEqual(name.AsStrongIdentifierValue, name);
        }

        [Test]
        [Parallelizable]
        public void NameHelper_Tests_Case_2_Empty()
        {
            var name = NameHelper.CreateName(string.Empty);

            Assert.AreEqual(name.IsEmpty, true);
            Assert.AreEqual(name.NameValue, string.Empty);
            Assert.AreEqual(name.NormalizedNameValue, string.Empty);
            Assert.AreEqual(name.KindOfName, KindOfName.Unknown);

            Assert.AreEqual(name.KindOfValue, KindOfValue.StrongIdentifierValue);
            Assert.AreEqual(name.IsStrongIdentifierValue, true);
            Assert.AreEqual(name.AsStrongIdentifierValue, name);
        }

        [Test]
        [Parallelizable]
        public void NameHelper_Tests_Case_Concept()
        {
            var text = "dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, text);
            Assert.AreEqual(name.NormalizedNameValue, text);
            Assert.AreEqual(name.KindOfName, KindOfName.Concept);
        }

        [Test]
        [Parallelizable]
        public void NameHelper_Tests_Case_2_Concept()
        {
            var name = NameHelper.CreateName("Dog");

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, "dog");
            Assert.AreEqual(name.NormalizedNameValue, "dog");
            Assert.AreEqual(name.KindOfName, KindOfName.Concept);
        }

        [Test]
        [Parallelizable]
        public void NameHelper_Tests_Case_3_Concept()
        {
            var text = "small dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, "`small dog`");
            Assert.AreEqual(name.NormalizedNameValue, text);
            Assert.AreEqual(name.KindOfName, KindOfName.Concept);
        }

        [Test]
        [Parallelizable]
        public void NameHelper_Tests_Case_Channel()
        {
            var text = "@>log";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, text);
            Assert.AreEqual(name.NormalizedNameValue, text);
            Assert.AreEqual(name.KindOfName, KindOfName.Channel);
        }

        [Test]
        [Parallelizable]
        public void NameHelper_Tests_Case_CreateRuleOrFactName()
        {
            var name = NameHelper.CreateRuleOrFactName();

            Assert.AreEqual(true, name.NameValue.StartsWith("#^"));
            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreNotEqual(name.NameValue, string.Empty);
            Assert.AreNotEqual(name.NormalizedNameValue, string.Empty);
            Assert.AreEqual(name.KindOfName, KindOfName.RuleOrFact);
        }

        [Test]
        [Parallelizable]
        public void NameHelper_Tests_Case_Entity()
        {
            var text = "#dog1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, text);
            Assert.AreEqual(name.NormalizedNameValue, text);
            Assert.AreEqual(name.KindOfName, KindOfName.Entity);
        }

        [Test]
        [Parallelizable]
        public void NameHelper_Tests_Case_2_Entity()
        {
            var text = "#Tom";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, "#tom");
            Assert.AreEqual(name.NormalizedNameValue, "#tom");
            Assert.AreEqual(name.KindOfName, KindOfName.Entity);
        }

        [Test]
        [Parallelizable]
        public void NameHelper_Tests_Case_3_Entity()
        {
            var text = "#`Barrel 1`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, "#`barrel 1`");
            Assert.AreEqual(name.NormalizedNameValue, "#barrel 1");
            Assert.AreEqual(name.KindOfName, KindOfName.Entity);
        }

        [Test]
        [Parallelizable]
        public void NameHelper_Tests_Case_Var()
        {
            var text = "@a";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, "@a");
            Assert.AreEqual(name.NormalizedNameValue, "@a");
            Assert.AreEqual(name.KindOfName, KindOfName.Var);
        }

        [Test]
        [Parallelizable]
        public void NameHelper_Tests_Case_SystemVar()
        {
            var text = "@@host";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, "@@host");
            Assert.AreEqual(name.NormalizedNameValue, "@@host");
            Assert.AreEqual(name.KindOfName, KindOfName.SystemVar);
        }

        [Test]
        [Parallelizable]
        public void NameHelper_Tests_Case_LogicVar()
        {
            var text = "$x";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, "$x");
            Assert.AreEqual(name.NormalizedNameValue, "$x");
            Assert.AreEqual(name.KindOfName, KindOfName.LogicalVar);
        }   
    }
}

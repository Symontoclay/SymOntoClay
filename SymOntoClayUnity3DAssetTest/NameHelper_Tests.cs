/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using SymOntoClay.Unity3DAsset.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test
{
    public class NameHelper_Tests
    {
        [SetUp]
        public void Setup()
        {
            _mainStorageContext = new TstMainStorageContext();
        }

        private IMainStorageContext _mainStorageContext;

        [Test]
        public void NameHelper_Tests_Case_Empty()
        {
            var name = new StrongIdentifierValue();

            Assert.AreEqual(name.IsEmpty, true);
            Assert.AreEqual(name.NameValue, string.Empty);
            Assert.AreEqual(name.KindOfName, KindOfName.Unknown);
            //Assert.AreEqual(name.NameKey, 0);
        }

        [Test]
        public void NameHelper_Tests_Case_Concept()
        {
            var text = "dog";

            var name = NameHelper.CreateName(text, _mainStorageContext.Dictionary);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, text);
            Assert.AreEqual(name.KindOfName, KindOfName.Concept);
            //Assert.AreNotEqual(name.NameKey, 0);

            //var key = _parserContext.Dictionary.GetKey(text);

            //Assert.AreEqual(name.NameKey, key);
        }

        [Test]
        public void NameHelper_Tests_Case_Channel()
        {
            var text = "@>log";

            var name = NameHelper.CreateName(text, _mainStorageContext.Dictionary);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, text);
            Assert.AreEqual(name.KindOfName, KindOfName.Channel);
            //Assert.AreNotEqual(name.NameKey, 0);

            //var key = _parserContext.Dictionary.GetKey(text);

            //Assert.AreEqual(name.NameKey, key);
        }

        [Test]
        public void NameHelper_Tests_Case_CreateRuleOrFactName()
        {
            var name = NameHelper.CreateRuleOrFactName(_mainStorageContext.Dictionary);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreNotEqual(name.NameValue, string.Empty);
            Assert.AreEqual(name.KindOfName, KindOfName.Entity);
            //Assert.AreNotEqual(name.NameKey, 0);
        }

        [Test]
        public void NameHelper_Tests_Case_Entity()
        {
            var text = "#dog1";

            var name = NameHelper.CreateName(text, _mainStorageContext.Dictionary);

            Assert.AreEqual(name.IsEmpty, false);
            Assert.AreEqual(name.NameValue, text);
            Assert.AreEqual(name.KindOfName, KindOfName.Entity);
            //Assert.AreNotEqual(name.NameKey, 0);

            //var key = _parserContext.Dictionary.GetKey(text);

            //Assert.AreEqual(name.NameKey, key);
        }
    }
}

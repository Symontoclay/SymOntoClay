/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

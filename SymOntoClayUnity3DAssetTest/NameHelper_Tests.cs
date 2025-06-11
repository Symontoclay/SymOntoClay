/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class NameHelper_Tests
    {
        [Test]
        [Parallelizable]
        public void Empty_Case1()
        {
            var name = new StrongIdentifierValue();

            Assert.AreEqual(true, name.IsEmpty);
            Assert.AreEqual(string.Empty, name.NameValue);
            Assert.AreEqual(string.Empty, name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.Unknown, name.KindOfName);

            Assert.AreEqual(KindOfValue.StrongIdentifierValue, name.KindOfValue);
            Assert.AreEqual(true, name.IsStrongIdentifierValue);
            Assert.AreEqual(name, name.AsStrongIdentifierValue);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(false, name.Capacity.HasValue);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void Empty_Case2()
        {
            var name = NameHelper.CreateName(string.Empty);

            Assert.AreEqual(true, name.IsEmpty);
            Assert.AreEqual(string.Empty, name.NameValue);
            Assert.AreEqual(string.Empty, name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.Unknown, name.KindOfName);

            Assert.AreEqual(KindOfValue.StrongIdentifierValue, name.KindOfValue);
            Assert.AreEqual(true, name.IsStrongIdentifierValue);
            Assert.AreEqual(name, name.AsStrongIdentifierValue);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(false, name.Capacity.HasValue);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void CommonConcept_Case1()
        {
            var text = "dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("`dog`", name.NameValue);
            Assert.AreEqual(text, name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.CommonConcept, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void CommonConcept_Case2()
        {
            var name = NameHelper.CreateName("Dog");

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("`Dog`", name.NameValue);
            Assert.AreEqual("dog", name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.CommonConcept, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void CommonConcept_Case3()
        {
            var text = "small dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("`small dog`", name.NameValue);
            Assert.AreEqual(text, name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.CommonConcept, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void CommonConcept_Case4()
        {
            var text = "__ctor";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("`__ctor`", name.NameValue);
            Assert.AreEqual("__ctor", name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.CommonConcept, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void Channel_Case1()
        {
            var text = "@>log";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("@>`log`", name.NameValue);
            Assert.AreEqual(text, name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.Channel, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void Channel_Case1_a()
        {
            var text = "@>`log`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("@>`log`", name.NameValue);
            Assert.AreEqual("@>log", name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.Channel, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void Channel_Case1_b()
        {
            var text = "@>`big log`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("@>`big log`", name.NameValue);
            Assert.AreEqual("@>big log", name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.Channel, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void CreateRuleOrFactName_Case1_b()
        {
            var name = NameHelper.CreateRuleOrFactName();

            Assert.AreEqual(true, name.NameValue.StartsWith("#^"));
            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreNotEqual(string.Empty, name.NameValue);
            Assert.AreNotEqual(string.Empty, name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.RuleOrFact, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void RuleOrFactName_Case2()
        {
            var text = "#^91e029e7-6a4c-454b-b15b-323d2b5ff0a9";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("#^`91e029e7-6a4c-454b-b15b-323d2b5ff0a9`", name.NameValue);
            Assert.AreEqual("#^91e029e7-6a4c-454b-b15b-323d2b5ff0a9", name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.RuleOrFact, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void RuleOrFactName_Case2_a()
        {
            var text = "#^`91e029e7-6a4c-454b-b15b-323d2b5ff0a9`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("#^`91e029e7-6a4c-454b-b15b-323d2b5ff0a9`", name.NameValue);
            Assert.AreEqual("#^91e029e7-6a4c-454b-b15b-323d2b5ff0a9", name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.RuleOrFact, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void Entity_Case1()
        {
            var text = "#dog1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("#`dog1`", name.NameValue);
            Assert.AreEqual(text, name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.Entity, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void Entity_Case2()
        {
            var text = "#Tom";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("#`Tom`", name.NameValue);
            Assert.AreEqual("#tom", name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.Entity, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void Entity_Case3()
        {
            var text = "#`Barrel 1`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "#`Barrel 1`");
            Assert.AreEqual(,name.NormalizedNameValue, "#barrel 1");
            Assert.AreEqual(,name.KindOfName, KindOfName.Entity);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Entity_Case4()
        {
            var text = "#020ED339-6313-459A-900D-92F809CEBDC5";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "#`020ED339-6313-459A-900D-92F809CEBDC5`");
            Assert.AreEqual(,name.NormalizedNameValue, "#020ed339-6313-459a-900d-92f809cebdc5");
            Assert.AreEqual(,name.KindOfName, KindOfName.Entity);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Entity_Case4_a()
        {
            var text = "#`020ED339-6313-459A-900D-92F809CEBDC5`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "#`020ED339-6313-459A-900D-92F809CEBDC5`");
            Assert.AreEqual(,name.NormalizedNameValue, "#020ed339-6313-459a-900d-92f809cebdc5");
            Assert.AreEqual(,name.KindOfName, KindOfName.Entity);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Var_Case1()
        {
            var text = "@a";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "@`a`");
            Assert.AreEqual(,name.NormalizedNameValue, "@a");
            Assert.AreEqual(,name.KindOfName, KindOfName.Var);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void SystemVar_Case1()
        {
            var text = "@@host";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "@@`host`");
            Assert.AreEqual(,name.NormalizedNameValue, "@@host");
            Assert.AreEqual(,name.KindOfName, KindOfName.SystemVar);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void SystemVar_Case1_a()
        {
            var text = "@@`host`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "@@`host`");
            Assert.AreEqual(,name.NormalizedNameValue, "@@host");
            Assert.AreEqual(,name.KindOfName, KindOfName.SystemVar);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void SystemVar_Case1_b()
        {
            var text = "@@`big host`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "@@`big host`");
            Assert.AreEqual(,name.NormalizedNameValue, "@@big host");
            Assert.AreEqual(,name.KindOfName, KindOfName.SystemVar);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void LogicVar_Case1()
        {
            var text = "$x";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("$`x`", name.NameValue);
            Assert.AreEqual("$x", name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.LogicalVar, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void LogicVar_Case2()
        {
            var text = "$_";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "$`_`");
            Assert.AreEqual(,name.NormalizedNameValue, "$_");
            Assert.AreEqual(,name.KindOfName, KindOfName.LogicalVar);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void LogicVar_Case2_a()
        {
            var text = "$`_`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "$`_`");
            Assert.AreEqual(,name.NormalizedNameValue, "$_");
            Assert.AreEqual(,name.KindOfName, KindOfName.LogicalVar);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void LinguisticVar_Case1()
        {
            var text = "#|`teenager`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "#|`teenager`");
            Assert.AreEqual(,name.NormalizedNameValue, "#|teenager");
            Assert.AreEqual(,name.KindOfName, KindOfName.LinguisticVar);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void LinguisticVar_Case1_a()
        {
            var text = "#|teenager";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "#|`teenager`");
            Assert.AreEqual(,name.NormalizedNameValue, "#|teenager");
            Assert.AreEqual(,name.KindOfName, KindOfName.LinguisticVar);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void LinguisticVar_Case1_b()
        {
            var text = "#|`big teenager`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "#|`big teenager`");
            Assert.AreEqual(,name.NormalizedNameValue, "#|big teenager");
            Assert.AreEqual(,name.KindOfName, KindOfName.LinguisticVar);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Concept_Case1()
        {
            var text = "##dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "##`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, text);
            Assert.AreEqual(,name.KindOfName, KindOfName.Concept);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Concept_Case2()
        {
            var text = "##`dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "##`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "##dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.Concept);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Concept_Case3()
        {
            var text = "##`small dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "##`small dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "##small dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.Concept);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Property_Case1()
        {
            var text = "@:Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "@:`Prop1`");
            Assert.AreEqual(,name.NormalizedNameValue, "@:prop1");
            Assert.AreEqual(,name.KindOfName, KindOfName.Property);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Property_Case2()
        {
            var text = "@:`small dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "@:`small dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "@:small dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.Property);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void EntityCondition_Case1()
        {
            var text = "#@";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "#@");
            Assert.AreEqual(,name.NormalizedNameValue, "#@");
            Assert.AreEqual(,name.KindOfName, KindOfName.AnonymousEntityCondition);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void EntityCondition_Case2()
        {
            var text = "#@`dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("#@`dog`", name.NameValue);
            Assert.AreEqual("#@dog", name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.EntityCondition, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void EntityCondition_Case2_a()
        {
            var text = "#@dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "#@`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "#@dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.EntityCondition);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void EntityCondition_Case3()
        {
            var text = "#@`small dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "#@`small dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "#@small dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.EntityCondition);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void OnceResolvedEntityCondition_Case1()
        {
            var text = "##@";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "##@");
            Assert.AreEqual(,name.NormalizedNameValue, "##@");
            Assert.AreEqual(,name.KindOfName, KindOfName.OnceResolvedAnonymousEntityCondition);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void OnceResolvedEntityCondition_Case2()
        {
            var text = "##@`dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "##@`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "##@dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.OnceResolvedEntityCondition);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void OnceResolvedEntityCondition_Case2_a()
        {
            var text = "##@dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "##@`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "##@dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.OnceResolvedEntityCondition);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void OnceResolvedEntityCondition_Case3()
        {
            var text = "##@`small dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "##@`small dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "##@small dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.OnceResolvedEntityCondition);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Level_Case1()
        {
            var text = "global::Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "`Prop1`");
            Assert.AreEqual(,name.NormalizedNameValue, "prop1");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.Global);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Level_Case2()
        {
            var text = "global::@:Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "@:`Prop1`");
            Assert.AreEqual(,name.NormalizedNameValue, "@:prop1");
            Assert.AreEqual(,name.KindOfName, KindOfName.Property);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.Global);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Level_Case3()
        {
            var text = "global::##Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "##`Prop1`");
            Assert.AreEqual(,name.NormalizedNameValue, "##prop1");
            Assert.AreEqual(,name.KindOfName, KindOfName.Concept);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.Global);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Level_Case4()
        {
            var text = "root::Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "`Prop1`");
            Assert.AreEqual(,name.NormalizedNameValue, "prop1");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.Root);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Level_Case5()
        {
            var text = "strategic::Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "`Prop1`");
            Assert.AreEqual(,name.NormalizedNameValue, "prop1");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.Strategic);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Level_Case6()
        {
            var text = "tactical::Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("`Prop1`", name.NameValue);
            Assert.AreEqual("prop1", name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.CommonConcept, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.Tactical, name.Level);

            Assert.AreEqual(0, name.Namespaces.Count);
        }

        [Test]
        [Parallelizable]
        public void Array_Case1()
        {
            var text = "number[5]";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "`number`");
            Assert.AreEqual(,name.NormalizedNameValue, "number");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(true, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 5);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Array_Case2()
        {
            var text = "number[]";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "`number`");
            Assert.AreEqual(,name.NormalizedNameValue, "number");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(true, name.IsArray);
            Assert.AreEqual(false, name.Capacity.HasValue);
            Assert.AreEqual(true, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Array_Case3()
        {
            var text = "number[*]";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "`number`");
            Assert.AreEqual(,name.NormalizedNameValue, "number");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(true, name.IsArray);
            Assert.AreEqual(false, name.Capacity.HasValue);
            Assert.AreEqual(true, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Array_Case4()
        {
            var text = "number[∞]";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "`number`");
            Assert.AreEqual(,name.NormalizedNameValue, "number");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(true, name.IsArray);
            Assert.AreEqual(false, name.Capacity.HasValue);
            Assert.AreEqual(true, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Namespaces_Case1()
        {
            var text = "global(politics)::dog (animal (alive))";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 2);

            var firstNamespacesElement = name.Namespaces[0];

            Assert.AreEqual(false, firstNamespacesElement.IsEmpty);
            Assert.AreEqual(,firstNamespacesElement.NameValue, "`animal`");
            Assert.AreEqual(,firstNamespacesElement.NormalizedNameValue, "animal");
            Assert.AreEqual(,firstNamespacesElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, firstNamespacesElement.IsArray);
            Assert.AreEqual(false, firstNamespacesElement.Capacity.HasValue);
            Assert.AreEqual(false, firstNamespacesElement.HasInfiniteCapacity);

            Assert.AreEqual(,firstNamespacesElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesElement.Namespaces.Count, 1);

            var firstNamespacesSubElement = firstNamespacesElement.Namespaces[0];

            Assert.AreEqual(false, firstNamespacesSubElement.IsEmpty);
            Assert.AreEqual(,firstNamespacesSubElement.NameValue, "`alive`");
            Assert.AreEqual(,firstNamespacesSubElement.NormalizedNameValue, "alive");
            Assert.AreEqual(,firstNamespacesSubElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, firstNamespacesSubElement.IsArray);
            Assert.AreEqual(false, firstNamespacesSubElement.Capacity.HasValue);
            Assert.AreEqual(false, firstNamespacesSubElement.HasInfiniteCapacity);

            Assert.AreEqual(,firstNamespacesSubElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesSubElement.Namespaces.Count, 0);

            var secondNamespacesElement = name.Namespaces[1];

            Assert.AreEqual(false, secondNamespacesElement.IsEmpty);
            Assert.AreEqual(,secondNamespacesElement.NameValue, "`global`");
            Assert.AreEqual(,secondNamespacesElement.NormalizedNameValue, "global");
            Assert.AreEqual(,secondNamespacesElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, secondNamespacesElement.IsArray);
            Assert.AreEqual(false, secondNamespacesElement.Capacity.HasValue);
            Assert.AreEqual(false, secondNamespacesElement.HasInfiniteCapacity);

            Assert.AreEqual(,secondNamespacesElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,secondNamespacesElement.Namespaces.Count, 1);

            var secondNamespacesSubElement = secondNamespacesElement.Namespaces[0];

            Assert.AreEqual(false, secondNamespacesSubElement.IsEmpty);
            Assert.AreEqual(,secondNamespacesSubElement.NameValue, "`politics`");
            Assert.AreEqual(,secondNamespacesSubElement.NormalizedNameValue, "politics");
            Assert.AreEqual(,secondNamespacesSubElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, secondNamespacesSubElement.IsArray);
            Assert.AreEqual(false, secondNamespacesSubElement.Capacity.HasValue);
            Assert.AreEqual(false, secondNamespacesSubElement.HasInfiniteCapacity);

            Assert.AreEqual(,secondNamespacesSubElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,secondNamespacesSubElement.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Namespaces_Case2()
        {
            var text = "dog (alive::animal | instrument (big))";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 2);

            var firstNamespacesElement = name.Namespaces[0];

            Assert.AreEqual(false, firstNamespacesElement.IsEmpty);
            Assert.AreEqual(,firstNamespacesElement.NameValue, "`animal`");
            Assert.AreEqual(,firstNamespacesElement.NormalizedNameValue, "animal");
            Assert.AreEqual(,firstNamespacesElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, firstNamespacesElement.IsArray);
            Assert.AreEqual(false, firstNamespacesElement.Capacity.HasValue);
            Assert.AreEqual(false, firstNamespacesElement.HasInfiniteCapacity);

            Assert.AreEqual(,firstNamespacesElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesElement.Namespaces.Count, 1);

            var firstNamespacesSubElement = firstNamespacesElement.Namespaces[0];

            Assert.AreEqual(false, firstNamespacesSubElement.IsEmpty);
            Assert.AreEqual(,firstNamespacesSubElement.NameValue, "`alive`");
            Assert.AreEqual(,firstNamespacesSubElement.NormalizedNameValue, "alive");
            Assert.AreEqual(,firstNamespacesSubElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, firstNamespacesSubElement.IsArray);
            Assert.AreEqual(false, firstNamespacesSubElement.Capacity.HasValue);
            Assert.AreEqual(false, firstNamespacesSubElement.HasInfiniteCapacity);

            Assert.AreEqual(,firstNamespacesSubElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesSubElement.Namespaces.Count, 0);

            var secondNamespacesElement = name.Namespaces[1];

            Assert.AreEqual(false, secondNamespacesElement.IsEmpty);
            Assert.AreEqual(,secondNamespacesElement.NameValue, "`instrument`");
            Assert.AreEqual(,secondNamespacesElement.NormalizedNameValue, "instrument");
            Assert.AreEqual(,secondNamespacesElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, secondNamespacesElement.IsArray);
            Assert.AreEqual(false, secondNamespacesElement.Capacity.HasValue);
            Assert.AreEqual(false, secondNamespacesElement.HasInfiniteCapacity);

            Assert.AreEqual(,secondNamespacesElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,secondNamespacesElement.Namespaces.Count, 1);

            var secondNamespacesSubElement = secondNamespacesElement.Namespaces[0];

            Assert.AreEqual(false, secondNamespacesSubElement.IsEmpty);
            Assert.AreEqual(,secondNamespacesSubElement.NameValue, "`big`");
            Assert.AreEqual(,secondNamespacesSubElement.NormalizedNameValue, "big");
            Assert.AreEqual(,secondNamespacesSubElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, secondNamespacesSubElement.IsArray);
            Assert.AreEqual(false, secondNamespacesSubElement.Capacity.HasValue);
            Assert.AreEqual(false, secondNamespacesSubElement.HasInfiniteCapacity);

            Assert.AreEqual(,secondNamespacesSubElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,secondNamespacesSubElement.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Namespaces_Case3()
        {
            var text = "alive::animal::big::dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual(,name.NameValue, "`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 1);

            var firstNamespacesElement = name.Namespaces[0];

            Assert.AreEqual(false, firstNamespacesElement.IsEmpty);
            Assert.AreEqual(,firstNamespacesElement.NameValue, "`big`");
            Assert.AreEqual(,firstNamespacesElement.NormalizedNameValue, "big");
            Assert.AreEqual(,firstNamespacesElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, firstNamespacesElement.IsArray);
            Assert.AreEqual(false, firstNamespacesElement.Capacity.HasValue);
            Assert.AreEqual(false, firstNamespacesElement.HasInfiniteCapacity);

            Assert.AreEqual(,firstNamespacesElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesElement.Namespaces.Count, 1);

            var firstNamespacesSubElement = firstNamespacesElement.Namespaces[0];

            Assert.AreEqual(false, firstNamespacesSubElement.IsEmpty);
            Assert.AreEqual(,firstNamespacesSubElement.NameValue, "`animal`");
            Assert.AreEqual(,firstNamespacesSubElement.NormalizedNameValue, "animal");
            Assert.AreEqual(,firstNamespacesSubElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, firstNamespacesSubElement.IsArray);
            Assert.AreEqual(false, firstNamespacesSubElement.Capacity.HasValue);
            Assert.AreEqual(false, firstNamespacesSubElement.HasInfiniteCapacity);

            Assert.AreEqual(,firstNamespacesSubElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesSubElement.Namespaces.Count, 1);

            var firstNamespacesSubSubElement = firstNamespacesSubElement.Namespaces[0];

            Assert.AreEqual(false, firstNamespacesSubSubElement.IsEmpty);
            Assert.AreEqual(,firstNamespacesSubSubElement.NameValue, "`alive`");
            Assert.AreEqual(,firstNamespacesSubSubElement.NormalizedNameValue, "alive");
            Assert.AreEqual(,firstNamespacesSubSubElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(false, firstNamespacesSubSubElement.IsArray);
            Assert.AreEqual(false, firstNamespacesSubSubElement.Capacity.HasValue);
            Assert.AreEqual(false, firstNamespacesSubSubElement.HasInfiniteCapacity);

            Assert.AreEqual(,firstNamespacesSubSubElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesSubSubElement.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Namespaces_Case4()
        {
            var text = "(animal | instrument)::dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(false, name.IsEmpty);
            Assert.AreEqual("`dog`", name.NameValue);
            Assert.AreEqual("dog", name.NormalizedNameValue);
            Assert.AreEqual(KindOfName.CommonConcept, name.KindOfName);

            Assert.AreEqual(false, name.IsArray);
            Assert.AreEqual(true, name.Capacity.HasValue);
            Assert.AreEqual(1, name.Capacity.Value);
            Assert.AreEqual(false, name.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, name.Level);

            Assert.AreEqual(2, name.Namespaces.Count);

            var firstNamespacesElement = name.Namespaces[0];

            Assert.AreEqual(false, firstNamespacesElement.IsEmpty);
            Assert.AreEqual("`animal`", firstNamespacesElement.NameValue);
            Assert.AreEqual("animal", firstNamespacesElement.NormalizedNameValue);
            Assert.AreEqual(KindOfName.CommonConcept, firstNamespacesElement.KindOfName);

            Assert.AreEqual(false, firstNamespacesElement.IsArray);
            Assert.AreEqual(false, firstNamespacesElement.Capacity.HasValue);
            Assert.AreEqual(false, firstNamespacesElement.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, firstNamespacesElement.Level);

            Assert.AreEqual(0, firstNamespacesElement.Namespaces.Count);

            var secondNamespacesElement = name.Namespaces[1];

            Assert.AreEqual(false, secondNamespacesElement.IsEmpty);
            Assert.AreEqual("`instrument`", secondNamespacesElement.NameValue);
            Assert.AreEqual("instrument", secondNamespacesElement.NormalizedNameValue);
            Assert.AreEqual(KindOfName.CommonConcept, secondNamespacesElement.KindOfName);

            Assert.AreEqual(false, secondNamespacesElement.IsArray);
            Assert.AreEqual(false, secondNamespacesElement.Capacity.HasValue);
            Assert.AreEqual(false, secondNamespacesElement.HasInfiniteCapacity);

            Assert.AreEqual(StrongIdentifierLevel.None, secondNamespacesElement.Level);

            Assert.AreEqual(0, secondNamespacesElement.Namespaces.Count);
        }
    }
}

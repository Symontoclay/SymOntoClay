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

            Assert.AreEqual(,name.IsEmpty, true);
            Assert.AreEqual(,name.NameValue, string.Empty);
            Assert.AreEqual(,name.NormalizedNameValue, string.Empty);
            Assert.AreEqual(,name.KindOfName, KindOfName.Unknown);

            Assert.AreEqual(,name.KindOfValue, KindOfValue.StrongIdentifierValue);
            Assert.AreEqual(,name.IsStrongIdentifierValue, true);
            Assert.AreEqual(,name.AsStrongIdentifierValue, name);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, false);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Empty_Case2()
        {
            var name = NameHelper.CreateName(string.Empty);

            Assert.AreEqual(,name.IsEmpty, true);
            Assert.AreEqual(,name.NameValue, string.Empty);
            Assert.AreEqual(,name.NormalizedNameValue, string.Empty);
            Assert.AreEqual(,name.KindOfName, KindOfName.Unknown);

            Assert.AreEqual(,name.KindOfValue, KindOfValue.StrongIdentifierValue);
            Assert.AreEqual(,name.IsStrongIdentifierValue, true);
            Assert.AreEqual(,name.AsStrongIdentifierValue, name);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, false);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void CommonConcept_Case1()
        {
            var text = "dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, text);
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void CommonConcept_Case2()
        {
            var name = NameHelper.CreateName("Dog");

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`Dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void CommonConcept_Case3()
        {
            var text = "small dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`small dog`");
            Assert.AreEqual(,name.NormalizedNameValue, text);
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void CommonConcept_Case4()
        {
            var text = "__ctor";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`__ctor`");
            Assert.AreEqual(,name.NormalizedNameValue, "__ctor");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Channel_Case1()
        {
            var text = "@>log";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "@>`log`");
            Assert.AreEqual(,name.NormalizedNameValue, text);
            Assert.AreEqual(,name.KindOfName, KindOfName.Channel);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Channel_Case1_a()
        {
            var text = "@>`log`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "@>`log`");
            Assert.AreEqual(,name.NormalizedNameValue, "@>log");
            Assert.AreEqual(,name.KindOfName, KindOfName.Channel);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Channel_Case1_b()
        {
            var text = "@>`big log`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "@>`big log`");
            Assert.AreEqual(,name.NormalizedNameValue, "@>big log");
            Assert.AreEqual(,name.KindOfName, KindOfName.Channel);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void CreateRuleOrFactName_Case1_b()
        {
            var name = NameHelper.CreateRuleOrFactName();

            Assert.AreEqual(true, name.NameValue.StartsWith("#^"));
            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreNotEqual(,name.NameValue, string.Empty);
            Assert.AreNotEqual(,name.NormalizedNameValue, string.Empty);
            Assert.AreEqual(,name.KindOfName, KindOfName.RuleOrFact);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void RuleOrFactName_Case2()
        {
            var text = "#^91e029e7-6a4c-454b-b15b-323d2b5ff0a9";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#^`91e029e7-6a4c-454b-b15b-323d2b5ff0a9`");
            Assert.AreEqual(,name.NormalizedNameValue, "#^91e029e7-6a4c-454b-b15b-323d2b5ff0a9");
            Assert.AreEqual(,name.KindOfName, KindOfName.RuleOrFact);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void RuleOrFactName_Case2_a()
        {
            var text = "#^`91e029e7-6a4c-454b-b15b-323d2b5ff0a9`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#^`91e029e7-6a4c-454b-b15b-323d2b5ff0a9`");
            Assert.AreEqual(,name.NormalizedNameValue, "#^91e029e7-6a4c-454b-b15b-323d2b5ff0a9");
            Assert.AreEqual(,name.KindOfName, KindOfName.RuleOrFact);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Entity_Case1()
        {
            var text = "#dog1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#`dog1`");
            Assert.AreEqual(,name.NormalizedNameValue, text);
            Assert.AreEqual(,name.KindOfName, KindOfName.Entity);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Entity_Case2()
        {
            var text = "#Tom";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#`Tom`");
            Assert.AreEqual(,name.NormalizedNameValue, "#tom");
            Assert.AreEqual(,name.KindOfName, KindOfName.Entity);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Entity_Case3()
        {
            var text = "#`Barrel 1`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#`Barrel 1`");
            Assert.AreEqual(,name.NormalizedNameValue, "#barrel 1");
            Assert.AreEqual(,name.KindOfName, KindOfName.Entity);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Entity_Case4()
        {
            var text = "#020ED339-6313-459A-900D-92F809CEBDC5";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#`020ED339-6313-459A-900D-92F809CEBDC5`");
            Assert.AreEqual(,name.NormalizedNameValue, "#020ed339-6313-459a-900d-92f809cebdc5");
            Assert.AreEqual(,name.KindOfName, KindOfName.Entity);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Entity_Case4_a()
        {
            var text = "#`020ED339-6313-459A-900D-92F809CEBDC5`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#`020ED339-6313-459A-900D-92F809CEBDC5`");
            Assert.AreEqual(,name.NormalizedNameValue, "#020ed339-6313-459a-900d-92f809cebdc5");
            Assert.AreEqual(,name.KindOfName, KindOfName.Entity);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Var_Case1()
        {
            var text = "@a";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "@`a`");
            Assert.AreEqual(,name.NormalizedNameValue, "@a");
            Assert.AreEqual(,name.KindOfName, KindOfName.Var);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void SystemVar_Case1()
        {
            var text = "@@host";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "@@`host`");
            Assert.AreEqual(,name.NormalizedNameValue, "@@host");
            Assert.AreEqual(,name.KindOfName, KindOfName.SystemVar);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void SystemVar_Case1_a()
        {
            var text = "@@`host`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "@@`host`");
            Assert.AreEqual(,name.NormalizedNameValue, "@@host");
            Assert.AreEqual(,name.KindOfName, KindOfName.SystemVar);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void SystemVar_Case1_b()
        {
            var text = "@@`big host`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "@@`big host`");
            Assert.AreEqual(,name.NormalizedNameValue, "@@big host");
            Assert.AreEqual(,name.KindOfName, KindOfName.SystemVar);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void LogicVar_Case1()
        {
            var text = "$x";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "$`x`");
            Assert.AreEqual(,name.NormalizedNameValue, "$x");
            Assert.AreEqual(,name.KindOfName, KindOfName.LogicalVar);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void LogicVar_Case2()
        {
            var text = "$_";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "$`_`");
            Assert.AreEqual(,name.NormalizedNameValue, "$_");
            Assert.AreEqual(,name.KindOfName, KindOfName.LogicalVar);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void LogicVar_Case2_a()
        {
            var text = "$`_`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "$`_`");
            Assert.AreEqual(,name.NormalizedNameValue, "$_");
            Assert.AreEqual(,name.KindOfName, KindOfName.LogicalVar);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void LinguisticVar_Case1()
        {
            var text = "#|`teenager`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#|`teenager`");
            Assert.AreEqual(,name.NormalizedNameValue, "#|teenager");
            Assert.AreEqual(,name.KindOfName, KindOfName.LinguisticVar);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void LinguisticVar_Case1_a()
        {
            var text = "#|teenager";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#|`teenager`");
            Assert.AreEqual(,name.NormalizedNameValue, "#|teenager");
            Assert.AreEqual(,name.KindOfName, KindOfName.LinguisticVar);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void LinguisticVar_Case1_b()
        {
            var text = "#|`big teenager`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#|`big teenager`");
            Assert.AreEqual(,name.NormalizedNameValue, "#|big teenager");
            Assert.AreEqual(,name.KindOfName, KindOfName.LinguisticVar);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Concept_Case1()
        {
            var text = "##dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "##`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, text);
            Assert.AreEqual(,name.KindOfName, KindOfName.Concept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Concept_Case2()
        {
            var text = "##`dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "##`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "##dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.Concept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Concept_Case3()
        {
            var text = "##`small dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "##`small dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "##small dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.Concept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Property_Case1()
        {
            var text = "@:Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "@:`Prop1`");
            Assert.AreEqual(,name.NormalizedNameValue, "@:prop1");
            Assert.AreEqual(,name.KindOfName, KindOfName.Property);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Property_Case2()
        {
            var text = "@:`small dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "@:`small dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "@:small dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.Property);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void EntityCondition_Case1()
        {
            var text = "#@";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#@");
            Assert.AreEqual(,name.NormalizedNameValue, "#@");
            Assert.AreEqual(,name.KindOfName, KindOfName.AnonymousEntityCondition);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void EntityCondition_Case2()
        {
            var text = "#@`dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#@`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "#@dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.EntityCondition);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void EntityCondition_Case2_a()
        {
            var text = "#@dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#@`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "#@dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.EntityCondition);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void EntityCondition_Case3()
        {
            var text = "#@`small dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "#@`small dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "#@small dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.EntityCondition);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void OnceResolvedEntityCondition_Case1()
        {
            var text = "##@";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "##@");
            Assert.AreEqual(,name.NormalizedNameValue, "##@");
            Assert.AreEqual(,name.KindOfName, KindOfName.OnceResolvedAnonymousEntityCondition);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void OnceResolvedEntityCondition_Case2()
        {
            var text = "##@`dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "##@`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "##@dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.OnceResolvedEntityCondition);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void OnceResolvedEntityCondition_Case2_a()
        {
            var text = "##@dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "##@`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "##@dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.OnceResolvedEntityCondition);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void OnceResolvedEntityCondition_Case3()
        {
            var text = "##@`small dog`";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "##@`small dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "##@small dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.OnceResolvedEntityCondition);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Level_Case1()
        {
            var text = "global::Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`Prop1`");
            Assert.AreEqual(,name.NormalizedNameValue, "prop1");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.Global);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Level_Case2()
        {
            var text = "global::@:Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "@:`Prop1`");
            Assert.AreEqual(,name.NormalizedNameValue, "@:prop1");
            Assert.AreEqual(,name.KindOfName, KindOfName.Property);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.Global);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Level_Case3()
        {
            var text = "global::##Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "##`Prop1`");
            Assert.AreEqual(,name.NormalizedNameValue, "##prop1");
            Assert.AreEqual(,name.KindOfName, KindOfName.Concept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.Global);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Level_Case4()
        {
            var text = "root::Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`Prop1`");
            Assert.AreEqual(,name.NormalizedNameValue, "prop1");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.Root);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Level_Case5()
        {
            var text = "strategic::Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`Prop1`");
            Assert.AreEqual(,name.NormalizedNameValue, "prop1");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.Strategic);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Level_Case6()
        {
            var text = "tactical::Prop1";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`Prop1`");
            Assert.AreEqual(,name.NormalizedNameValue, "prop1");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.Tactical);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Array_Case1()
        {
            var text = "number[5]";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`number`");
            Assert.AreEqual(,name.NormalizedNameValue, "number");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, true);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 5);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Array_Case2()
        {
            var text = "number[]";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`number`");
            Assert.AreEqual(,name.NormalizedNameValue, "number");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, true);
            Assert.AreEqual(,name.Capacity.HasValue, false);
            Assert.AreEqual(,name.HasInfiniteCapacity, true);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Array_Case3()
        {
            var text = "number[*]";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`number`");
            Assert.AreEqual(,name.NormalizedNameValue, "number");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, true);
            Assert.AreEqual(,name.Capacity.HasValue, false);
            Assert.AreEqual(,name.HasInfiniteCapacity, true);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Array_Case4()
        {
            var text = "number[∞]";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`number`");
            Assert.AreEqual(,name.NormalizedNameValue, "number");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, true);
            Assert.AreEqual(,name.Capacity.HasValue, false);
            Assert.AreEqual(,name.HasInfiniteCapacity, true);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Namespaces_Case1()
        {
            var text = "global(politics)::dog (animal (alive))";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 2);

            var firstNamespacesElement = name.Namespaces[0];

            Assert.AreEqual(,firstNamespacesElement.IsEmpty, false);
            Assert.AreEqual(,firstNamespacesElement.NameValue, "`animal`");
            Assert.AreEqual(,firstNamespacesElement.NormalizedNameValue, "animal");
            Assert.AreEqual(,firstNamespacesElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,firstNamespacesElement.IsArray, false);
            Assert.AreEqual(,firstNamespacesElement.Capacity.HasValue, false);
            Assert.AreEqual(,firstNamespacesElement.HasInfiniteCapacity, false);

            Assert.AreEqual(,firstNamespacesElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesElement.Namespaces.Count, 1);

            var firstNamespacesSubElement = firstNamespacesElement.Namespaces[0];

            Assert.AreEqual(,firstNamespacesSubElement.IsEmpty, false);
            Assert.AreEqual(,firstNamespacesSubElement.NameValue, "`alive`");
            Assert.AreEqual(,firstNamespacesSubElement.NormalizedNameValue, "alive");
            Assert.AreEqual(,firstNamespacesSubElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,firstNamespacesSubElement.IsArray, false);
            Assert.AreEqual(,firstNamespacesSubElement.Capacity.HasValue, false);
            Assert.AreEqual(,firstNamespacesSubElement.HasInfiniteCapacity, false);

            Assert.AreEqual(,firstNamespacesSubElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesSubElement.Namespaces.Count, 0);

            var secondNamespacesElement = name.Namespaces[1];

            Assert.AreEqual(,secondNamespacesElement.IsEmpty, false);
            Assert.AreEqual(,secondNamespacesElement.NameValue, "`global`");
            Assert.AreEqual(,secondNamespacesElement.NormalizedNameValue, "global");
            Assert.AreEqual(,secondNamespacesElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,secondNamespacesElement.IsArray, false);
            Assert.AreEqual(,secondNamespacesElement.Capacity.HasValue, false);
            Assert.AreEqual(,secondNamespacesElement.HasInfiniteCapacity, false);

            Assert.AreEqual(,secondNamespacesElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,secondNamespacesElement.Namespaces.Count, 1);

            var secondNamespacesSubElement = secondNamespacesElement.Namespaces[0];

            Assert.AreEqual(,secondNamespacesSubElement.IsEmpty, false);
            Assert.AreEqual(,secondNamespacesSubElement.NameValue, "`politics`");
            Assert.AreEqual(,secondNamespacesSubElement.NormalizedNameValue, "politics");
            Assert.AreEqual(,secondNamespacesSubElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,secondNamespacesSubElement.IsArray, false);
            Assert.AreEqual(,secondNamespacesSubElement.Capacity.HasValue, false);
            Assert.AreEqual(,secondNamespacesSubElement.HasInfiniteCapacity, false);

            Assert.AreEqual(,secondNamespacesSubElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,secondNamespacesSubElement.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Namespaces_Case2()
        {
            var text = "dog (alive::animal | instrument (big))";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 2);

            var firstNamespacesElement = name.Namespaces[0];

            Assert.AreEqual(,firstNamespacesElement.IsEmpty, false);
            Assert.AreEqual(,firstNamespacesElement.NameValue, "`animal`");
            Assert.AreEqual(,firstNamespacesElement.NormalizedNameValue, "animal");
            Assert.AreEqual(,firstNamespacesElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,firstNamespacesElement.IsArray, false);
            Assert.AreEqual(,firstNamespacesElement.Capacity.HasValue, false);
            Assert.AreEqual(,firstNamespacesElement.HasInfiniteCapacity, false);

            Assert.AreEqual(,firstNamespacesElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesElement.Namespaces.Count, 1);

            var firstNamespacesSubElement = firstNamespacesElement.Namespaces[0];

            Assert.AreEqual(,firstNamespacesSubElement.IsEmpty, false);
            Assert.AreEqual(,firstNamespacesSubElement.NameValue, "`alive`");
            Assert.AreEqual(,firstNamespacesSubElement.NormalizedNameValue, "alive");
            Assert.AreEqual(,firstNamespacesSubElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,firstNamespacesSubElement.IsArray, false);
            Assert.AreEqual(,firstNamespacesSubElement.Capacity.HasValue, false);
            Assert.AreEqual(,firstNamespacesSubElement.HasInfiniteCapacity, false);

            Assert.AreEqual(,firstNamespacesSubElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesSubElement.Namespaces.Count, 0);

            var secondNamespacesElement = name.Namespaces[1];

            Assert.AreEqual(,secondNamespacesElement.IsEmpty, false);
            Assert.AreEqual(,secondNamespacesElement.NameValue, "`instrument`");
            Assert.AreEqual(,secondNamespacesElement.NormalizedNameValue, "instrument");
            Assert.AreEqual(,secondNamespacesElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,secondNamespacesElement.IsArray, false);
            Assert.AreEqual(,secondNamespacesElement.Capacity.HasValue, false);
            Assert.AreEqual(,secondNamespacesElement.HasInfiniteCapacity, false);

            Assert.AreEqual(,secondNamespacesElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,secondNamespacesElement.Namespaces.Count, 1);

            var secondNamespacesSubElement = secondNamespacesElement.Namespaces[0];

            Assert.AreEqual(,secondNamespacesSubElement.IsEmpty, false);
            Assert.AreEqual(,secondNamespacesSubElement.NameValue, "`big`");
            Assert.AreEqual(,secondNamespacesSubElement.NormalizedNameValue, "big");
            Assert.AreEqual(,secondNamespacesSubElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,secondNamespacesSubElement.IsArray, false);
            Assert.AreEqual(,secondNamespacesSubElement.Capacity.HasValue, false);
            Assert.AreEqual(,secondNamespacesSubElement.HasInfiniteCapacity, false);

            Assert.AreEqual(,secondNamespacesSubElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,secondNamespacesSubElement.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Namespaces_Case3()
        {
            var text = "alive::animal::big::dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 1);

            var firstNamespacesElement = name.Namespaces[0];

            Assert.AreEqual(,firstNamespacesElement.IsEmpty, false);
            Assert.AreEqual(,firstNamespacesElement.NameValue, "`big`");
            Assert.AreEqual(,firstNamespacesElement.NormalizedNameValue, "big");
            Assert.AreEqual(,firstNamespacesElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,firstNamespacesElement.IsArray, false);
            Assert.AreEqual(,firstNamespacesElement.Capacity.HasValue, false);
            Assert.AreEqual(,firstNamespacesElement.HasInfiniteCapacity, false);

            Assert.AreEqual(,firstNamespacesElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesElement.Namespaces.Count, 1);

            var firstNamespacesSubElement = firstNamespacesElement.Namespaces[0];

            Assert.AreEqual(,firstNamespacesSubElement.IsEmpty, false);
            Assert.AreEqual(,firstNamespacesSubElement.NameValue, "`animal`");
            Assert.AreEqual(,firstNamespacesSubElement.NormalizedNameValue, "animal");
            Assert.AreEqual(,firstNamespacesSubElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,firstNamespacesSubElement.IsArray, false);
            Assert.AreEqual(,firstNamespacesSubElement.Capacity.HasValue, false);
            Assert.AreEqual(,firstNamespacesSubElement.HasInfiniteCapacity, false);

            Assert.AreEqual(,firstNamespacesSubElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesSubElement.Namespaces.Count, 1);

            var firstNamespacesSubSubElement = firstNamespacesSubElement.Namespaces[0];

            Assert.AreEqual(,firstNamespacesSubSubElement.IsEmpty, false);
            Assert.AreEqual(,firstNamespacesSubSubElement.NameValue, "`alive`");
            Assert.AreEqual(,firstNamespacesSubSubElement.NormalizedNameValue, "alive");
            Assert.AreEqual(,firstNamespacesSubSubElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,firstNamespacesSubSubElement.IsArray, false);
            Assert.AreEqual(,firstNamespacesSubSubElement.Capacity.HasValue, false);
            Assert.AreEqual(,firstNamespacesSubSubElement.HasInfiniteCapacity, false);

            Assert.AreEqual(,firstNamespacesSubSubElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesSubSubElement.Namespaces.Count, 0);
        }

        [Test]
        [Parallelizable]
        public void Namespaces_Case4()
        {
            var text = "(animal | instrument)::dog";

            var name = NameHelper.CreateName(text);

            Assert.AreEqual(,name.IsEmpty, false);
            Assert.AreEqual(,name.NameValue, "`dog`");
            Assert.AreEqual(,name.NormalizedNameValue, "dog");
            Assert.AreEqual(,name.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,name.IsArray, false);
            Assert.AreEqual(,name.Capacity.HasValue, true);
            Assert.AreEqual(,name.Capacity.Value, 1);
            Assert.AreEqual(,name.HasInfiniteCapacity, false);

            Assert.AreEqual(,name.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,name.Namespaces.Count, 2);

            var firstNamespacesElement = name.Namespaces[0];

            Assert.AreEqual(,firstNamespacesElement.IsEmpty, false);
            Assert.AreEqual(,firstNamespacesElement.NameValue, "`animal`");
            Assert.AreEqual(,firstNamespacesElement.NormalizedNameValue, "animal");
            Assert.AreEqual(,firstNamespacesElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,firstNamespacesElement.IsArray, false);
            Assert.AreEqual(,firstNamespacesElement.Capacity.HasValue, false);
            Assert.AreEqual(,firstNamespacesElement.HasInfiniteCapacity, false);

            Assert.AreEqual(,firstNamespacesElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,firstNamespacesElement.Namespaces.Count, 0);

            var secondNamespacesElement = name.Namespaces[1];

            Assert.AreEqual(,secondNamespacesElement.IsEmpty, false);
            Assert.AreEqual(,secondNamespacesElement.NameValue, "`instrument`");
            Assert.AreEqual(,secondNamespacesElement.NormalizedNameValue, "instrument");
            Assert.AreEqual(,secondNamespacesElement.KindOfName, KindOfName.CommonConcept);

            Assert.AreEqual(,secondNamespacesElement.IsArray, false);
            Assert.AreEqual(,secondNamespacesElement.Capacity.HasValue, false);
            Assert.AreEqual(,secondNamespacesElement.HasInfiniteCapacity, false);

            Assert.AreEqual(,secondNamespacesElement.Level, StrongIdentifierLevel.None);

            Assert.AreEqual(,secondNamespacesElement.Namespaces.Count, 0);
        }
    }
}

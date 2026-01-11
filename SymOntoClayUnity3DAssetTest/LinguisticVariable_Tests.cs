/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.BaseTestLib;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class LinguisticVariable_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"linvar age for range (0, 150]
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(true, linguisticVariable.Constraint.IsEmpty);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case1_a()
        {
            var text = @"linvar age for range [0, 150)
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, true, 0, 150, false);

            Assert.AreEqual(true, linguisticVariable.Constraint.IsEmpty);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case1_b()
        {
            var text = @"linvar age for range (0, 150)
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, false, 0, 150, false);

            Assert.AreEqual(true, linguisticVariable.Constraint.IsEmpty);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case1_c()
        {
            var text = @"linvar age for range [0, 150]
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, true, 0, 150, true);

            Assert.AreEqual(true, linguisticVariable.Constraint.IsEmpty);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var text = @"linvar age for range (-∞, +∞)
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range);

            Assert.AreEqual(true, linguisticVariable.Constraint.IsEmpty);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            var text = @"linvar age for range (*, *)
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range);

            Assert.AreEqual(true, linguisticVariable.Constraint.IsEmpty);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case4()
        {
            var text = @"linvar age
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range);

            Assert.AreEqual(true, linguisticVariable.Constraint.IsEmpty);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case5()
        {
            var text = @"linvar age
{
    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range);

            Assert.AreEqual(true, linguisticVariable.Constraint.IsEmpty);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case6()
        {
            var text = @"linvar age for range (0, 150]
{
    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(true, linguisticVariable.Constraint.IsEmpty);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case7()
        {
            var text = @"linvar age for range (0, 150]
{
	`teenager` = L(5, 10);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(true, linguisticVariable.Constraint.IsEmpty);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.LFunction, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 5"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 10"));
        }

        [Test]
        [Parallelizable]
        public void Case8()
        {
            var text = @"linvar age for range (0, 150]
{
    `teenager` = S(12, 22);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(true, linguisticVariable.Constraint.IsEmpty);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.SFunction, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_m = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 22"));
        }

        [Test]
        [Parallelizable]
        public void Case9()
        {
            var text = @"linvar age for range (0, 150]
{
    `teenager` = S(12, 17, 22);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(true, linguisticVariable.Constraint.IsEmpty);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.SFunction, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_m = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 22"));
        }

        [Test]
        [Parallelizable]
        public void Case10()
        {
            var text = @"linvar age for range (0, 150]
{
        constraints:
	    for inheritance;
	    for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(false, linguisticVariable.Constraint.IsEmpty);

            var constraintItems = linguisticVariable.Constraint.Items.ToList();

            Assert.AreEqual(3, constraintItems.Count);

            var item1 = constraintItems[0];
            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Inheritance, item1.Kind);
            Assert.AreEqual(null, item1.RelationName);

            var item2 = constraintItems[1];

            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Relation, item2.Kind);
            Assert.AreEqual(NameHelper.CreateName("age"), item2.RelationName);

            var item3 = constraintItems[2];

            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Relation, item3.Kind);
            Assert.AreEqual(NameHelper.CreateName("is"), item3.RelationName);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case11()
        {
            var text = @"linvar age for range (0, 150]
{
    constraints:
        for inh;
        for rel age;

    terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(false, linguisticVariable.Constraint.IsEmpty);

            var constraintItems = linguisticVariable.Constraint.Items.ToList();

            Assert.AreEqual(3, constraintItems.Count);

            var item1 = constraintItems[0];
            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Inheritance, item1.Kind);
            Assert.AreEqual(null, item1.RelationName);

            var item2 = constraintItems[1];

            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Relation, item2.Kind);
            Assert.AreEqual(NameHelper.CreateName("age"), item2.RelationName);

            var item3 = constraintItems[2];

            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Relation, item3.Kind);
            Assert.AreEqual(NameHelper.CreateName("is"), item3.RelationName);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case12()
        {
            var text = @"linvar age for range (0, 150]
{
    constraints:
	    for inheritance;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(false, linguisticVariable.Constraint.IsEmpty);

            var constraintItems = linguisticVariable.Constraint.Items.ToList();

            Assert.AreEqual(2, constraintItems.Count);

            var item1 = constraintItems[0];
            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Inheritance, item1.Kind);
            Assert.AreEqual(null, item1.RelationName);

            var item2 = constraintItems[1];

            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Relation, item2.Kind);
            Assert.AreEqual(NameHelper.CreateName("is"), item2.RelationName);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case13()
        {
            var text = @"linvar age for range (0, 150]
{
    constraints:
        for inh;

    terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(false, linguisticVariable.Constraint.IsEmpty);

            var constraintItems = linguisticVariable.Constraint.Items.ToList();

            Assert.AreEqual(2, constraintItems.Count);

            var item1 = constraintItems[0];
            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Inheritance, item1.Kind);
            Assert.AreEqual(null, item1.RelationName);

            var item2 = constraintItems[1];

            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Relation, item2.Kind);
            Assert.AreEqual(NameHelper.CreateName("is"), item2.RelationName);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case14()
        {
            var text = @"linvar age for range (0, 150]
{
    constraints:
	    for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(false, linguisticVariable.Constraint.IsEmpty);

            var constraintItems = linguisticVariable.Constraint.Items.ToList();

            Assert.AreEqual(1, constraintItems.Count);

            var item1 = constraintItems[0];

            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Relation, item1.Kind);
            Assert.AreEqual(NameHelper.CreateName("age"), item1.RelationName);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case15()
        {
            var text = @"linvar age for range (0, 150]
{
    constraints:
        for rel age;

    terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("age"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(false, linguisticVariable.Constraint.IsEmpty);

            var constraintItems = linguisticVariable.Constraint.Items.ToList();

            Assert.AreEqual(1, constraintItems.Count);

            var item1 = constraintItems[0];

            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Relation, item1.Kind);
            Assert.AreEqual(NameHelper.CreateName("age"), item1.RelationName);

            var term = linguisticVariable.Values.Single();

            Assert.AreEqual(NameHelper.CreateName("teenager"), term.Name);
            Assert.AreEqual(linguisticVariable, term.Parent);

            var handler = term.Handler;

            Assert.AreNotEqual(null, handler);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler.Kind);

            var handlerStr = handler.ToString();

            Assert.AreEqual(true, handlerStr.Contains("_a = 10"));
            Assert.AreEqual(true, handlerStr.Contains("_b = 12"));
            Assert.AreEqual(true, handlerStr.Contains("_c = 17"));
            Assert.AreEqual(true, handlerStr.Contains("_d = 20"));
        }

        [Test]
        [Parallelizable]
        public void Case16()
        {
            var text = @"linvar logic for range [0, 1]
{
    constraints:
	    for inheritance;

	terms:
		minimal = L(0, 0.1);
		low = Trapezoid(0, 0.05, 0.3, 0.45);
		middle = Trapezoid(0.3, 0.4, 0.6, 0.7);
		high = Trapezoid(0.55, 0.7, 0.95, 1);
		maximal = S(0.9, 1);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "logic");

            var linguisticVariable = codeEntity.AsLinguisticVariable;

            linguisticVariable.CheckDirty();

            Assert.AreEqual(NameHelper.CreateName("logic"), linguisticVariable.Name);

            CheckRange(linguisticVariable.Range, true, 0, 1, true);

            Assert.AreEqual(false, linguisticVariable.Constraint.IsEmpty);

            var constraintItems = linguisticVariable.Constraint.Items.ToList();

            Assert.AreEqual(2, constraintItems.Count);

            var item1 = constraintItems[0];
            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Inheritance, item1.Kind);
            Assert.AreEqual(null, item1.RelationName);

            var item2 = constraintItems[1];

            Assert.AreEqual(KindOfLinguisticVariableConstraintItem.Relation, item2.Kind);
            Assert.AreEqual(NameHelper.CreateName("is"), item2.RelationName);

            var termsList = linguisticVariable.Values;

            Assert.AreEqual(5, termsList.Count);

            var term1 = termsList[0];

            Assert.AreEqual(NameHelper.CreateName("minimal"), term1.Name);
            Assert.AreEqual(linguisticVariable, term1.Parent);

            var handler1 = term1.Handler;

            Assert.AreNotEqual(null, handler1);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.LFunction, handler1.Kind);

            var handlerStr1 = handler1.ToString();

            Assert.AreEqual(true, handlerStr1.Contains("_a = 0"));
            Assert.AreEqual(true, handlerStr1.Contains("_b = 0.1"));

            var term2 = termsList[1];

            Assert.AreEqual(NameHelper.CreateName("low"), term2.Name);
            Assert.AreEqual(linguisticVariable, term2.Parent);

            var handler2 = term2.Handler;

            Assert.AreNotEqual(null, handler2);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler2.Kind);

            var handlerStr2 = handler2.ToString();

            Assert.AreEqual(true, handlerStr2.Contains("_a = 0"));
            Assert.AreEqual(true, handlerStr2.Contains("_b = 0.05"));
            Assert.AreEqual(true, handlerStr2.Contains("_c = 0.3"));
            Assert.AreEqual(true, handlerStr2.Contains("_d = 0.45"));

            var term3 = termsList[2];

            Assert.AreEqual(NameHelper.CreateName("middle"), term3.Name);
            Assert.AreEqual(linguisticVariable, term3.Parent);

            var handler3 = term3.Handler;

            Assert.AreNotEqual(null, handler3);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler3.Kind);

            var handlerStr3 = handler3.ToString();

            Assert.AreEqual(true, handlerStr3.Contains("_a = 0.3"));
            Assert.AreEqual(true, handlerStr3.Contains("_b = 0.4"));
            Assert.AreEqual(true, handlerStr3.Contains("_c = 0.6"));
            Assert.AreEqual(true, handlerStr3.Contains("_d = 0.7"));

            var term4 = termsList[3];

            Assert.AreEqual(NameHelper.CreateName("high"), term4.Name);
            Assert.AreEqual(linguisticVariable, term4.Parent);

            var handler4 = term4.Handler;

            Assert.AreNotEqual(null, handler4);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.Trapezoid, handler4.Kind);

            var handlerStr4 = handler4.ToString();

            Assert.AreEqual(true, handlerStr4.Contains("_a = 0.55"));
            Assert.AreEqual(true, handlerStr4.Contains("_b = 0.7"));
            Assert.AreEqual(true, handlerStr4.Contains("_c = 0.95"));
            Assert.AreEqual(true, handlerStr4.Contains("_d = 1"));

            var term5 = termsList[4];

            Assert.AreEqual(NameHelper.CreateName("maximal"), term5.Name);
            Assert.AreEqual(linguisticVariable, term5.Parent);

            var handler5 = term5.Handler;

            Assert.AreNotEqual(null, handler5);

            Assert.AreEqual(KindOfFuzzyLogicMemberFunction.SFunction, handler5.Kind);

            var handlerStr5 = handler5.ToString();

            Assert.AreEqual(true, handlerStr5.Contains("_a = 0.9"));
            Assert.AreEqual(true, handlerStr5.Contains("_m = 0.95"));
            Assert.AreEqual(true, handlerStr5.Contains("_b = 1"));
        }

        [Test]
        [Parallelizable]
        public void Case17()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @b: number = `teenager`;
        @b >> @>log;
        'End' >> @>log;
    }
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("14.777777777777782", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case18()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        var @b: number = #|`teenager`;
        @b >> @>log;
        'End' >> @>log;
    }

    prop Teenager: number = 16;
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("14.777777777777782", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(3, maxN);
        }

        private void CheckCodeEntity(CodeItem codeEntity, string name)
        {
            Assert.AreEqual(KindOfCodeEntity.LinguisticVariable, codeEntity.Kind);
            Assert.AreEqual(NameHelper.CreateName(name), codeEntity.Name);
            Assert.AreNotEqual(null, codeEntity.AsLinguisticVariable);
            Assert.AreEqual(0, codeEntity.InheritanceItems.Count);
        }

        private void CheckRange(RangeValue rangeValue)
        {
            Assert.AreEqual(null, rangeValue.LeftBoundary);
            Assert.AreEqual(null, rangeValue.RightBoundary);
        }

        private void CheckRange(RangeValue rangeValue, bool leftBoundaryIncludes, double leftValue, double rightValue, bool rightValueIncludes)
        {
            Assert.AreEqual(leftBoundaryIncludes, rangeValue.LeftBoundary.Includes);
            Assert.AreEqual(leftValue, rangeValue.LeftBoundary.Value.GetSystemValue());
            Assert.AreEqual(rightValue, rangeValue.RightBoundary.Value.GetSystemValue());
            Assert.AreEqual(rightValueIncludes, rangeValue.RightBoundary.Includes);
        }

        [SetUp]
        public void Setup()
        {
            _mainStorageContext = new UnityTestMainStorageContext();
        }

        private IMainStorageContext _mainStorageContext;

        private CodeItem Parse(string text)
        {
            var codeFile = new CodeFile();

            var internalParserContext = new InternalParserContext(text, codeFile, _mainStorageContext);

            var parser = new SourceCodeParser(internalParserContext);
            parser.Run();

            var result = parser.Result;

            Assert.AreEqual(1, result.Count);

            var firstItem = result.Single();

            return firstItem;
        }
    }
}

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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, true, 0, 150, false);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, false);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, true, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.LFunction);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 5"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 10"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.SFunction);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_m = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 22"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.SFunction);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_m = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 22"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, false);

            var constraintItems = lingiusticVariable.Constraint.Items.ToList();

            Assert.AreEqual(constraintItems.Count, 3);

            var item1 = constraintItems[0];
            Assert.AreEqual(item1.Kind, KindOfLinguisticVariableConstraintItem.Inheritance);
            Assert.AreEqual(item1.RelationName, null);

            var item2 = constraintItems[1];

            Assert.AreEqual(item2.Kind, KindOfLinguisticVariableConstraintItem.Relation);
            Assert.AreEqual(item2.RelationName, NameHelper.CreateName("age"));

            var item3 = constraintItems[2];

            Assert.AreEqual(item3.Kind, KindOfLinguisticVariableConstraintItem.Relation);
            Assert.AreEqual(item3.RelationName, NameHelper.CreateName("is"));

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, false);

            var constraintItems = lingiusticVariable.Constraint.Items.ToList();

            Assert.AreEqual(constraintItems.Count, 3);

            var item1 = constraintItems[0];
            Assert.AreEqual(item1.Kind, KindOfLinguisticVariableConstraintItem.Inheritance);
            Assert.AreEqual(item1.RelationName, null);

            var item2 = constraintItems[1];

            Assert.AreEqual(item2.Kind, KindOfLinguisticVariableConstraintItem.Relation);
            Assert.AreEqual(item2.RelationName, NameHelper.CreateName("age"));

            var item3 = constraintItems[2];

            Assert.AreEqual(item3.Kind, KindOfLinguisticVariableConstraintItem.Relation);
            Assert.AreEqual(item3.RelationName, NameHelper.CreateName("is"));

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, false);

            var constraintItems = lingiusticVariable.Constraint.Items.ToList();

            Assert.AreEqual(constraintItems.Count, 2);

            var item1 = constraintItems[0];
            Assert.AreEqual(item1.Kind, KindOfLinguisticVariableConstraintItem.Inheritance);
            Assert.AreEqual(item1.RelationName, null);

            var item2 = constraintItems[1];

            Assert.AreEqual(item2.Kind, KindOfLinguisticVariableConstraintItem.Relation);
            Assert.AreEqual(item2.RelationName, NameHelper.CreateName("is"));

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, false);

            var constraintItems = lingiusticVariable.Constraint.Items.ToList();

            Assert.AreEqual(constraintItems.Count, 2);

            var item1 = constraintItems[0];
            Assert.AreEqual(item1.Kind, KindOfLinguisticVariableConstraintItem.Inheritance);
            Assert.AreEqual(item1.RelationName, null);

            var item2 = constraintItems[1];

            Assert.AreEqual(item2.Kind, KindOfLinguisticVariableConstraintItem.Relation);
            Assert.AreEqual(item2.RelationName, NameHelper.CreateName("is"));

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, false);

            var constraintItems = lingiusticVariable.Constraint.Items.ToList();

            Assert.AreEqual(constraintItems.Count, 1);

            var item1 = constraintItems[0];

            Assert.AreEqual(item1.Kind, KindOfLinguisticVariableConstraintItem.Relation);
            Assert.AreEqual(item1.RelationName, NameHelper.CreateName("age"));

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, false);

            var constraintItems = lingiusticVariable.Constraint.Items.ToList();

            Assert.AreEqual(constraintItems.Count, 1);

            var item1 = constraintItems[0];

            Assert.AreEqual(item1.Kind, KindOfLinguisticVariableConstraintItem.Relation);
            Assert.AreEqual(item1.RelationName, NameHelper.CreateName("age"));

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            var handler = term.Handler;

            Assert.AreNotEqual(handler, null);

            Assert.AreEqual(handler.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr = handler.ToString();

            Assert.AreEqual(handlerStr.Contains("_a = 10"), true);
            Assert.AreEqual(handlerStr.Contains("_b = 12"), true);
            Assert.AreEqual(handlerStr.Contains("_c = 17"), true);
            Assert.AreEqual(handlerStr.Contains("_d = 20"), true);
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

            var lingiusticVariable = codeEntity.AsLinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("logic"));

            CheckRange(lingiusticVariable.Range, true, 0, 1, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, false);

            var constraintItems = lingiusticVariable.Constraint.Items.ToList();

            Assert.AreEqual(constraintItems.Count, 2);

            var item1 = constraintItems[0];
            Assert.AreEqual(item1.Kind, KindOfLinguisticVariableConstraintItem.Inheritance);
            Assert.AreEqual(item1.RelationName, null);

            var item2 = constraintItems[1];

            Assert.AreEqual(item2.Kind, KindOfLinguisticVariableConstraintItem.Relation);
            Assert.AreEqual(item2.RelationName, NameHelper.CreateName("is"));

            var termsList = lingiusticVariable.Values;

            Assert.AreEqual(termsList.Count, 5);

            var term1 = termsList[0];

            Assert.AreEqual(term1.Name, NameHelper.CreateName("minimal"));
            Assert.AreEqual(term1.Parent, lingiusticVariable);

            var handler1 = term1.Handler;

            Assert.AreNotEqual(handler1, null);

            Assert.AreEqual(handler1.Kind, KindOfFuzzyLogicMemberFunction.LFunction);

            var handlerStr1 = handler1.ToString();

            Assert.AreEqual(handlerStr1.Contains("_a = 0"), true);
            Assert.AreEqual(handlerStr1.Contains("_b = 0.1"), true);

            var term2 = termsList[1];

            Assert.AreEqual(term2.Name, NameHelper.CreateName("low"));
            Assert.AreEqual(term2.Parent, lingiusticVariable);

            var handler2 = term2.Handler;

            Assert.AreNotEqual(handler2, null);

            Assert.AreEqual(handler2.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr2 = handler2.ToString();

            Assert.AreEqual(handlerStr2.Contains("_a = 0"), true);
            Assert.AreEqual(handlerStr2.Contains("_b = 0.05"), true);
            Assert.AreEqual(handlerStr2.Contains("_c = 0.3"), true);
            Assert.AreEqual(handlerStr2.Contains("_d = 0.45"), true);

            var term3 = termsList[2];

            Assert.AreEqual(term3.Name, NameHelper.CreateName("middle"));
            Assert.AreEqual(term3.Parent, lingiusticVariable);

            var handler3 = term3.Handler;

            Assert.AreNotEqual(handler3, null);

            Assert.AreEqual(handler3.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr3 = handler3.ToString();

            Assert.AreEqual(handlerStr3.Contains("_a = 0.3"), true);
            Assert.AreEqual(handlerStr3.Contains("_b = 0.4"), true);
            Assert.AreEqual(handlerStr3.Contains("_c = 0.6"), true);
            Assert.AreEqual(handlerStr3.Contains("_d = 0.7"), true);

            var term4 = termsList[3];

            Assert.AreEqual(term4.Name, NameHelper.CreateName("high"));
            Assert.AreEqual(term4.Parent, lingiusticVariable);

            var handler4 = term4.Handler;

            Assert.AreNotEqual(handler4, null);

            Assert.AreEqual(handler4.Kind, KindOfFuzzyLogicMemberFunction.Trapezoid);

            var handlerStr4 = handler4.ToString();

            Assert.AreEqual(handlerStr4.Contains("_a = 0.55"), true);
            Assert.AreEqual(handlerStr4.Contains("_b = 0.7"), true);
            Assert.AreEqual(handlerStr4.Contains("_c = 0.95"), true);
            Assert.AreEqual(handlerStr4.Contains("_d = 1"), true);

            var term5 = termsList[4];

            Assert.AreEqual(term5.Name, NameHelper.CreateName("maximal"));
            Assert.AreEqual(term5.Parent, lingiusticVariable);

            var handler5 = term5.Handler;

            Assert.AreNotEqual(handler5, null);

            Assert.AreEqual(handler5.Kind, KindOfFuzzyLogicMemberFunction.SFunction);

            var handlerStr5 = handler5.ToString();

            Assert.AreEqual(handlerStr5.Contains("_a = 0.9"), true);
            Assert.AreEqual(handlerStr5.Contains("_m = 0.95"), true);
            Assert.AreEqual(handlerStr5.Contains("_b = 1"), true);
        }

        private void CheckCodeEntity(CodeItem codeEntity, string name)
        {
            Assert.AreEqual(codeEntity.Kind, KindOfCodeEntity.LinguisticVariable);
            Assert.AreEqual(codeEntity.Name, NameHelper.CreateName(name));
            Assert.AreNotEqual(codeEntity.AsLinguisticVariable, null);
            Assert.AreEqual(codeEntity.InheritanceItems.Count, 0);
        }

        private void CheckRange(RangeValue rangeValue)
        {
            Assert.AreEqual(rangeValue.LeftBoundary, null);
            Assert.AreEqual(rangeValue.RightBoundary, null);
        }

        private void CheckRange(RangeValue rangeValue, bool leftBoundaryIncludes, double leftValue, double rightValue, bool rightValueIncludes)
        {
            Assert.AreEqual(rangeValue.LeftBoundary.Includes, leftBoundaryIncludes);
            Assert.AreEqual(rangeValue.LeftBoundary.Value.GetSystemValue(), leftValue);
            Assert.AreEqual(rangeValue.RightBoundary.Value.GetSystemValue(), rightValue);
            Assert.AreEqual(rangeValue.RightBoundary.Includes, rightValueIncludes);
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

            Assert.AreEqual(result.Count, 1);

            var firstItem = result.Single();

            return firstItem;
        }
    }
}

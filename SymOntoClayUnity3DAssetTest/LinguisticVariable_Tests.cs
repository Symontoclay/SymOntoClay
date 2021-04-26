using NUnit.Framework;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class LinguisticVariable_Tests
    {
        [Test]
        public void Case1()
        {
            var text = @"linvar age for range (0, 150]
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var lingiusticVariable = codeEntity.LinguisticVariable;

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

            throw new NotImplementedException();
        }

        [Test]
        public void Case1_a()
        {
            var text = @"linvar age for range [0, 150)
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, true, 0, 150, false);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            throw new NotImplementedException();
        }

        [Test]
        public void Case1_b()
        {
            var text = @"linvar age for range (0, 150)
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, false);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            throw new NotImplementedException();
        }

        [Test]
        public void Case1_c()
        {
            var text = @"linvar age for range [0, 150]
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, true, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            var term = lingiusticVariable.Values.Single();

            Assert.AreEqual(term.Name, NameHelper.CreateName("teenager"));
            Assert.AreEqual(term.Parent, lingiusticVariable);

            throw new NotImplementedException();
        }

        [Test]
        public void Case2()
        {
            var text = @"linvar age for range (-∞, +∞)
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            throw new NotImplementedException();
        }

        [Test]
        public void Case3()
        {
            var text = @"linvar age for range (*, *)
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            throw new NotImplementedException();
        }

        [Test]
        public void Case4()
        {
            var text = @"linvar age
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            throw new NotImplementedException();
        }

        [Test]
        public void Case5()
        {
            var text = @"linvar age
{
    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            throw new NotImplementedException();
        }

        [Test]
        public void Case6()
        {
            var text = @"linvar age for range (0, 150]
{
    `teenager` = Trapezoid(10, 12, 17, 20);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            throw new NotImplementedException();
        }

        [Test]
        public void Case7()
        {
            var text = @"linvar age for range (0, 150]
{
	`teenager` = L(5, 10);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            throw new NotImplementedException();
        }

        [Test]
        public void Case8()
        {
            var text = @"linvar age for range (0, 150]
{
    `teenager` = S(12, 22);

}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            throw new NotImplementedException();
        }

        [Test]
        public void Case9()
        {
            var text = @"linvar age for range (0, 150]
{
    `teenager` = S(12, 17, 22);
}";

            var codeEntity = Parse(text);

            CheckCodeEntity(codeEntity, "age");

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            Assert.AreEqual(lingiusticVariable.Constraint.IsEmpty, true);

            throw new NotImplementedException();
        }

        [Test]
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

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            throw new NotImplementedException();
        }

        [Test]
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

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            throw new NotImplementedException();
        }

        [Test]
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

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            throw new NotImplementedException();
        }

        [Test]
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

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            throw new NotImplementedException();
        }

        [Test]
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

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            throw new NotImplementedException();
        }

        [Test]
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

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("age"));

            CheckRange(lingiusticVariable.Range, false, 0, 150, true);

            throw new NotImplementedException();
        }

        [Test]
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

            var lingiusticVariable = codeEntity.LinguisticVariable;

            lingiusticVariable.CheckDirty();

            Assert.AreEqual(lingiusticVariable.Name, NameHelper.CreateName("logic"));

            CheckRange(lingiusticVariable.Range, true, 0, 1, true);

            throw new NotImplementedException();
        }

        private void CheckCodeEntity(CodeEntity codeEntity, string name)
        {
            Assert.AreEqual(codeEntity.Kind, KindOfCodeEntity.LinguisticVariable);
            Assert.AreEqual(codeEntity.Name, NameHelper.CreateName(name));
            Assert.AreNotEqual(codeEntity.LinguisticVariable, null);
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

        private CodeEntity Parse(string text)
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

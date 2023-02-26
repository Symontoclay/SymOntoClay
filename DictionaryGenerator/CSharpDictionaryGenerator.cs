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

using NLog;
using SymOntoClay.NLP.CommonDict;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SymOntoClay.CoreHelper.DebugHelpers;
using System.Globalization;

namespace DictionaryGenerator
{
    public class CSharpDictionaryGenerator
    {
#if DEBUG
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public CSharpDictionaryGenerator(string outputFileName, string className, Dictionary<string, List<BaseGrammaticalWordFrame>> source)
        {
            _outputFileName = outputFileName;
            _source = source;

            _targetUnit = new CodeCompileUnit();

            var targetNamespace = new CodeNamespace("SymOntoClay.NLP.BinaryDict");
            targetNamespace.Imports.Add(new CodeNamespaceImport("SymOntoClay.NLP.CommonDict"));
            targetNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            var targetClass = new CodeTypeDeclaration(className);
            targetClass.IsClass = true;
            targetClass.TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;
            targetNamespace.Types.Add(targetClass);
            _targetUnit.Namespaces.Add(targetNamespace);

            targetClass.BaseTypes.Add("IWordsDict");

            _constructor = new CodeConstructor();
            _constructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            targetClass.Members.Add(_constructor);

            var wordFramesDictField = new CodeMemberField();
            wordFramesDictField.Attributes = MemberAttributes.Private;
            wordFramesDictField.Name = "_wordFramesDict";
            wordFramesDictField.Type = new CodeTypeReference("Dictionary<string, List<BaseGrammaticalWordFrame>>");

            targetClass.Members.Add(wordFramesDictField);

            var getWordFramesMethod = new CodeMemberMethod();
            getWordFramesMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            getWordFramesMethod.Name = "GetWordFrames";
            getWordFramesMethod.ReturnType = new CodeTypeReference("IList<BaseGrammaticalWordFrame>");
            getWordFramesMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "word"));
            targetClass.Members.Add(getWordFramesMethod);

            var conditionalStatement = new CodeConditionStatement();
            conditionalStatement.Condition = new CodeSnippetExpression("_wordFramesDict.ContainsKey(word)");
            conditionalStatement.TrueStatements.Add(new CodeSnippetExpression("return _wordFramesDict[word]"));
            getWordFramesMethod.Statements.Add(conditionalStatement);

            var returnStatement = new CodeMethodReturnStatement(new CodePrimitiveExpression(null));
            getWordFramesMethod.Statements.Add(returnStatement);
        }

        private readonly string _outputFileName;
        private readonly Dictionary<string, List<BaseGrammaticalWordFrame>> _source;
        private CodeCompileUnit _targetUnit;
        private CodeConstructor _constructor;

        public void Run()
        {
#if DEBUG
            _logger.Info("Begin");
#endif

            _constructor.Statements.Add(new CodeSnippetExpression("_wordFramesDict = new Dictionary<string, List<BaseGrammaticalWordFrame>>()"));
            _constructor.Statements.Add(new CodeSnippetExpression("List<BaseGrammaticalWordFrame> framesList = null"));

            foreach(var sourceItem in _source)
            {
                GenerateSourceItem(sourceItem.Key, sourceItem.Value);
            }

            GenerateCSharpCode();

#if DEBUG
            _logger.Info("End");
#endif
        }

        private void GenerateSourceItem(string word, List<BaseGrammaticalWordFrame> grammaticalWordFrames)
        {
#if DEBUG
            //_logger.Info($"word = {word}");
            //_logger.Info($"grammaticalWordFrames = {grammaticalWordFrames.WriteListToString()}");
#endif

            var statementsList = _constructor.Statements;

            statementsList.Add(new CodeSnippetExpression("framesList = new List<BaseGrammaticalWordFrame>()"));
            statementsList.Add(new CodeSnippetExpression($"_wordFramesDict[\"{word}\"] = framesList"));

            foreach(var grammaticalWordFrame in grammaticalWordFrames)
            {
                var partOfSpeech = grammaticalWordFrame.PartOfSpeech;

                switch(partOfSpeech)
                {
                    case GrammaticalPartOfSpeech.Noun:
                        GenerateNoun(grammaticalWordFrame.AsNoun);
                        break;

                    case GrammaticalPartOfSpeech.Pronoun:
                        GeneratePronoun(grammaticalWordFrame.AsPronoun);
                        break;

                    case GrammaticalPartOfSpeech.Adjective:
                        GenerateAdjective(grammaticalWordFrame.AsAdjective);
                        break;

                    case GrammaticalPartOfSpeech.Verb:
                        GenerateVerb(grammaticalWordFrame.AsVerb);
                        break;

                    case GrammaticalPartOfSpeech.Adverb:
                        GenerateAdverb(grammaticalWordFrame.AsAdverb);
                        break;

                    case GrammaticalPartOfSpeech.Preposition:
                        GeneratePreposition(grammaticalWordFrame.AsPreposition);
                        break;

                    case GrammaticalPartOfSpeech.Postposition:
                        GeneratePostposition(grammaticalWordFrame.AsPostposition);
                        break;

                    case GrammaticalPartOfSpeech.Conjunction:
                        GenerateConjunction(grammaticalWordFrame.AsConjunction);
                        break;

                    case GrammaticalPartOfSpeech.Interjection:
                        GenerateInterjection(grammaticalWordFrame.AsInterjection);
                        break;

                    case GrammaticalPartOfSpeech.Article:
                        GenerateArticle(grammaticalWordFrame.AsArticle);
                        break;

                    case GrammaticalPartOfSpeech.Numeral:
                        GenerateNumeral(grammaticalWordFrame.AsNumeral);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(partOfSpeech), partOfSpeech, null);
                }
            }
        }

        private void GenerateNoun(NounGrammaticalWordFrame frame)
        {
#if DEBUG
            //_logger.Info($"frame = {frame}");
#endif

            var propertiesList = new List<string>();

            propertiesList.Add($"IsName = {frame.IsName.ToString().ToLower()}");
            propertiesList.Add($"IsShortForm = {frame.IsShortForm.ToString().ToLower()}");
            propertiesList.Add($"Gender = GrammaticalGender.{frame.Gender}");
            propertiesList.Add($"Number = GrammaticalNumberOfWord.{frame.Number}");
            propertiesList.Add($"IsCountable = {frame.IsCountable.ToString().ToLower()}");
            propertiesList.Add($"IsGerund = {frame.IsGerund.ToString().ToLower()}");
            propertiesList.Add($"IsPossessive = {frame.IsPossessive.ToString().ToLower()}");

            AddBaseGrammaticalWordFrameProps(frame, propertiesList);

            var sb = new StringBuilder();
            sb.AppendLine("framesList.Add(new NounGrammaticalWordFrame(){");
            sb.AppendLine(string.Join(",\n", propertiesList));
            sb.Append("})");
#if DEBUG
            //_logger.Info($"sb = {sb}");
#endif

            _constructor.Statements.Add(new CodeSnippetExpression(sb.ToString()));
        }

        private void GeneratePronoun(PronounGrammaticalWordFrame frame)
        {
#if DEBUG
            //_logger.Info($"frame = {frame}");
#endif

            var propertiesList = new List<string>();

            propertiesList.Add($"Gender = GrammaticalGender.{frame.Gender}");
            propertiesList.Add($"Number = GrammaticalNumberOfWord.{frame.Number}");
            propertiesList.Add($"Person = GrammaticalPerson.{frame.Person}");
            propertiesList.Add($"TypeOfPronoun = TypeOfPronoun.{frame.TypeOfPronoun}");
            propertiesList.Add($"Case = CaseOfPersonalPronoun.{frame.Case}");
            propertiesList.Add($"IsQuestionWord = {frame.IsQuestionWord.ToString().ToLower()}");
            propertiesList.Add($"IsNegation = {frame.IsNegation.ToString().ToLower()}");

            AddBaseGrammaticalWordFrameProps(frame, propertiesList);

            var sb = new StringBuilder();
            sb.AppendLine("framesList.Add(new PronounGrammaticalWordFrame(){");
            sb.AppendLine(string.Join(",\n", propertiesList));
            sb.Append("})");
#if DEBUG
            //_logger.Info($"sb = {sb}");
#endif

            _constructor.Statements.Add(new CodeSnippetExpression(sb.ToString()));
        }

        private void GenerateAdjective(AdjectiveGrammaticalWordFrame frame)
        {
#if DEBUG
            //_logger.Info($"frame = {frame}");
#endif

            var propertiesList = new List<string>();

            propertiesList.Add($"Comparison = GrammaticalComparison.{frame.Comparison}");
            propertiesList.Add($"IsDeterminer = {frame.IsDeterminer.ToString().ToLower()}");

            AddBaseGrammaticalWordFrameProps(frame, propertiesList);

            var sb = new StringBuilder();
            sb.AppendLine("framesList.Add(new AdjectiveGrammaticalWordFrame(){");
            sb.AppendLine(string.Join(",\n", propertiesList));
            sb.Append("})");
#if DEBUG
            //_logger.Info($"sb = {sb}");
#endif

            _constructor.Statements.Add(new CodeSnippetExpression(sb.ToString()));
        }

        private void GenerateVerb(VerbGrammaticalWordFrame frame)
        {
#if DEBUG
            //_logger.Info($"frame = {frame}");
#endif

            var propertiesList = new List<string>();

            propertiesList.Add($"VerbType = VerbType.{frame.VerbType}");
            propertiesList.Add($"Number = GrammaticalNumberOfWord.{frame.Number}");
            propertiesList.Add($"Person = GrammaticalPerson.{frame.Person}");
            propertiesList.Add($"Tense = GrammaticalTenses.{frame.Tense}");
            propertiesList.Add($"IsModal = {frame.IsModal.ToString().ToLower()}");
            propertiesList.Add($"IsFormOfToBe = {frame.IsFormOfToBe.ToString().ToLower()}");
            propertiesList.Add($"IsFormOfToHave = {frame.IsFormOfToHave.ToString().ToLower()}");
            propertiesList.Add($"IsFormOfToDo = {frame.IsFormOfToDo.ToString().ToLower()}");
            propertiesList.Add($"MayHaveGerundOrInfinitiveAfterSelf = {frame.MayHaveGerundOrInfinitiveAfterSelf.ToString().ToLower()}");

            AddBaseGrammaticalWordFrameProps(frame, propertiesList);

            var sb = new StringBuilder();
            sb.AppendLine("framesList.Add(new VerbGrammaticalWordFrame(){");
            sb.AppendLine(string.Join(",\n", propertiesList));
            sb.Append("})");
#if DEBUG
            //_logger.Info($"sb = {sb}");
#endif

            _constructor.Statements.Add(new CodeSnippetExpression(sb.ToString()));
        }

        private void GenerateAdverb(AdverbGrammaticalWordFrame frame)
        {
#if DEBUG
            //_logger.Info($"frame = {frame}");
#endif

            var propertiesList = new List<string>();

            propertiesList.Add($"Comparison = GrammaticalComparison.{frame.Comparison}");
            propertiesList.Add($"IsQuestionWord = {frame.IsQuestionWord.ToString().ToLower()}");
            propertiesList.Add($"IsDeterminer = {frame.IsDeterminer.ToString().ToLower()}");
            propertiesList.Add($"IsNegation = {frame.IsNegation.ToString().ToLower()}");

            AddBaseGrammaticalWordFrameProps(frame, propertiesList);

            var sb = new StringBuilder();
            sb.AppendLine("framesList.Add(new AdverbGrammaticalWordFrame(){");
            sb.AppendLine(string.Join(",\n", propertiesList));
            sb.Append("})");
#if DEBUG
            //_logger.Info($"sb = {sb}");
#endif

            _constructor.Statements.Add(new CodeSnippetExpression(sb.ToString()));
        }

        private void GeneratePreposition(PrepositionGrammaticalWordFrame frame)
        {
#if DEBUG
            //_logger.Info($"frame = {frame}");
#endif

            var propertiesList = new List<string>();

            propertiesList.Add($"Comparison = GrammaticalComparison.{frame.Comparison}");

            AddBaseGrammaticalWordFrameProps(frame, propertiesList);

            var sb = new StringBuilder();
            sb.AppendLine("framesList.Add(new PrepositionGrammaticalWordFrame(){");
            sb.AppendLine(string.Join(",\n", propertiesList));
            sb.Append("})");
#if DEBUG
            //_logger.Info($"sb = {sb}");
#endif

            _constructor.Statements.Add(new CodeSnippetExpression(sb.ToString()));
        }

        private void GeneratePostposition(PostpositionGrammaticalWordFrame frame)
        {
#if DEBUG
            //_logger.Info($"frame = {frame}");
#endif

            var propertiesList = new List<string>();

            AddBaseGrammaticalWordFrameProps(frame, propertiesList);

            var sb = new StringBuilder();
            sb.AppendLine("framesList.Add(new PostpositionGrammaticalWordFrame(){");
            sb.AppendLine(string.Join(",\n", propertiesList));
            sb.Append("})");
#if DEBUG
            //_logger.Info($"sb = {sb}");
#endif

            _constructor.Statements.Add(new CodeSnippetExpression(sb.ToString()));
        }

        private void GenerateConjunction(ConjunctionGrammaticalWordFrame frame)
        {
#if DEBUG
            //_logger.Info($"frame = {frame}");
#endif

            var propertiesList = new List<string>();

            propertiesList.Add($"Kind = KindOfConjunction.{frame.Kind}");
            propertiesList.Add($"SecondKind = SecondKindOfConjunction.{frame.SecondKind}");

            propertiesList.Add($"IsQuestionWord = {frame.IsQuestionWord.ToString().ToLower()}");
            propertiesList.Add($"IsNegation = {frame.IsNegation.ToString().ToLower()}");

            AddBaseGrammaticalWordFrameProps(frame, propertiesList);

            var sb = new StringBuilder();
            sb.AppendLine("framesList.Add(new ConjunctionGrammaticalWordFrame(){");
            sb.AppendLine(string.Join(",\n", propertiesList));
            sb.Append("})");
#if DEBUG
            //_logger.Info($"sb = {sb}");
#endif

            _constructor.Statements.Add(new CodeSnippetExpression(sb.ToString()));
        }

        private void GenerateInterjection(InterjectionGrammaticalWordFrame frame)
        {
#if DEBUG
            //_logger.Info($"frame = {frame}");
#endif

            var propertiesList = new List<string>();

            AddBaseGrammaticalWordFrameProps(frame, propertiesList);

            var sb = new StringBuilder();
            sb.AppendLine("framesList.Add(new InterjectionGrammaticalWordFrame(){");
            sb.AppendLine(string.Join(",\n", propertiesList));
            sb.Append("})");
#if DEBUG
            //_logger.Info($"sb = {sb}");
#endif

            _constructor.Statements.Add(new CodeSnippetExpression(sb.ToString()));
        }

        private void GenerateArticle(ArticleGrammaticalWordFrame frame)
        {
#if DEBUG
            //_logger.Info($"frame = {frame}");
#endif

            var propertiesList = new List<string>();

            propertiesList.Add($"Number = GrammaticalNumberOfWord.{frame.Number}");
            propertiesList.Add($"Kind = KindOfArticle.{frame.Kind}");

            AddBaseGrammaticalWordFrameProps(frame, propertiesList);

            var sb = new StringBuilder();
            sb.AppendLine("framesList.Add(new ArticleGrammaticalWordFrame(){");
            sb.AppendLine(string.Join(",\n", propertiesList));
            sb.Append("})");
#if DEBUG
            //_logger.Info($"sb = {sb}");
#endif

            _constructor.Statements.Add(new CodeSnippetExpression(sb.ToString()));
        }

        private void GenerateNumeral(NumeralGrammaticalWordFrame frame)
        {
#if DEBUG
            //_logger.Info($"frame = {frame}");
#endif

            var propertiesList = new List<string>();

            propertiesList.Add($"NumeralType = NumeralType.{frame.NumeralType}");

            if(frame.RepresentedNumber.HasValue)
            {
                propertiesList.Add($"RepresentedNumber = {frame.RepresentedNumber.Value.ToString(CultureInfo.InvariantCulture)}F");
            }

            AddBaseGrammaticalWordFrameProps(frame, propertiesList);

            var sb = new StringBuilder();
            sb.AppendLine("framesList.Add(new NumeralGrammaticalWordFrame(){");
            sb.AppendLine(string.Join(",\n", propertiesList));
            sb.Append("})");
#if DEBUG
            //_logger.Info($"sb = {sb}");
#endif

            _constructor.Statements.Add(new CodeSnippetExpression(sb.ToString()));
        }

        private void AddBaseGrammaticalWordFrameProps(BaseGrammaticalWordFrame frame, List<string> propertiesList)
        {
            propertiesList.Add($"Word = \"{frame.Word}\"");
            propertiesList.Add($"RootWord = \"{frame.RootWord}\"");
            propertiesList.Add($"LogicalMeaning = {StringListToString(frame.LogicalMeaning)}");
            propertiesList.Add($"FullLogicalMeaning = {StringListToString(frame.FullLogicalMeaning)}");
            propertiesList.Add($"ConditionalLogicalMeaning = {ConditionalLogicalMeaningToString(frame.ConditionalLogicalMeaning)}");

            propertiesList.Add($"IsArchaic = {frame.IsArchaic.ToString().ToLower()}");
            propertiesList.Add($"IsDialectal = {frame.IsDialectal.ToString().ToLower()}");
            propertiesList.Add($"IsPoetic = {frame.IsPoetic.ToString().ToLower()}");
            propertiesList.Add($"IsAbbreviation = {frame.IsAbbreviation.ToString().ToLower()}");
            propertiesList.Add($"IsRare = {frame.IsRare.ToString().ToLower()}");

            /*
            propertiesList.Add($" = {frame.}");
            propertiesList.Add($" = \"{frame.}\"");
            propertiesList.Add($" = {frame..ToString().ToLower()}");
            */
        }

        private string ConditionalLogicalMeaningToString(IDictionary<string, IList<string>> dict)
        {
            if (dict == null)
            {
                return "null";
            }

            if (!dict.Any())
            {
                return "new Dictionary<string, IList<string>>()";
            }

            var itemsList = new List<string>();

            foreach(var kvpItem in dict)
            {
                itemsList.Add($"{{\"{kvpItem.Key}\", {StringListToString(kvpItem.Value)} }}");
            }

            var sb = new StringBuilder();
            sb.Append("new Dictionary<string, IList<string>>() {");
            sb.Append(string.Join(',', itemsList));
            sb.Append("}");

#if DEBUG
            //_logger.Info($"sb = {sb}");
#endif

            return sb.ToString();
        }

        private string StringListToString(IList<string> list)
        {
            if (list == null)
            {
                return "null";
            }

            if(!list.Any())
            {
                return "new List<string>()";
            }

            var sb = new StringBuilder();
            sb.Append("new List<string>() {");
            sb.Append(string.Join(',', list.Select(p => $"\"{p}\"")));
            sb.Append("}");
            return sb.ToString();
        }

        private void GenerateCSharpCode()
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using var sourceWriter = new StreamWriter(_outputFileName);
            provider.GenerateCodeFromCompileUnit(
                _targetUnit, sourceWriter, options);
        }
    }
}

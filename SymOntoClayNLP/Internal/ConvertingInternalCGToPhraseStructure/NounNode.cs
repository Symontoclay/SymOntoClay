/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.Dot;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public class NounNode
    {
        public NounNode(BaseInternalConceptCGNode source, RoleOfNoun roleOfNoun, BaseContextOfConvertingInternalCGToPhraseStructure baseContext)
        {
            _baseContext = baseContext;
            _wordsDict = baseContext.WordsDict;
            _logger = baseContext.Logger;
            _nlpContext = baseContext.NLPContext;
            _source = source;
            _roleOfNoun = roleOfNoun;
            _visitedRelations = baseContext.VisitedRelations;
        }

        private readonly BaseContextOfConvertingInternalCGToPhraseStructure _baseContext;
        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;
        private readonly INLPConverterContext _nlpContext;

        private readonly BaseInternalConceptCGNode _source;
        private readonly RoleOfNoun _roleOfNoun;
        private readonly List<InternalRelationCGNode> _visitedRelations;

        public ResultOfNode Run()
        {
#if DEBUG
            //_logger.Log($"_source = {_source}");
#endif

            var kind = _source.Kind;

#if DEBUG
            //_logger.Log($"kind = {kind}");
#endif

            switch(kind)
            {
                case KindOfCGNode.Concept:
                    return ProcessConcept();

                case KindOfCGNode.Graph:
                    if(_source.AsConceptualGraph.Children.Any(p => p.IsConceptNode && p.AsConceptNode.IsRootConceptOfEntitiCondition))
                    {
                        return ProcessConditionalEntity();
                    }
                    throw new NotImplementedException();

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private ResultOfNode ProcessConcept()
        {
            var conceptName = _source.Name;

#if DEBUG
            _logger.Log($"conceptName = '{conceptName}'");
#endif

            var mood = _source.Parent.Mood;

#if DEBUG
            _logger.Log($"mood = {mood}");
#endif

            var nounWordNode = new NounWordNode(conceptName, _roleOfNoun, _logger, _wordsDict, mood);

            if(_roleOfNoun == RoleOfNoun.PossessDeterminer)
            {
                return new ResultOfNode()
                {
                    SentenceItem = nounWordNode.GetWord()
                };
            }

            var nounPhrase = nounWordNode.GetNounPhrase();

#if DEBUG
            _logger.Log($"nounPhrase = {nounPhrase}");
#endif

            return new ResultOfNode()
            {
                SentenceItem = nounPhrase
            };
        }

        private ResultOfNode ProcessConditionalEntity()
        {
#if DEBUG
            //var dotStr = DotConverter.ConvertToString(_source);
            //_logger.Log($"dotStr = '{dotStr}'");
#endif

            var conditionalEntityNode = new ConditionalEntityNode(_source.AsConceptualGraph, _baseContext);
            var conditionalEntityNodeResult = conditionalEntityNode.Run();

#if DEBUG
            //_logger.Log($"conditionalEntityNodeResult = {conditionalEntityNodeResult}");
#endif

            return conditionalEntityNodeResult;
        }
    }
}

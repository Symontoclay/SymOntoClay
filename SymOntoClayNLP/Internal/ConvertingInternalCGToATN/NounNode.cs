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

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToATN
{
    public class NounNode
    {
        public NounNode(BaseInternalConceptCGNode source, List<string> disabledRelations, RoleOfNoun roleOfNoun, ContextOfConvertingInternalCGToText context)
        {
            _context = context;
            _wordsDict = context.WordsDict;
            _logger = context.Logger;
            _source = source;
            _disabledRelations = disabledRelations;
            _roleOfNoun = roleOfNoun;
        }

        private readonly ContextOfConvertingInternalCGToText _context;
        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;

        private readonly BaseInternalConceptCGNode _source;
        private readonly List<string> _disabledRelations;
        private readonly RoleOfNoun _roleOfNoun;

        public ResultOfNode Run()
        {
#if DEBUG
            _logger.Log($"_source = {_source}");
#endif

            var kind = _source.Kind;

#if DEBUG
            _logger.Log($"kind = {kind}");
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

            var wordFramesList = _wordsDict.GetWordFrames(conceptName);

#if DEBUG
            _logger.Log($"wordFramesList = {wordFramesList.WriteListToString()}");
#endif

            switch(_roleOfNoun)
            {
                case RoleOfNoun.Subject:
                    {
                        var pronounsList = wordFramesList.Where(p => p.IsPronoun).Select(p => p.AsPronoun).Where(p => p.Case == CaseOfPersonalPronoun.Subject).ToList();

                        if(pronounsList.Any())
                        {
#if DEBUG
                            _logger.Log($"pronounsList = {pronounsList.WriteListToString()}");
#endif

                            var nounPhrase = new NounPhrase();

                            var word = new Word();
                            nounPhrase.N = word;

                            word.Content = conceptName;

                            word.WordFrame = pronounsList.Single();

#if DEBUG
                            _logger.Log($"nounPhrase = {nounPhrase}");
#endif

                            return new ResultOfNode()
                            {
                                SentenceItem = nounPhrase
                            };
                        }

                        throw new NotImplementedException();
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(_roleOfNoun), _roleOfNoun, null);
            }
        }

        private ResultOfNode ProcessConditionalEntity()
        {
#if DEBUG
            var dotStr = DotConverter.ConvertToString(_source);
            _logger.Log($"dotStr = '{dotStr}'");
#endif

            var conditionalEntityNode = new ConditionalEntityNode(_source.AsConceptualGraph, _logger, _context.WordsDict, _context.NLPContext);
            var conditionalEntityNodeResult = conditionalEntityNode.Run();

#if DEBUG
            _logger.Log($"conditionalEntityNodeResult = {conditionalEntityNodeResult}");
#endif

            throw new NotImplementedException();
        }
    }
}

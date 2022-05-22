using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToATN
{
    public class VerbNode
    {
        public VerbNode(InternalConceptCGNode source, List<string> disabledRelations, BaseSentenceItem subject, ContextOfConvertingInternalCGToText context)
        {
            _context = context;
            _wordsDict = context.WordsDict;
            _logger = context.Logger;
            _source = source;
            _disabledRelations = disabledRelations;
            _subject = subject;
        }

        private readonly ContextOfConvertingInternalCGToText _context;
        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;

        private readonly InternalConceptCGNode _source;
        private readonly List<string> _disabledRelations;
        private readonly BaseSentenceItem _subject;

        public ResultOfNode Run()
        {
#if DEBUG
            _logger.Log($"_source = {_source}");
#endif

            var result = new ResultOfNode();

            var sb = new StringBuilder();

            var verbText = GetVerb();

#if DEBUG
            _logger.Log($"verbText = '{verbText}'");
#endif

            sb.Append(verbText);

            var objectText = GetObjectText(result);

#if DEBUG
            _logger.Log($"objectText = '{objectText}'");
#endif

            if(!string.IsNullOrWhiteSpace(objectText))
            {
                sb.Append(" ");
                sb.Append(objectText);
            }

#if DEBUG
            _logger.Log($"sb = '{sb}'");
#endif

            throw new NotImplementedException();
        }

        private (VerbPhrase, VerbPhrase) GetVerb()
        {
            var voice = _context.Voice;

            switch (voice)
            {
                case GrammaticalVoice.Active:
                    return GetVerbForActiveVoice();

                default:
                    throw new ArgumentOutOfRangeException(nameof(voice), voice, null);
            }
        }

        private (VerbPhrase, VerbPhrase) GetVerbForActiveVoice()
        {
            var aspect = _context.Aspect;

            switch (aspect)
            {
                case GrammaticalAspect.Simple:
                    return GetVerbForSimpleAspectOfActiveVoice();

                default:
                    throw new ArgumentOutOfRangeException(nameof(aspect), aspect, null);
            }
        }

        private (VerbPhrase, VerbPhrase) GetVerbForSimpleAspectOfActiveVoice()
        {
            var verbName = _source.Name;

#if DEBUG
            _logger.Log($"verbName = '{verbName}'");
#endif

            var tense = _context.Tense;

            switch (tense)
            {
                case GrammaticalTenses.Present:
                    if (_context.IsNegation)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        return GetTargetPresentSimpleVerbForm(verbName);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(tense), tense, null);
            }

            throw new NotImplementedException();
        }

        private (VerbPhrase, VerbPhrase) GetTargetPresentSimpleVerbForm(string verb)
        {
#if DEBUG
            _logger.Log($"verb = '{verb}'");
            _logger.Log($"_subject = '{_subject}'");
#endif

            var subjectWordFrame = _subject.RootWordFrame;

            if ((subjectWordFrame.PartOfSpeech == GrammaticalPartOfSpeech.Noun) ||
                (subjectWordFrame.PartOfSpeech == GrammaticalPartOfSpeech.Pronoun && subjectWordFrame.AsPronoun.Number == GrammaticalNumberOfWord.Singular && subjectWordFrame.AsPronoun.Person == GrammaticalPerson.Third && subjectWordFrame.AsPronoun.Case == CaseOfPersonalPronoun.Subject))
            {
                //like -> likes
                throw new NotImplementedException();
            }

            throw new NotImplementedException();

            //var wordFramesList = _wordsDict.GetWordFrames(_subject);



            //return verb;
        }

        private string GetObjectText(ResultOfNode result)
        {
            var objectRelationsList = _source.Outputs.Where(p => p.Kind == KindOfCGNode.Relation && p.Name == SpecialNamesOfRelations.ObjectRelationName).Select(p => p.AsRelationNode).ToList();

            if (objectRelationsList.Count > 1)
            {
                throw new NotImplementedException();
            }

            var objectRelation = objectRelationsList.First();

#if DEBUG
            _logger.Log($"objectRelation = {objectRelation}");
#endif


            if (objectRelation.Outputs.Count != 1)
            {
                throw new NotImplementedException();
            }

            var objectConcept = objectRelation.Outputs.First();

#if DEBUG
            _logger.Log($"objectConcept = {objectConcept}");
#endif

            var objectNode = new NounNode(objectConcept.AsGraphOrConceptNode, new List<string>() { "object" }, RoleOfNoun.Object, _context);

            var objectResult = objectNode.Run();

#if DEBUG
            _logger.Log($"objectResult = {objectResult}");
#endif

            throw new NotImplementedException();
        }
    }
}

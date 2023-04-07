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

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public class VerbNode
    {
        public VerbNode(InternalConceptCGNode source, BaseSentenceItem subject, ContextOfConvertingInternalCGToPhraseStructure context)
        {
            _context = context;
            _wordsDict = context.WordsDict;
            _logger = context.Logger;
            _source = source;
            _subject = subject;
        }

        private readonly ContextOfConvertingInternalCGToPhraseStructure _context;
        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;

        private readonly InternalConceptCGNode _source;
        private readonly BaseSentenceItem _subject;

        public ResultOfNode Run()
        {
            var result = new ResultOfNode();

            var verbs = GetVerb();

            var objectPhrase = GetObjectPhrase(result);

            if(objectPhrase != null)
            {
                verbs.Item2.Object = objectPhrase;
            }

            var phrasesOfAnotherRelations = ProcessAnotherRelations();

            if(phrasesOfAnotherRelations.Any())
            {
                var phrasesOfAnotherRelationsDict = phrasesOfAnotherRelations.GroupBy(p => p.Item1).ToDictionary(p => p.Key, p => p.Select(x => x.Item2).ToList());

                var targetVerb = verbs.Item2;

                foreach (var kvpItem in phrasesOfAnotherRelationsDict)
                {
                    var target = kvpItem.Key;

                    var valuesList = kvpItem.Value;

                    switch (target)
                    {
                        case KindOfVerbPhraseTarget.PP:
                            {
                                if(valuesList.Count == 1)
                                {
                                    var targetPhrase = valuesList.Single().AsPreOrPostpositionalPhrase;

                                    targetVerb.PP = targetPhrase;
                                }
                                break;

                                throw new NotImplementedException();
                            }

                        default:
                            throw new ArgumentOutOfRangeException(nameof(target), target, null);
                    }
                }
            }

            return new ResultOfNode()
            {
                SentenceItem = verbs.Item1
            };
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
            if(_subject != null)
            {
                var subjectWordFrame = _subject.RootWordFrame;

                if ((subjectWordFrame.PartOfSpeech == GrammaticalPartOfSpeech.Noun) ||
                    (subjectWordFrame.PartOfSpeech == GrammaticalPartOfSpeech.Pronoun && subjectWordFrame.AsPronoun.Number == GrammaticalNumberOfWord.Singular && subjectWordFrame.AsPronoun.Person == GrammaticalPerson.Third && subjectWordFrame.AsPronoun.Case == CaseOfPersonalPronoun.Subject) || 
                    (subjectWordFrame.PartOfSpeech == GrammaticalPartOfSpeech.Pronoun && subjectWordFrame.AsPronoun.Number == GrammaticalNumberOfWord.Neuter && subjectWordFrame.AsPronoun.Person == GrammaticalPerson.Neuter && subjectWordFrame.AsPronoun.Case == CaseOfPersonalPronoun.Undefined))
                {

                    var verbsList = _wordsDict.GetWordFramesByRootWord(verb).Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Verb).Select(p => p.AsVerb).Where(p => p.Person == GrammaticalPerson.Third && p.Tense == GrammaticalTenses.Present);

                    var targetVerbFrame = verbsList.Single();

                    var verbPhrase = new VerbPhrase();

                    var word = new Word();
                    verbPhrase.V = word;

                    word.Content = targetVerbFrame.Word;

                    word.WordFrame = targetVerbFrame;

                    return (verbPhrase, verbPhrase);
                }
            }

            {
                var verbsList = _wordsDict.GetWordFramesByWord(verb).Where(p => p.PartOfSpeech == GrammaticalPartOfSpeech.Verb);

                var verbPhrase = new VerbPhrase();

                var word = new Word();
                verbPhrase.V = word;

                word.Content = verb;

                word.WordFrame = verbsList.Single();

                return (verbPhrase, verbPhrase);
            }
        }

        private BaseSentenceItem GetObjectPhrase(ResultOfNode result)
        {
            var objectRelationsList = _source.Outputs.Where(p => p.Kind == KindOfCGNode.Relation && p.Name == SpecialNamesOfRelations.ObjectRelationName).Select(p => p.AsRelationNode).ToList();

            if(!objectRelationsList.Any())
            {
                return null;
            }

            if (objectRelationsList.Count > 1)
            {
                throw new NotImplementedException();
            }

            var objectRelation = objectRelationsList.First();

            _context.VisitedRelations.Add(objectRelation);

            if (objectRelation.Outputs.Count != 1)
            {
                throw new NotImplementedException();
            }

            var objectConcept = objectRelation.Outputs.First();

            var objectNode = new NounNode(objectConcept.AsGraphOrConceptNode, RoleOfNoun.Object, _context);

            var objectResult = objectNode.Run();

            return objectResult.SentenceItem;
        }

        private List<(KindOfVerbPhraseTarget, BaseSentenceItem)> ProcessAnotherRelations()
        {
            var anotherRelationsList = _source.Outputs.Where(p => p.Kind == KindOfCGNode.Relation && p.Name != SpecialNamesOfRelations.ObjectRelationName && p.Name != SpecialNamesOfRelations.AgentRelationName && p.Name != SpecialNamesOfRelations.ExperiencerRelationName).Select(p => p.AsRelationNode).ToList();

            if(!anotherRelationsList.Any())
            {
                return new List<(KindOfVerbPhraseTarget, BaseSentenceItem)>();
            }

            var phrasesList = new List<(KindOfVerbPhraseTarget, BaseSentenceItem)>();

            foreach (var relation in anotherRelationsList)
            {
                var relationName = relation.Name;

                switch(relationName)
                {
                    case "direction":
                        {
                            var directionConcept = relation.Outputs.First().AsGraphOrConceptNode;

                            var nounNode = new NounNode(directionConcept, RoleOfNoun.AnotherRole, _context);

                            var nounResult = nounNode.Run();

                            var nounPhrase = nounResult.SentenceItem.AsNounPhrase;

                            if(nounPhrase == null)
                            {
                                throw new ArgumentNullException(nameof(nounPhrase));
                            }

                            var toWordFramesList = _wordsDict.GetWordFramesByWord("to").Where(p => p.IsPreposition).Select(p => p.AsPreposition);

                            var toPrepositionWordFrame = toWordFramesList.Single();

                            var pp = new PreOrPostpositionalPhrase()
                            {
                                P = new Word()
                                {
                                    Content = "to",
                                    WordFrame = toPrepositionWordFrame
                                },
                                NP = nounPhrase
                            };

                            phrasesList.Add((KindOfVerbPhraseTarget.PP, pp));
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(relationName), relationName, null);
                }
            }

            return phrasesList;
        }
    }
}

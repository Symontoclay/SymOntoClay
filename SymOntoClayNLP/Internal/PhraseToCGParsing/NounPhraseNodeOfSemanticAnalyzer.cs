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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Linq;

namespace SymOntoClay.NLP.Internal.PhraseToCGParsing
{
    public class NounPhraseNodeOfSemanticAnalyzer: BaseNodeOfSemanticAnalyzer
    {
        public NounPhraseNodeOfSemanticAnalyzer(ContextOfSemanticAnalyzer context, NounPhrase nounPhrase)
            : base(context)
        {
            _nounPhrase = nounPhrase;
        }

        private NounPhrase _nounPhrase;
        private ConceptCGNode _concept;

        public ResultOfNodeOfSemanticAnalyzer Run()
        {
            var result = new ResultOfNodeOfSemanticAnalyzer();            
            var resultSecondaryRolesDict = result.SecondaryRolesDict;
            var conceptualGraph = Context.ConceptualGraph;

            if (_nounPhrase.N == null)
            {
                throw new NotImplementedException("82008E41-50FA-4025-A4A2-BF744FFC6A85");
            }
            else
            {
                ProcessNAsBaseSentenceItem(_nounPhrase.N, result);
            }

            if (_nounPhrase.D != null)
            {
                ProcessDAsBaseSentenceItem(_nounPhrase.D, result);
            }

            if (_nounPhrase.QP != null)
            {
                throw new NotImplementedException("CEE33650-D2F9-455F-ABAE-E6FA4B9BFF15");
            }

            if (_nounPhrase.AP != null)
            {
                ProcessAPAsBaseSentenceItem(_nounPhrase.AP, result);
            }

            if (_nounPhrase.NounAdjunct != null)
            {
                throw new NotImplementedException("1351077F-612E-4AF6-AD3E-E8E3186C2A33");
            }

            if (_nounPhrase.Negation != null)
            {
                throw new NotImplementedException("3CC6FEA6-D4DC-4ED6-9F23-F13137FAA7DC");
            }

            if (_nounPhrase.HasPossesiveMark)
            {
                throw new NotImplementedException("04B16417-0704-48C9-8390-A4226F499E70");
            }

            if (_nounPhrase.PP != null)
            {
                throw new NotImplementedException("E3CFBEE7-B5E5-4DA6-9334-F77004751FB2");
            }

            if (_nounPhrase.AdvP != null)
            {
                throw new NotImplementedException("107E7E1B-AE56-41A6-A9FC-07C14F7E16A8");
            }

            if (_nounPhrase.RelativeClauses != null)
            {
                throw new NotImplementedException("3AF68B85-6062-49EA-8CF4-968508850A28");
            }

            if (_nounPhrase.OtherClauses != null)
            {
                throw new NotImplementedException("84F50287-7291-4BA4-8312-807666686F67");
            }

            if (_nounPhrase.InfinitivePhrases != null)
            {
                throw new NotImplementedException("156C4F97-0EA8-43C5-90DC-1FAE5E6DB212");
            }

            return result;
        }

        private void ProcessNAsBaseSentenceItem(BaseSentenceItem sentenceItem, ResultOfNodeOfSemanticAnalyzer result)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

            switch (kindOfSentenceItem)
            {
                case KindOfSentenceItem.Word:
                    ProcessNAsWord(sentenceItem.AsWord, result);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }
        }

        private void ProcessNAsWord(Word word, ResultOfNodeOfSemanticAnalyzer result)
        {
            var conceptualGraph = Context.ConceptualGraph;

            var baseGrammaticalWordFrame = word.WordFrame;

            if (baseGrammaticalWordFrame.IsNoun && baseGrammaticalWordFrame.AsNoun.IsName)
            {
                _concept = new ConceptCGNode();
                result.RootConcept = _concept;
                _concept.Parent = conceptualGraph;
                _concept.Name = "entity";

                var nameConcept = new ConceptCGNode();
                nameConcept.Parent = conceptualGraph;
                nameConcept.Name = word.RootWordAsString;

                var nameRelation = new RelationCGNode();
                nameRelation.Parent = conceptualGraph;
                nameRelation.Name = "name";

                _concept.AddOutputNode(nameRelation);
                nameRelation.AddOutputNode(nameConcept);

                Context.RelationStorage.AddRelation(_concept.Name, nameConcept.Name, nameRelation.Name);

                MarkAsEntityCondition(nameRelation);
            }
            else
            {
                _concept = new ConceptCGNode();
                result.RootConcept = _concept;
                _concept.Parent = conceptualGraph;
                _concept.Name = word.RootWordAsString;
            }

            var nounFullLogicalMeaning = baseGrammaticalWordFrame.FullLogicalMeaning;

            if (!nounFullLogicalMeaning.IsNullOrEmpty())
            {
                var resultPrimaryRolesDict = result.PrimaryRolesDict;

                foreach (var logicalMeaning in nounFullLogicalMeaning)
                {
                    PrimaryRolesDict.Add(logicalMeaning, _concept);
                    resultPrimaryRolesDict.Add(logicalMeaning, _concept);
                }
            }
        }

        private void ProcessDAsBaseSentenceItem(BaseSentenceItem sentenceItem, ResultOfNodeOfSemanticAnalyzer result)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

            switch (kindOfSentenceItem)
            {
                case KindOfSentenceItem.Word:
                    ProcessDAsWord(sentenceItem.AsWord, result);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }
        }

        private void ProcessDAsWord(Word word, ResultOfNodeOfSemanticAnalyzer result)
        {
            CreateDeterminerMark(_concept, word);
        }

        private void CreateDeterminerMark(ConceptCGNode concept, Word determiner)
        {
            var relationName = CGGramamaticalNamesOfRelations.DeterminerName;

            if (IsPossesDeterminer(determiner))
            {
                CreatePossessMark(concept, determiner);
                return;
            }

            var determinerConceptName = determiner.RootWordAsString;

            if (Context.RelationStorage.ContainsRelation(concept.Name, determinerConceptName, relationName))
            {
                return;
            }

            var conceptualGraph = Context.ConceptualGraph;

            var determinerConcept = new ConceptCGNode();
            determinerConcept.Parent = conceptualGraph;
            determinerConcept.Name = determinerConceptName;

            var determinerRelation = new RelationCGNode();
            determinerRelation.Parent = conceptualGraph;
            determinerRelation.Name = relationName;

            concept.AddOutputNode(determinerRelation);
            determinerRelation.AddOutputNode(determinerConcept);

            Context.RelationStorage.AddRelation(concept.Name, determinerConceptName, relationName);

            MarkAsEntityCondition(determinerRelation);
        }

        private void CreatePossessMark(ConceptCGNode concept, Word determiner)
        {
            var relationName = SpecialNamesOfRelations.PossessName;

            var determinerConceptName = determiner.RootWordAsString;

            if (Context.RelationStorage.ContainsRelation(determinerConceptName, concept.Name, relationName))
            {
                return;
            }

            var conceptualGraph = Context.ConceptualGraph;

            var determinerConcept = new ConceptCGNode();
            determinerConcept.Parent = conceptualGraph;
            determinerConcept.Name = determinerConceptName;

            var determinerRelation = new RelationCGNode();
            determinerRelation.Parent = conceptualGraph;
            determinerRelation.Name = relationName;

            determinerConcept.AddOutputNode(determinerRelation);
            determinerRelation.AddOutputNode(concept);

            Context.RelationStorage.AddRelation(determinerConceptName, concept.Name, relationName);

            MarkAsEntityCondition(determinerRelation);

        }

        public bool IsPossesDeterminer(Word determiner)
        {
            switch(determiner.Content)
            {
                case "my":
                case "our":
                case "your":
                case "his":
                case "her":
                case "its":
                case "their":
                    return true;

                default:
                    return false;
            }
        }

        private void ProcessAPAsBaseSentenceItem(BaseSentenceItem sentenceItem, ResultOfNodeOfSemanticAnalyzer result)
        {
            var kindOfSentenceItem = sentenceItem.KindOfSentenceItem;

            switch (kindOfSentenceItem)
            {
                case KindOfSentenceItem.Word:
                    ProcessAPAsWord(sentenceItem.AsWord, result);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSentenceItem), kindOfSentenceItem, null);
            }
        }

        private void ProcessAPAsWord(Word word, ResultOfNodeOfSemanticAnalyzer result)
        {
            var role = word.WordFrame.LogicalMeaning.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(role))
            {
                throw new NotImplementedException("73B9668A-8B40-42B7-B44C-B41B40B377D8");
            }

            var ajectiveConceptName = word.RootWordAsString;

            if (Context.RelationStorage.ContainsRelation(ajectiveConceptName, _concept.Name, role))
            {
                return;
            }

            var conceptualGraph = Context.ConceptualGraph;

            var ajectiveConcept = new ConceptCGNode();
            ajectiveConcept.Parent = conceptualGraph;
            ajectiveConcept.Name = ajectiveConceptName;

            var ajectiveRelation = new RelationCGNode();
            ajectiveRelation.Parent = conceptualGraph;
            ajectiveRelation.Name = role;

            MarkAsEntityCondition(ajectiveRelation);

            ajectiveRelation.AddInputNode(_concept);
            ajectiveRelation.AddOutputNode(ajectiveConcept);

            Context.RelationStorage.AddRelation(ajectiveConceptName, _concept.Name, role);
        }
    }
}

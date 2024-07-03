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
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;

namespace SymOntoClay.NLP.Internal.PhraseToCGParsing
{
    public class SentenceNodeOfSemanticAnalyzer : BaseNodeOfSemanticAnalyzer
    {
        public SentenceNodeOfSemanticAnalyzer(ContextOfSemanticAnalyzer context, Sentence sentence)
            : base(context)
        {
            _sentence = sentence;
        }

        private Sentence _sentence;
        private ConceptualGraph _conceptualGraph;

        public ResultOfNodeOfSemanticAnalyzer Run()
        {
            var result = new ResultOfNodeOfSemanticAnalyzer();
            var resultPrimaryRolesDict = result.PrimaryRolesDict;
            var resultSecondaryRolesDict = result.SecondaryRolesDict;

            _conceptualGraph = new ConceptualGraph();
            _conceptualGraph.Parent = Context.OuterConceptualGraph;
            Context.ConceptualGraph = _conceptualGraph;
            _conceptualGraph.Name = NameHelper.CreateRuleOrFactName().NameValue;

            CreateGrammaticalRelations();

            if (_sentence.VocativePhrase != null)
            {
                var vocativeNode = new NounPhraseNodeOfSemanticAnalyzer(Context, _sentence.VocativePhrase.AsNounPhrase);
                var vocativeResult = vocativeNode.Run();

#if DEBUG
                Context.Logger.Info("AFD4E961-7430-4EE0-B418-22A1B48C5F4F", $"vocativeResult = {vocativeResult}");
#endif

                var entitiesList = vocativeResult.PrimaryRolesDict.GetByRole("entity");

                foreach (var vocativeEntity in entitiesList)
                {
                    CreateAdresatRelation(vocativeEntity);
                }
            }

            if (_sentence.Condition != null)
            {
                throw new NotImplementedException("5F41FCFD-0CE6-45C9-AAD4-B3C83EC4A80B");
            }

            ResultOfNodeOfSemanticAnalyzer subjectResult = null;

            if (_sentence.Subject != null)
            {
                var subject = _sentence.Subject.AsNounPhrase;

                var subjectNode = new NounPhraseNodeOfSemanticAnalyzer(Context, subject);
                subjectResult = subjectNode.Run();

            }

            var rootVerb = _sentence.Predicate.AsVerbPhrase;

            ResultOfNodeOfSemanticAnalyzer verbResult = null;

            if (rootVerb != null)
            {
                var verbNode = new VerbPhraseNodeOfSemanticAnalyzer(Context, rootVerb);
                verbResult = verbNode.Run();
                if (_sentence.Mood == GrammaticalMood.Imperative)
                {
                    var commandRelation = new RelationCGNode();
                    commandRelation.Name = SpecialNamesOfRelations.CommandRelationName;
                    commandRelation.Parent = _conceptualGraph;

                    commandRelation.AddOutputNode(verbResult.RootConcept);
                }
            }

            if (subjectResult != null && verbResult != null)
            {
                var entitiesList = subjectResult.PrimaryRolesDict.GetByRole("entity");

                if (!entitiesList.IsNullOrEmpty())
                {

                    var primaryStatesList = verbResult.PrimaryRolesDict.GetByRole("state");

                    if (!primaryStatesList.IsNullOrEmpty())
                    {
                        foreach (var state in primaryStatesList)
                        {
                            foreach (var entity in entitiesList)
                            {
                                CreateExperiencerRelation(state, entity);
                                CreateStateRelation(state, entity);
                            }
                        }
                    }

                    var primaryActsList = verbResult.PrimaryRolesDict.GetByRole("act");

                    if (!primaryActsList.IsNullOrEmpty())
                    {
                        foreach (var act in primaryActsList)
                        {
                            foreach (var entity in entitiesList)
                            {
                                CreateAgentRelation(act, entity);
                                CreateActionRelation(act, entity);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private void CreateGrammaticalRelations()
        {
            var conceptualGraph = Context.ConceptualGraph;
            var outerConceptualGraph = Context.OuterConceptualGraph;

            var kindOfQuestionName = GrammaticalElementsHelper.GetKindOfQuestionName(_sentence.KindOfQuestion);

            if(!string.IsNullOrWhiteSpace(kindOfQuestionName))
            {
                var grammarConcept = new ConceptCGNode();
                grammarConcept.Parent = outerConceptualGraph;
                grammarConcept.Name = kindOfQuestionName;

                var grammarRelation = new RelationCGNode();
                grammarRelation.Parent = outerConceptualGraph;
                grammarRelation.Name = CGGramamaticalNamesOfRelations.KindOfQuestionName;

                conceptualGraph.AddOutputNode(grammarRelation);
                grammarRelation.AddOutputNode(grammarConcept);
            }

            var aspectName = GrammaticalElementsHelper.GetAspectName(_sentence.Aspect);

            if (!string.IsNullOrWhiteSpace(aspectName))
            {
                var grammarConcept = new ConceptCGNode();
                grammarConcept.Parent = outerConceptualGraph;
                grammarConcept.Name = aspectName;

                var grammarRelation = new RelationCGNode();
                grammarRelation.Parent = outerConceptualGraph;
                grammarRelation.Name = CGGramamaticalNamesOfRelations.AspectName;

                conceptualGraph.AddOutputNode(grammarRelation);
                grammarRelation.AddOutputNode(grammarConcept);
            }

            var tenseName = GrammaticalElementsHelper.GetTenseName(_sentence.Tense);

            if (!string.IsNullOrWhiteSpace(tenseName))
            {
                var grammarConcept = new ConceptCGNode();
                grammarConcept.Parent = outerConceptualGraph;
                grammarConcept.Name = tenseName;

                var grammarRelation = new RelationCGNode();
                grammarRelation.Parent = outerConceptualGraph;
                grammarRelation.Name = CGGramamaticalNamesOfRelations.TenseName;

                conceptualGraph.AddOutputNode(grammarRelation);
                grammarRelation.AddOutputNode(grammarConcept);
            }

            var voiceName = GrammaticalElementsHelper.GetVoiceName(_sentence.Voice);

            if (!string.IsNullOrWhiteSpace(voiceName))
            {
                var grammarConcept = new ConceptCGNode();
                grammarConcept.Parent = outerConceptualGraph;
                grammarConcept.Name = voiceName;

                var grammarRelation = new RelationCGNode();
                grammarRelation.Parent = outerConceptualGraph;
                grammarRelation.Name = CGGramamaticalNamesOfRelations.VoiceName;

                conceptualGraph.AddOutputNode(grammarRelation);
                grammarRelation.AddOutputNode(grammarConcept);
            }

            var moodName = GrammaticalElementsHelper.GetMoodName(_sentence.Mood);

            if (!string.IsNullOrWhiteSpace(moodName))
            {
                var grammarConcept = new ConceptCGNode();
                grammarConcept.Parent = outerConceptualGraph;
                grammarConcept.Name = moodName;

                var grammarRelation = new RelationCGNode();
                grammarRelation.Parent = outerConceptualGraph;
                grammarRelation.Name = CGGramamaticalNamesOfRelations.MoodName;

                conceptualGraph.AddOutputNode(grammarRelation);
                grammarRelation.AddOutputNode(grammarConcept);
            }

            var abilityModalityName = GrammaticalElementsHelper.GetAbilityModalityName(_sentence.AbilityModality);

            if (!string.IsNullOrWhiteSpace(abilityModalityName))
            {
                var grammarConcept = new ConceptCGNode();
                grammarConcept.Parent = outerConceptualGraph;
                grammarConcept.Name = abilityModalityName;

                var grammarRelation = new RelationCGNode();
                grammarRelation.Parent = outerConceptualGraph;
                grammarRelation.Name = CGGramamaticalNamesOfRelations.AbilityModalityName;

                conceptualGraph.AddOutputNode(grammarRelation);
                grammarRelation.AddOutputNode(grammarConcept);
            }

            var permissionModalityName = GrammaticalElementsHelper.GetPermissionModalityName(_sentence.PermissionModality);

            if (!string.IsNullOrWhiteSpace(permissionModalityName))
            {
                var grammarConcept = new ConceptCGNode();
                grammarConcept.Parent = outerConceptualGraph;
                grammarConcept.Name = permissionModalityName;

                var grammarRelation = new RelationCGNode();
                grammarRelation.Parent = outerConceptualGraph;
                grammarRelation.Name = CGGramamaticalNamesOfRelations.PermissionModalityName;

                conceptualGraph.AddOutputNode(grammarRelation);
                grammarRelation.AddOutputNode(grammarConcept);
            }

            var obligationModalityName = GrammaticalElementsHelper.GetObligationModalityName(_sentence.ObligationModality);

            if (!string.IsNullOrWhiteSpace(obligationModalityName))
            {
                var grammarConcept = new ConceptCGNode();
                grammarConcept.Parent = outerConceptualGraph;
                grammarConcept.Name = obligationModalityName;

                var grammarRelation = new RelationCGNode();
                grammarRelation.Parent = outerConceptualGraph;
                grammarRelation.Name = CGGramamaticalNamesOfRelations.ObligationModalityName;

                conceptualGraph.AddOutputNode(grammarRelation);
                grammarRelation.AddOutputNode(grammarConcept);
            }

            var probabilityModalityName = GrammaticalElementsHelper.GetProbabilityModalityName(_sentence.ProbabilityModality);

            if (!string.IsNullOrWhiteSpace(probabilityModalityName))
            {
                var grammarConcept = new ConceptCGNode();
                grammarConcept.Parent = outerConceptualGraph;
                grammarConcept.Name = probabilityModalityName;

                var grammarRelation = new RelationCGNode();
                grammarRelation.Parent = outerConceptualGraph;
                grammarRelation.Name = CGGramamaticalNamesOfRelations.ProbabilityModalityName;

                conceptualGraph.AddOutputNode(grammarRelation);
                grammarRelation.AddOutputNode(grammarConcept);
            }

            var conditionalModalityName = GrammaticalElementsHelper.GetConditionalModalityName(_sentence.ConditionalModality);

            if (!string.IsNullOrWhiteSpace(conditionalModalityName))
            {
                var grammarConcept = new ConceptCGNode();
                grammarConcept.Parent = outerConceptualGraph;
                grammarConcept.Name = conditionalModalityName;

                var grammarRelation = new RelationCGNode();
                grammarRelation.Parent = outerConceptualGraph;
                grammarRelation.Name = CGGramamaticalNamesOfRelations.ConditionalModalityName;

                conceptualGraph.AddOutputNode(grammarRelation);
                grammarRelation.AddOutputNode(grammarConcept);
            }
        }

        private void CreateAgentRelation(ConceptCGNode verbConcept, ConceptCGNode nounConcept)
        {
            var relationName = SpecialNamesOfRelations.AgentRelationName;

            if (Context.RelationStorage.ContainsRelation(verbConcept.Name, nounConcept.Name, relationName))
            {
                return;
            }

            var conceptualGraph = Context.ConceptualGraph;

            var relation = new RelationCGNode();
            relation.Parent = conceptualGraph;
            relation.Name = relationName;

            verbConcept.AddOutputNode(relation);
            relation.AddOutputNode(nounConcept);

            Context.RelationStorage.AddRelation(verbConcept.Name, nounConcept.Name, relationName);
        }

        private void CreateActionRelation(ConceptCGNode verbConcept, ConceptCGNode nounConcept)
        {
            var relationName = SpecialNamesOfRelations.ActionRelationName;

            if (Context.RelationStorage.ContainsRelation(nounConcept.Name, verbConcept.Name, relationName))
            {
                return;
            }

            var conceptualGraph = Context.ConceptualGraph;

            var relation = new RelationCGNode();
            relation.Parent = conceptualGraph;
            relation.Name = relationName;

            nounConcept.AddOutputNode(relation);
            relation.AddOutputNode(verbConcept);

            Context.RelationStorage.AddRelation(nounConcept.Name, verbConcept.Name, relationName);
        }

        private void CreateAdresatRelation(ConceptCGNode nounConcept)
        {
            var conceptualGraph = Context.ConceptualGraph;
            var outerConceptualGraph = Context.OuterConceptualGraph;

            nounConcept.Parent = outerConceptualGraph;

            var grammarRelation = new RelationCGNode();
            grammarRelation.Parent = outerConceptualGraph;
            grammarRelation.Name = SpecialNamesOfRelations.AdresatRelationName;

            conceptualGraph.AddOutputNode(grammarRelation);
            grammarRelation.AddOutputNode(nounConcept);
        }

        private void CreateExperiencerRelation(ConceptCGNode verbConcept, ConceptCGNode nounConcept)
        {
            var relationName = SpecialNamesOfRelations.ExperiencerRelationName;

            if (Context.RelationStorage.ContainsRelation(verbConcept.Name, nounConcept.Name, relationName))
            {
                return;
            }

            var conceptualGraph = Context.ConceptualGraph;

            var relation = new RelationCGNode();
            relation.Parent = conceptualGraph;
            relation.Name = relationName;

            verbConcept.AddOutputNode(relation);
            relation.AddOutputNode(nounConcept);

            Context.RelationStorage.AddRelation(verbConcept.Name, nounConcept.Name, relationName);
        }

        private void CreateStateRelation(ConceptCGNode verbConcept, ConceptCGNode nounConcept)
        {
            var relationName = SpecialNamesOfRelations.StateRelationName;

            if (Context.RelationStorage.ContainsRelation(nounConcept.Name, verbConcept.Name, relationName))
            {
                return;
            }

            var conceptualGraph = Context.ConceptualGraph;

            var relation = new RelationCGNode();
            relation.Parent = conceptualGraph;
            relation.Name = relationName;

            nounConcept.AddOutputNode(relation);
            relation.AddOutputNode(verbConcept);

            Context.RelationStorage.AddRelation(verbConcept.Name, nounConcept.Name, relationName);
        }
    }
}

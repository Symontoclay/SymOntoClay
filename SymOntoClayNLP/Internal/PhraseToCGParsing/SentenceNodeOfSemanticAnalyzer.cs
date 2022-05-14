using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System.Collections.Generic;
using System.Text;
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
#if DEBUG
            //Context.Logger.Log($"_sentence = {_sentence.ToDbgString()}");
#endif

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
#if DEBUG
                //Context.Logger.Log($"_sentence.VocativePhrase = {_sentence.VocativePhrase}");
#endif

                var vocativeNode = new NounPhraseNodeOfSemanticAnalyzer(Context, _sentence.VocativePhrase.AsNounPhrase);
                var vocativeResult = vocativeNode.Run();

#if DEBUG
                Context.Logger.Log($"vocativeResult = {vocativeResult}");
#endif

                var entitiesList = vocativeResult.PrimaryRolesDict.GetByRole("entity");

                foreach (var vocativeEntity in entitiesList)
                {
                    CreateAdresatRelation(vocativeEntity);
                }
            }

            if (_sentence.Condition != null)
            {
                throw new NotImplementedException();
            }

            var subject = _sentence.Subject.AsNounPhrase;

#if DEBUG
            //Context.Logger.Log($"subject = {subject}");
#endif

            ResultOfNodeOfSemanticAnalyzer subjectResult = null;

            if (subject != null)
            {
                var subjectNode = new NounPhraseNodeOfSemanticAnalyzer(Context, subject);
                subjectResult = subjectNode.Run();

#if DEBUG
                //Context.Logger.Log($"subjectResult = {subjectResult}");
#endif
            }

            var rootVerb = _sentence.Predicate.AsVerbPhrase;

#if DEBUG
            //Context.Logger.Log($"rootVerb = {rootVerb}");
#endif

            ResultOfNodeOfSemanticAnalyzer verbResult = null;

            if (rootVerb != null)
            {
                var verbNode = new VerbPhraseNodeOfSemanticAnalyzer(Context, rootVerb);
                verbResult = verbNode.Run();
#if DEBUG
                //Context.Logger.Log($"verbResult = {verbResult}");
#endif

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
                    //state -> experiencer -> animate

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
                    //act -> agent -> animate

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

            var aspectName = GrammaticalElementsHelper.GetAspectName(_sentence.Aspect);

#if DEBUG
            //Context.Logger.Log($"aspectName = {aspectName}");
#endif

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

#if DEBUG
            //Context.Logger.Log($"tenseName = {tenseName}");
#endif 

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

#if DEBUG
            //Context.Logger.Log($"voiceName = {voiceName}");
#endif
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

#if DEBUG
            //Context.Logger.Log($"moodName = {moodName}");
#endif

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

#if DEBUG
            //Context.Logger.Log($"abilityModalityName = {abilityModalityName}");
#endif

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

#if DEBUG
            //Context.Logger.Log($"permissionModalityName = {permissionModalityName}");
#endif

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

#if DEBUG
            //Context.Logger.Log($"obligationModalityName = {obligationModalityName}");
#endif

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

#if DEBUG
            //Context.Logger.Log($"probabilityModalityName = {probabilityModalityName}");
#endif

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

#if DEBUG
            //Context.Logger.Log($"conditionalModalityName = {conditionalModalityName}");
#endif

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
#if DEBUG
            //Context.Logger.Log($"verbConcept = {verbConcept}");
            //Context.Logger.Log($"nounConcept = {nounConcept}");
#endif

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

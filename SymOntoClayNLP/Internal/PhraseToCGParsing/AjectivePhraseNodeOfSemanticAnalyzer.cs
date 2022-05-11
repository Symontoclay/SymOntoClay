using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseToCGParsing
{
    public class AjectivePhraseNodeOfSemanticAnalyzer : BaseNodeOfSemanticAnalyzer
    {
        public AjectivePhraseNodeOfSemanticAnalyzer(ContextOfSemanticAnalyzer context, Sentence sentence, AdjectivePhrase adjectivePhrase)
            : base(context)
        {
            _sentence = sentence;
            _adjectivePhrase = adjectivePhrase;
        }

        private Sentence _sentence;
        private AdjectivePhrase _adjectivePhrase;
        private ConceptCGNode _concept;

        public ResultOfNodeOfSemanticAnalyzer Run()
        {
#if DEBUG
            Context.Logger.Log($"_adjectivePhrase = {_adjectivePhrase}");
#endif

            var result = new ResultOfNodeOfSemanticAnalyzer();
            var resultPrimaryRolesDict = result.PrimaryRolesDict;
            var resultSecondaryRolesDict = result.SecondaryRolesDict;
            var conceptualGraph = Context.ConceptualGraph;

            if (_adjectivePhrase.AdvP != null)
            {
                throw new NotImplementedException();
            }

            if (_adjectivePhrase.A != null)
            {
                throw new NotImplementedException();
            }

            if (_adjectivePhrase.PP != null)
            {
                throw new NotImplementedException();
            }

            //            var ajective = _adjectivePhrase.Adjective;

            //#if DEBUG
            //            Context.Logger.Log($"ajective = {ajective}");
            //#endif

            //            _concept = new ConceptCGNode();
            //            result.RootConcept = _concept;
            //            _concept.Parent = conceptualGraph;
            //            _concept.Name = GetName(ajective);

            //            var ajectiveFullLogicalMeaning = ajective.FullLogicalMeaning;

            //            if (ajectiveFullLogicalMeaning.IsEmpty())
            //            {
            //                return result;
            //            }

            //            foreach (var logicalMeaning in ajectiveFullLogicalMeaning)
            //            {
            //#if DEBUG
            //                Context.Logger.Log($"logicalMeaning = {logicalMeaning}");
            //#endif

            //                PrimaryRolesDict.Add(logicalMeaning, _concept);
            //                resultPrimaryRolesDict.Add(logicalMeaning, _concept);
            //            }

            //#if DEBUG
            //            Context.Logger.Log($"PrimaryRolesDict = {PrimaryRolesDict}");
            //            Context.Logger.Log("End");
            //#endif

            //            return result;

            throw new NotImplementedException();
        }
    }
}

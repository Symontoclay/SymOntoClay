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

using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseToCGParsing
{
    public class AjectivePhraseNodeOfSemanticAnalyzer : BaseNodeOfSemanticAnalyzer
    {
        public AjectivePhraseNodeOfSemanticAnalyzer(ContextOfSemanticAnalyzer context, AdjectivePhrase adjectivePhrase)
            : base(context)
        {
            _adjectivePhrase = adjectivePhrase;
        }

        private AdjectivePhrase _adjectivePhrase;
        private ConceptCGNode _concept;

        public ResultOfNodeOfSemanticAnalyzer Run()
        {
#if DEBUG
            Context.Logger.Info("F7CB6D71-3AE3-4B7D-B37B-EB15A2F95667", $"_adjectivePhrase = {_adjectivePhrase}");
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










            throw new NotImplementedException();
        }
    }
}

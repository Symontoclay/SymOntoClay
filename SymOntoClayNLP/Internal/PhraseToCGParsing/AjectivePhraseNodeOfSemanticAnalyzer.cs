/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
        //private ConceptCGNode _concept;

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
                throw new NotImplementedException("F71E9FA2-37F3-4C17-B7E3-CDC64B2C27B4");
            }

            if (_adjectivePhrase.A != null)
            {
                throw new NotImplementedException("DD100DB3-93AD-4D81-B4AF-716FCB662BAC");
            }

            if (_adjectivePhrase.PP != null)
            {
                throw new NotImplementedException("4FF3F645-13F4-4F7A-A4BE-ECC88FB8FEC2");
            }

            throw new NotImplementedException("9B4F853B-9A49-4D3F-B413-F0EC439CCE49");
        }
    }
}

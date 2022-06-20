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
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public class ConverterInternalCGToPhraseStructure
    {
        public ConverterInternalCGToPhraseStructure(IEntityLogger logger, IWordsDict wordsDict)
        {
            _wordsDict = wordsDict;
            _logger = logger;
        }

        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;

        public BaseSentenceItem Convert(InternalConceptualGraph source, INLPConverterContext nlpContext)
        {
            var context = new ContextOfConvertingInternalCGToPhraseStructure();
            context.Logger = _logger;
            context.WordsDict = _wordsDict;
            context.NLPContext = nlpContext;

            context.KindOfQuestion = source.KindOfQuestion;
            context.Tense = source.Tense;
            context.Aspect = source.Aspect;
            context.Voice = source.Voice;
            context.Mood = source.Mood;
            context.AbilityModality = source.AbilityModality;
            context.PermissionModality = source.PermissionModality;
            context.ObligationModality = source.ObligationModality;
            context.ProbabilityModality = source.ProbabilityModality;
            context.ConditionalModality = source.ConditionalModality;

            var sentenceNode = new SentenceNode(source, context);

            var nodeResult = sentenceNode.Run();

#if DEBUG
            //_logger.Log($"nodeResult = {nodeResult}");
#endif            

            return nodeResult.SentenceItem;
        }
    }
}

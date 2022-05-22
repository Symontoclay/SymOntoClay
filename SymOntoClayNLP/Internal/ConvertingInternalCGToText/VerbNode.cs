using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToText
{
    public class VerbNode
    {
        public VerbNode(InternalConceptCGNode source, List<string> disabledRelations, string rootSubject, ContextOfConvertingInternalCGToText context)
        {
            _context = context;
            _wordsDict = context.WordsDict;
            _logger = context.Logger;
            _source = source;
            _disabledRelations = disabledRelations;
            _rootSubject = rootSubject;
        }

        private readonly ContextOfConvertingInternalCGToText _context;
        private readonly IWordsDict _wordsDict;
        private readonly IEntityLogger _logger;

        private readonly InternalConceptCGNode _source;
        private readonly List<string> _disabledRelations;
        private readonly string _rootSubject;

        public ResultOfNode Run()
        {
#if DEBUG
            _logger.Log($"_source = {_source}");
#endif

            var verbText = GetVerbText();

#if DEBUG
            _logger.Log($"verbText = '{verbText}'");
#endif

            throw new NotImplementedException();
        }
        
        private string GetVerbText()
        {
            throw new NotImplementedException();
        }

        private string GetVerbTextForActiveVoice()
        {
            throw new NotImplementedException();
        }

        private string GetVerbTextForSimpleAspectOfActiveVoice()
        {
            var verbName = _source.Name;

#if DEBUG
            _logger.Log($"verbName = '{verbName}'");
            _logger.Log($"_rootSubject = '{_rootSubject}'");
#endif

            throw new NotImplementedException();
        }
    }
}

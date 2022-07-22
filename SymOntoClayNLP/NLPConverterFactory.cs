using SymOntoClay.Core;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Dicts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP
{
    public class NLPConverterFactory: INLPConverterFactory
    {
        public NLPConverterFactory(NLPConverterProviderSettings settings, IEntityLogger logger)
        {
            _logger = logger;
            _creationStrategy = settings.CreationStrategy;

            if(_creationStrategy == CreationStrategy.Unknown)
            {
                throw new Exception($"Option {nameof(settings.CreationStrategy)} must not be {CreationStrategy.Unknown}!");
            }

            var dictsPaths = settings.DictsPaths;

            if(dictsPaths.IsNullOrEmpty())
            {
                throw new Exception($"Option {nameof(settings.DictsPaths)} can not be null or empty!");
            }

            if(dictsPaths.Count == 1)
            {
                _wordsDict = new JsonDictionary(dictsPaths.Single());
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private readonly CreationStrategy _creationStrategy;
        private readonly IEntityLogger _logger;
        private readonly object _lockObj = new object();
        private readonly IWordsDict _wordsDict;
        private INLPConverter _converter;

        /// <inheritdoc/>
        public INLPConverter GetConverter()
        {
            lock(_lockObj)
            {
                switch(_creationStrategy)
                {
                    case CreationStrategy.Singleton:
                        lock(_lockObj)
                        {
                            if(_converter == null)
                            {
                                _converter = new NLPConverter(_logger, _wordsDict);
                            }

                            return _converter;
                        }                        

                    case CreationStrategy.NewInstance:
                        return new NLPConverter(_logger, _wordsDict);

                    default:
                        throw new ArgumentOutOfRangeException(nameof(_creationStrategy), _creationStrategy, null);
                }
            }            
        }
    }
}

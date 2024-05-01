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
using SymOntoClay.Core;
using SymOntoClay.Monitor.Common;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Dicts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.NLP
{
    public class NLPConverterFactory: INLPConverterFactory
    {
        public NLPConverterFactory(NLPConverterProviderSettings settings, IMonitorLogger logger)
        {
            _logger = logger;
            _creationStrategy = settings.CreationStrategy;

            if(_creationStrategy == CreationStrategy.Unknown)
            {
                throw new Exception($"Option {nameof(settings.CreationStrategy)} must not be {CreationStrategy.Unknown}!");
            }

            var dictsPaths = settings.DictsPaths;
            var dictsList = settings.DictsList;

            if(dictsPaths.IsNullOrEmpty() && dictsList.IsNullOrEmpty())
            {
                throw new Exception($"Option {nameof(settings.DictsPaths)} and {nameof(settings.DictsList)} can not be null or empty! You should set at least one dict in some way.");
            }

            var targetDictsList = new List<IWordsDict>();

            if(!dictsPaths.IsNullOrEmpty())
            {
                foreach(var dictPath in dictsPaths)
                {
                    targetDictsList.Add(new JsonDictionary(dictPath));
                }
            }

            if(!dictsList.IsNullOrEmpty())
            {
                targetDictsList.AddRange(dictsList);
            }

            if(targetDictsList.Count == 1)
            {
                _wordsDict = targetDictsList.Single();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private readonly CreationStrategy _creationStrategy;
        private readonly IMonitorLogger _logger;
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

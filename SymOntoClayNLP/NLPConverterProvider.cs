using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP
{
    public class NLPConverterProvider: INLPConverterProvider
    {
        public NLPConverterProvider(NLPConverterProviderSettings settings)
        {
            _settings = settings;
        }

        private readonly NLPConverterProviderSettings _settings;
        private readonly object _lockObj = new object();
        private INLPConverterFactory _factory;

        /// <inheritdoc/>
        public INLPConverterFactory GetFactory(IEntityLogger logger)
        {
            lock(_lockObj)
            {
                if(_factory == null)
                {
                    _factory = new NLPConverterFactory(_settings, logger);
                }

                return _factory;
            }
        }
    }
}

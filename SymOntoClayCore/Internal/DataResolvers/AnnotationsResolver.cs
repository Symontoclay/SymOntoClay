using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class AnnotationsResolver : BaseResolver
    {
        public AnnotationsResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _synonymsResolver = dataResolversFactory.GetSynonymsResolver();
        }

        private readonly SynonymsResolver _synonymsResolver;

        public Value GetSettings(Value annotatedItem, StrongIdentifierValue key, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"key = {key}");
#endif

            var value = annotatedItem.GetSettings(key);

            if (value != null)
            {
                return value;
            }

            var synonymsList = _synonymsResolver.GetSynonyms(key, localCodeExecutionContext);

            foreach (var synonym in synonymsList)
            {
                value = annotatedItem.GetSettings(synonym);

                if (value != null)
                {
                    return value;
                }
            }

            return null;
        }
    }
}

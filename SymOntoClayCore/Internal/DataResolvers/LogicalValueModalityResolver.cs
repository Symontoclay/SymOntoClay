using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalValueModalityResolver : BaseResolver
    {
        public LogicalValueModalityResolver(IMainStorageContext context)
            : base(context)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _fuzzyLogicResolver = dataResolversFactory.GetFuzzyLogicResolver();
        }

        private readonly FuzzyLogicResolver _fuzzyLogicResolver;

        public bool IsFit(Value modalityValue, Value queryModalityValue, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"modalityValue = {modalityValue}");
            //Log($"queryModalityValue = {queryModalityValue}");
#endif

            if(modalityValue == null)
            {
                return false;
            }

            if(modalityValue.KindOfValue == KindOfValue.NullValue)
            {
                return false;
            }

            return _fuzzyLogicResolver.Equals(modalityValue, queryModalityValue, localCodeExecutionContext);
        }
    }
}

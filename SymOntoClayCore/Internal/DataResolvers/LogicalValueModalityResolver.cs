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

        public bool IsFit(Value modalityValue, Value queryModalityValue)
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

            return EqualityCompare(modalityValue, queryModalityValue);
        }

        private bool EqualityCompare(Value modalityValue, Value queryModalityValue)
        {
            if ((modalityValue.IsNumberValue || modalityValue.IsLogicalValue) && (queryModalityValue.IsNumberValue || queryModalityValue.IsLogicalValue))
            {
                var sysValue1 = modalityValue.GetSystemValue();
                var sysValue2 = queryModalityValue.GetSystemValue();

                return ObjectHelper.IsEquals(sysValue1, sysValue2);
            }

            if(modalityValue.IsStrongIdentifierValue && queryModalityValue.IsStrongIdentifierValue)
            {
                var modalityValueStrongIdentifierValue = modalityValue.AsStrongIdentifierValue;
                var queryModalityStrongIdentifierValue = queryModalityValue.AsStrongIdentifierValue;

                if(modalityValueStrongIdentifierValue == queryModalityStrongIdentifierValue)
                {
                    return true;
                }

                throw new NotImplementedException();
            }

            if(modalityValue.IsFuzzyLogicNonNumericSequenceValue && queryModalityValue.IsFuzzyLogicNonNumericSequenceValue)
            {
                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }
    }
}

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
        }

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

            if((modalityValue.IsNumberValue || modalityValue.IsLogicalValue) && (queryModalityValue.IsNumberValue || queryModalityValue.IsLogicalValue))
            {
                var sysValue1 = modalityValue.GetSystemValue();
                var sysValue2 = queryModalityValue.GetSystemValue();

                return ObjectHelper.IsEquals(sysValue1, sysValue2);
            }

            throw new NotImplementedException();
        }
    }
}

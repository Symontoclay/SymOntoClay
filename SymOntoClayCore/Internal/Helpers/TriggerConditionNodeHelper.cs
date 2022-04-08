using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class TriggerConditionNodeHelper
    {
        public static int GetInt32Duration(TriggerConditionNode node)
        {
            var value = node.Value;

            var kindOfValue = value.KindOfValue;

            switch (kindOfValue)
            {
                case KindOfValue.NumberValue:
                    return Convert.ToInt32(value.AsNumberValue.SystemValue);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfValue), kindOfValue, null);
            }
        }
    }
}

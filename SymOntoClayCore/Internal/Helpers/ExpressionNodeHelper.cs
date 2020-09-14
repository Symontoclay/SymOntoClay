using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class ExpressionNodeHelper
    {
        public static bool Compare(BaseIndexedLogicalQueryNode expressionNode1, BaseIndexedLogicalQueryNode expressionNode2, IEntityLogger logger)
        {
#if DEBUG
            logger.Log($"(expressionNode1 == null) = {expressionNode1 == null} (expressionNode2 == null) = {expressionNode2 == null}");
            logger.Log($"expressionNode1 = {expressionNode1}");
            logger.Log($"expressionNode2 = {expressionNode2}");
#endif

            if(expressionNode1.IsKeyRef && expressionNode2.IsKeyRef)
            {
                if(expressionNode1.AsKeyRef.Key == expressionNode2.AsKeyRef.Key)
                {
                    return true;
                }

                return false;
            }

            throw new NotImplementedException();
        }
    }
}

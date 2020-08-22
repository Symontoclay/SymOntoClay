using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class EntityConditionNode : BaseNode
    {
        public EntityConditionNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(EntityConditionAstExpression expression)
        {
#if DEBUG
            Log($"expression = {expression}");
#endif

            throw new NotImplementedException();
        }
    }
}

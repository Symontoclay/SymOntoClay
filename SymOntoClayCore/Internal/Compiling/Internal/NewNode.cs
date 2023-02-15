using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class NewNode : BaseNode
    {
        public NewNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(NewAstExpression expression)
        {
#if DEBUG
            //Log($"expression = {expression}");
            Log($"expression = {expression.ToHumanizedString()}");
#endif

            throw new NotImplementedException();

            var node = new ExpressionNode(_context);
            node.Run(expression.PrototypeExpression);

            AddCommands(node.Result);

            CompilePushAnnotation(expression);

            var command = new IntermediateScriptCommand();

            command.OperationCode = OperationCode.Instantiate;

            AddCommand(command);

#if DEBUG
            //DbgPrintCommands();
#endif
        }
    }
}

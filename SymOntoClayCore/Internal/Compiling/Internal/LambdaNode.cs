using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class LambdaNode : BaseNode
    {
        public LambdaNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstStatement statement)
        {
#if DEBUG
            //Info("D86974B2-295C-4F9C-AD8B-29B48AF85CB7", $"statement.GetType().Name = {statement.GetType().Name}");
#endif

            var astExpressionStatement = statement as AstExpressionStatement;

            var node = new ExpressionNode(_context, KindOfCompilePushVal.GetAllCases);
            node.Run(astExpressionStatement.Expression);

            AddCommands(node.Result);

            AddCommand(new IntermediateScriptCommand()
            {
                OperationCode = OperationCode.ReturnVal
            });

#if DEBUG
            //DbgPrintCommands();
#endif
        }
    }
}

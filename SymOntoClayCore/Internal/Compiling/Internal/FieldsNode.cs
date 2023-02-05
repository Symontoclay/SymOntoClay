using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class FieldsNode : BaseNode
    {
        public FieldsNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(List<Field> fields)
        {
#if DEBUG
            //Log($"fields = {fields.WriteListToString()}");
#endif

            foreach(var field in fields)
            {
#if DEBUG
                //Log($"field = {field}");
#endif

                CompileVarDecl(field);

                var node = new ExpressionNode(_context);
                node.Run(field.Value);

                AddCommands(node.Result);

                CompilePushVal(field.Name);

                CompilePushAnnotation(field);

                var command = new IntermediateScriptCommand();
                command.OperationCode = OperationCode.CallBinOp;
                command.KindOfOperator = KindOfOperator.Assign;

                AddCommand(command);

#if DEBUG
                //DbgPrintCommands();
#endif
            }
        }
    }
}

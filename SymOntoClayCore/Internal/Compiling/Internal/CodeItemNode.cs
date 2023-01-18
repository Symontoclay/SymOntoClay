using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class CodeItemNode : BaseNode
    {
        public CodeItemNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(CodeItemAstExpression expression)
        {
#if DEBUG
            //Log($"expression = {expression}");
#endif

            CompilePushVal(expression.CodeItem);

            CompilePushAnnotation(expression);

            
            if(expression.CodeItem.IsAnonymous)
            {
                var command = new IntermediateScriptCommand();

                command.OperationCode = OperationCode.Instantiate;

                AddCommand(command);
            }
            else
            {
                var command = new IntermediateScriptCommand();

                command.OperationCode = OperationCode.CodeItemDecl;

                AddCommand(command);

                AddCommand(new IntermediateScriptCommand()
                {
                    OperationCode = OperationCode.ClearStack
                });
            }

#if DEBUG
            //DbgPrintCommands();
#endif
        }
    }
}

using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class UseInheritanceStatementNode : BaseNode
    {
        public UseInheritanceStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstUseInheritanceStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement}");
#endif

            CompileValue(statement.SubName);
            CompileValue(statement.SuperName);
            CompileValue(statement.Rank);

            var command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = statement.GetAnnotationValue().GetIndexedValue(_context);

            AddCommand(command);

            command = new ScriptCommand();

            if (statement.HasNot)
            {
                command.OperationCode = OperationCode.UseNotInheritance;
            }
            else
            {
                command.OperationCode = OperationCode.UseInheritance;
            }
            
            AddCommand(command);

            AddCommand(new ScriptCommand()
            {
                OperationCode = OperationCode.ClearStack
            });
        }
    }
}

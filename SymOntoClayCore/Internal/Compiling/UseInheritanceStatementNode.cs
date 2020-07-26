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
            Log($"statement = {statement}");
#endif

            var command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = statement.SubName;

            AddCommand(command);

            command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = statement.SuperName;

            AddCommand(command);

            command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = statement.Rank;

            AddCommand(command);

            command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = statement.GetAnnotationValue();

            AddCommand(command);

            command = new ScriptCommand();
            command.OperationCode = OperationCode.UseInheritance;

            AddCommand(command);

            AddCommand(new ScriptCommand()
            {
                OperationCode = OperationCode.ClearStack
            });
        }
    }
}

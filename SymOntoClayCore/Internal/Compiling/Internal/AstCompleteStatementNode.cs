using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstCompleteStatementNode : BaseNode
    {
        public AstCompleteStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstCompleteStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement}");
#endif

            var kind = statement.KindOfCompletion;

            switch (kind)
            {
                case KindOfCompletion.Action:
                    {
                        var ruleInstance = statement.RuleInstanceValue;

                        if(ruleInstance == null)
                        {
                            AddCommand(new IntermediateScriptCommand()
                            {
                                OperationCode = OperationCode.CompleteAction
                            });
                            break;
                        }

                        CompileValue(statement.RuleInstanceValue);

                        AddCommand(new IntermediateScriptCommand()
                        {
                            OperationCode = OperationCode.CompleteActionVal
                        });
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}

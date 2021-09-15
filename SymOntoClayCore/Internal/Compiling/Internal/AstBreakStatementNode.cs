using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class AstBreakStatementNode : BaseNode
    {
        public AstBreakStatementNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(AstBreakStatement statement)
        {
#if DEBUG
            //Log($"statement = {statement}");
#endif

            var kind = statement.KindOfBreak;

            switch(kind)
            {
                case KindOfBreak.Action:
                    {
                        var ruleInstance = statement.RuleInstanceValue;

                        if(ruleInstance == null)
                        {
                            AddCommand(new IntermediateScriptCommand()
                            {
                                OperationCode = OperationCode.BreakAction
                            });
                            break;
                        }

                        CompileValue(statement.RuleInstanceValue);

                        AddCommand(new IntermediateScriptCommand()
                        {
                            OperationCode = OperationCode.BreakActionVal
                        });
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}

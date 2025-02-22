using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class PropertiesNode : BaseNode
    {
        public PropertiesNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(List<Property> properties)
        {
            foreach (var property in properties)
            {
                CompilePropertyDecl(property);

                if (property.DefaultValue == null)
                {
                    continue;
                }

                var node = new ExpressionNode(_context, null);
                node.Run(property.DefaultValue);

                AddCommands(node.Result);

                CompilePushVal(property.Name, KindOfCompilePushVal.Direct);

                CompilePushAnnotation(property);

                var command = new IntermediateScriptCommand();
                command.OperationCode = OperationCode.CallBinOp;
                command.KindOfOperator = KindOfOperator.Assign;

                AddCommand(command);
            }

#if DEBUG
            //DbgPrintCommands();
#endif

            //throw new NotImplementedException("B31892E2-8D57-49F8-B60D-6530CEE4F5F1");
        }

        private void CompilePropertyDecl(Property property)
        {
            CompilePushAnnotation(property);

            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.PropDecl;

            AddCommand(command);
        }
    }
}

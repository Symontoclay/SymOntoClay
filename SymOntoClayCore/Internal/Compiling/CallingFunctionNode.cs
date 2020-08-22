using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class CallingFunctionNode : BaseNode
    {
        public CallingFunctionNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(CallingFunctionAstExpression expression)
        {
#if DEBUG
            Log($"expression = {expression}");
#endif

            if(!expression.MainParameters.IsNullOrEmpty())
            {
                if(expression.MainParameters.Any(p => p.IsNamed) && expression.MainParameters.Any(p => !p.IsNamed))
                {
                    throw new NotSupportedException();
                }

                var isNamed = expression.MainParameters.Any(p => p.IsNamed);

#if DEBUG
                Log($"isNamed = {isNamed}");
#endif

                foreach(var parameter in expression.MainParameters)
                {
#if DEBUG
                    Log($"parameter = {parameter}");
#endif

                    if (isNamed)
                    {
                        var node = new ExpressionNode(_context);
                        node.Run(parameter.Name);
                        AddCommands(node.Result);
                        node = new ExpressionNode(_context);
                        node.Run(parameter.Value);
                        AddCommands(node.Result);
                    }
                    else
                    {
                        var node = new ExpressionNode(_context);
                        node.Run(parameter.Value);
                        AddCommands(node.Result);
                    }
                }
            }

            if(!expression.AdditionalParameters.IsNullOrEmpty())
            {
                if(expression.AdditionalParameters.Any(p => p.IsNamed) && expression.AdditionalParameters.Any(p => !p.IsNamed))
                {
                    throw new NotSupportedException();
                }

                var isNamed = expression.AdditionalParameters.Any(p => p.IsNamed);

#if DEBUG
                Log($"isNamed (2) = {isNamed}");
#endif

                foreach (var parameter in expression.AdditionalParameters)
                {
#if DEBUG
                    Log($"parameter = {parameter}");
#endif

                    if (isNamed)
                    {
                        var node = new ExpressionNode(_context);
                        node.Run(parameter.Name);
                        AddCommands(node.Result);
                        node = new ExpressionNode(_context);
                        node.Run(parameter.Value);
                        AddCommands(node.Result);
                    }
                    else
                    {
                        var node = new ExpressionNode(_context);
                        node.Run(parameter.Value);
                        AddCommands(node.Result);
                    }
                }
            }

            //var methodnode = 

            throw new NotImplementedException();
        }
    }
}

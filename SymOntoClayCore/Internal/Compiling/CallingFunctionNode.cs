/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public class CallingFunctionNode : BaseNode
    {
        private enum KindOfParameters
        {
            NoParameters,
            NamedParameters,
            PositionedParameters
        }

        public CallingFunctionNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(CallingFunctionAstExpression expression)
        {
#if DEBUG
            //Log($"expression = {expression}");
#endif

            var kindOfParameters = KindOfParameters.NoParameters;

            var command = new ScriptCommand();

            if (!expression.Parameters.IsNullOrEmpty())
            {
                if(expression.Parameters.Any(p => p.IsNamed) && expression.Parameters.Any(p => !p.IsNamed))
                {
                    throw new NotSupportedException();
                }

                command.CountParams = expression.Parameters.Count;

                var isNamed = expression.Parameters.Any(p => p.IsNamed);

#if DEBUG
                //Log($"isNamed = {isNamed}");
#endif

                if(isNamed)
                {
                    kindOfParameters = KindOfParameters.NamedParameters;
                }
                else
                {
                    kindOfParameters = KindOfParameters.PositionedParameters;
                }

                foreach(var parameter in expression.Parameters)
                {
#if DEBUG
                    //Log($"parameter = {parameter}");
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

            var leftNode = new ExpressionNode(_context);
            leftNode.Run(expression.Left);
            AddCommands(leftNode.Result);

            CompilePushAnnotation(expression);

#if DEBUG
            //Log($"kindOfParameters = {kindOfParameters}");
#endif

            if(expression.IsAsync)
            {
                switch (kindOfParameters)
                {
                    case KindOfParameters.NoParameters:
                        command.OperationCode = OperationCode.AsyncCall;
                        break;

                    case KindOfParameters.NamedParameters:
                        command.OperationCode = OperationCode.AsyncCall_N;
                        break;

                    case KindOfParameters.PositionedParameters:
                        command.OperationCode = OperationCode.AsyncCall_P;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
                }
            }
            else
            {
                switch (kindOfParameters)
                {
                    case KindOfParameters.NoParameters:
                        command.OperationCode = OperationCode.Call;
                        break;

                    case KindOfParameters.NamedParameters:
                        command.OperationCode = OperationCode.Call_N;
                        break;

                    case KindOfParameters.PositionedParameters:
                        command.OperationCode = OperationCode.Call_P;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
                }
            }

#if DEBUG
            //Log($"command = {command}");
#endif

            AddCommand(command);
        }
    }
}

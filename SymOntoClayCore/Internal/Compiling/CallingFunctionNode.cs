/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

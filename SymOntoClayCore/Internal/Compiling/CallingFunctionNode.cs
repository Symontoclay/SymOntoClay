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
            Log($"expression = {expression}");
#endif

            var kindOfMainParameters = KindOfParameters.NoParameters;
            var kindOfAdditionalParameters = KindOfParameters.NoParameters;

            var command = new ScriptCommand();

            if (!expression.MainParameters.IsNullOrEmpty())
            {
                if(expression.MainParameters.Any(p => p.IsNamed) && expression.MainParameters.Any(p => !p.IsNamed))
                {
                    throw new NotSupportedException();
                }

                command.CountMainParams = expression.MainParameters.Count;

                var isNamed = expression.MainParameters.Any(p => p.IsNamed);

#if DEBUG
                Log($"isNamed = {isNamed}");
#endif

                if(isNamed)
                {
                    kindOfMainParameters = KindOfParameters.NamedParameters;
                }
                else
                {
                    kindOfMainParameters = KindOfParameters.PositionedParameters;
                }

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

                command.CountAdditionalParams = expression.AdditionalParameters.Count;

                var isNamed = expression.AdditionalParameters.Any(p => p.IsNamed);

#if DEBUG
                Log($"isNamed (2) = {isNamed}");
#endif

                if (isNamed)
                {
                    kindOfAdditionalParameters = KindOfParameters.NamedParameters;
                }
                else
                {
                    kindOfAdditionalParameters = KindOfParameters.PositionedParameters;
                }

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

            CompilePushAnnotation(expression);

#if DEBUG
            Log($"kindOfMainParameters = {kindOfMainParameters}");
            Log($"kindOfAdditionalParameters = {kindOfAdditionalParameters}");
#endif

            switch (kindOfMainParameters)
            {
                case KindOfParameters.NoParameters:
                    switch (kindOfAdditionalParameters)
                    {
                        case KindOfParameters.NoParameters:
                            command.OperationCode = OperationCode.Call;
                            break;

                        case KindOfParameters.NamedParameters:
                            command.OperationCode = OperationCode.Call_AN;
                            break;

                        case KindOfParameters.PositionedParameters:
                            command.OperationCode = OperationCode.Call_AP;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfAdditionalParameters), kindOfAdditionalParameters, null);
                    }
                    break;

                case KindOfParameters.NamedParameters:
                    switch (kindOfAdditionalParameters)
                    {
                        case KindOfParameters.NoParameters:
                            command.OperationCode = OperationCode.Call_MN;
                            break;

                        case KindOfParameters.NamedParameters:
                            command.OperationCode = OperationCode.Call_MN_AN;
                            break;

                        case KindOfParameters.PositionedParameters:
                            command.OperationCode = OperationCode.Call_MN_AP;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfAdditionalParameters), kindOfAdditionalParameters, null);
                    }
                    break;

                case KindOfParameters.PositionedParameters:
                    switch (kindOfAdditionalParameters)
                    {
                        case KindOfParameters.NoParameters:
                            command.OperationCode = OperationCode.Call_MP;
                            break;

                        case KindOfParameters.NamedParameters:
                            command.OperationCode = OperationCode.Call_MP_AN;
                            break;

                        case KindOfParameters.PositionedParameters:
                            command.OperationCode = OperationCode.Call_MP_AP;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(kindOfAdditionalParameters), kindOfAdditionalParameters, null);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfMainParameters), kindOfMainParameters, null);
            }

#if DEBUG
            Log($"command = {command}");
#endif

            AddCommand(command);
        }
    }
}

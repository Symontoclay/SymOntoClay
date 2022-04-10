﻿using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution.Helpers
{
    public static class CodeFrameHelper
    {
        public static CodeFrame ConvertExecutableToCodeFrame(IExecutable function, KindOfFunctionParameters kindOfParameters,
            Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            LocalCodeExecutionContext parentLocalCodeExecutionContext, IMainStorageContext context)
        {
#if DEBUG
            //Log($"kindOfParameters = {kindOfParameters}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
#endif

            var storagesList = parentLocalCodeExecutionContext.Storage.GetStorages();

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach(var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            var localStorageSettings = RealStorageSettingsHelper.Create(context, storagesList.ToList(), false);

            var newStorage = new LocalStorage(localStorageSettings);

            localCodeExecutionContext.Storage = newStorage;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    if (function.Arguments.Any())
                    {
                        throw new NotImplementedException();
                    }
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    FillUpPositionedParameters(localCodeExecutionContext, function, positionedParameters);
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    FillUpNamedParameters(localCodeExecutionContext, function, namedParameters);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

            localCodeExecutionContext.Holder = parentLocalCodeExecutionContext.Holder;

            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = function.CompiledFunctionBody;
            codeFrame.LocalContext = localCodeExecutionContext;

            var processInfo = new ProcessInfo();

            codeFrame.ProcessInfo = processInfo;
            processInfo.CodeFrame = codeFrame;
            codeFrame.Metadata = function.CodeItem;

#if DEBUG
            //Log($"codeFrame = {codeFrame}");
#endif

            return codeFrame;
        }

        private static void FillUpPositionedParameters(LocalCodeExecutionContext localCodeExecutionContext, IExecutable function, List<Value> positionedParameters)
        {
            var varsStorage = localCodeExecutionContext.Storage.VarStorage;

            var positionedParametersEnumerator = positionedParameters.GetEnumerator();

            foreach (var argument in function.Arguments)
            {
#if DEBUG
                //Log($"argument = {argument}");
#endif

                if (!positionedParametersEnumerator.MoveNext())
                {
                    if (argument.HasDefaultValue)
                    {
                        varsStorage.SetValue(argument.Name, argument.DefaultValue);
                        break;
                    }

                    throw new NotImplementedException();
                }

                var parameterItem = positionedParametersEnumerator.Current;

#if DEBUG
                //Log($"parameterItem = {parameterItem}");
#endif

                varsStorage.SetValue(argument.Name, parameterItem);
            }
        }

        private static void FillUpNamedParameters(LocalCodeExecutionContext localCodeExecutionContext, IExecutable function, Dictionary<StrongIdentifierValue, Value> namedParameters)
        {
            var varsStorage = localCodeExecutionContext.Storage.VarStorage;

            var usedParameters = new List<StrongIdentifierValue>();

            foreach (var namedParameter in namedParameters)
            {
                var parameterName = namedParameter.Key;

#if DEBUG
                //Log($"parameterName = {parameterName}");
#endif

                var kindOfParameterName = parameterName.KindOfName;

                switch (kindOfParameterName)
                {
                    case KindOfName.Var:
                        break;

                    case KindOfName.Concept:
                        parameterName = NameHelper.CreateName($"@{parameterName.NameValue}");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParameterName), kindOfParameterName, null);
                }

#if DEBUG
                //Log($"parameterName (after) = {parameterName}");
#endif

                if (function.ContainsArgument(parameterName))
                {
                    usedParameters.Add(parameterName);

                    varsStorage.SetValue(parameterName, namedParameter.Value);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

#if DEBUG
            //Log($"usedParameters = {usedParameters.WriteListToString()}");
#endif

            var argumentsList = function.Arguments;

            if (usedParameters.Count < argumentsList.Count)
            {
                foreach (var argument in argumentsList)
                {
                    if (usedParameters.Contains(argument.Name))
                    {
                        continue;
                    }

#if DEBUG
                    //Log($"argument = {argument}");
#endif

                    if (argument.HasDefaultValue)
                    {
                        varsStorage.SetValue(argument.Name, argument.DefaultValue);
                        continue;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }
    }
}

/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.CodeExecution.Helpers
{
    public static class CodeFrameHelper
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static CodeFrame ConvertCompiledFunctionBodyToCodeFrame(CompiledFunctionBody compiledFunctionBody, LocalCodeExecutionContext parentLocalCodeExecutionContext, IMainStorageContext context)
        {
            var storagesList = parentLocalCodeExecutionContext.Storage.GetStorages();

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach(var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var localCodeExecutionContext = new LocalCodeExecutionContext(parentLocalCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(context, storagesList.ToList(), false);

            var newStorage = new LocalStorage(localStorageSettings);

            localCodeExecutionContext.Storage = newStorage;

            localCodeExecutionContext.Holder = parentLocalCodeExecutionContext.Holder;

            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = compiledFunctionBody;
            codeFrame.LocalContext = localCodeExecutionContext;

            var processInfo = new ProcessInfo();

            codeFrame.ProcessInfo = processInfo;
            processInfo.CodeFrame = codeFrame;
            //codeFrame.Metadata = function.CodeItem;

#if DEBUG
            //Log($"codeFrame = {codeFrame}");
#endif

            return codeFrame;
        }

        public static CodeFrame ConvertExecutableToCodeFrame(IExecutable function, KindOfFunctionParameters kindOfParameters,
            Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            LocalCodeExecutionContext parentLocalCodeExecutionContext, IMainStorageContext context, ConversionExecutableToCodeFrameAdditionalSettings additionalSettings = null)
        {
#if DEBUG
            //Log($"kindOfParameters = {kindOfParameters}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
            //_gbcLogger.Info($"parentLocalCodeExecutionContext.GetHashCode() = {parentLocalCodeExecutionContext.GetHashCode()}");
#endif

            var storagesList = parentLocalCodeExecutionContext.Storage.GetStorages();

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach(var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var localCodeExecutionContext = new LocalCodeExecutionContext(parentLocalCodeExecutionContext);
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
                    FillUpNamedParameters(localCodeExecutionContext, function, namedParameters, context);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

            localCodeExecutionContext.Holder = parentLocalCodeExecutionContext.Holder;

#if DEBUG
            //_gbcLogger.Info($"localCodeExecutionContext = {localCodeExecutionContext.GetHashCode()}");
            //_gbcLogger.Info($"additionalSettings = {additionalSettings}");
#endif

            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = function.CompiledFunctionBody;
            codeFrame.LocalContext = localCodeExecutionContext;

            var processInfo = new ProcessInfo();

            codeFrame.ProcessInfo = processInfo;
            processInfo.CodeFrame = codeFrame;

            var codeItem = function.CodeItem;

#if DEBUG
            //_gbcLogger.Info($"codeItem = {codeItem}");
#endif

            codeFrame.Metadata = codeItem;

            var timeout = additionalSettings?.Timeout;

            if (timeout.HasValue)
            {
                codeFrame.TargetDuration = timeout;
            }

            var priority = additionalSettings?.Priority;

#if DEBUG
            //_gbcLogger.Info($"priority = {priority}");
#endif

            if (priority.HasValue)
            {
                processInfo.Priority = priority.Value;
            }
            else
            {
                var codeItemPriority = codeItem.Priority;

                if (codeItemPriority != null)
                {
#if DEBUG
                    //_gbcLogger.Info($"codeItemPriority = {codeItemPriority}");
#endif
                    var numberValueLinearResolver = context.DataResolversFactory.GetNumberValueLinearResolver();

                    var numberValue = numberValueLinearResolver.Resolve(codeItemPriority, parentLocalCodeExecutionContext);

#if DEBUG
                    //_gbcLogger.Info($"numberValue = {numberValue}");
#endif

                    if (!(numberValue == null || numberValue.KindOfValue == KindOfValue.NullValue))
                    {
                        processInfo.Priority = Convert.ToSingle(numberValue.SystemValue.Value);
                    }
                }
            }


#if DEBUG
            //_gbcLogger.Info($"processInfo = {processInfo}");
#endif

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

        private static void FillUpNamedParameters(LocalCodeExecutionContext localCodeExecutionContext, IExecutable function, Dictionary<StrongIdentifierValue, Value> namedParameters, IMainStorageContext context)
        {
            var varsStorage = localCodeExecutionContext.Storage.VarStorage;

            var usedParameters = new List<StrongIdentifierValue>();

            var synonymsResolver = context.DataResolversFactory.GetSynonymsResolver();

            foreach (var namedParameter in namedParameters)
            {
                var parameterName = namedParameter.Key;

#if DEBUG
                //_gbcLogger.Info($"parameterName = {parameterName}");
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
                //_gbcLogger.Info($"parameterName (after) = {parameterName}");
#endif

                parameterName = CheckParameterName(parameterName, function, synonymsResolver, localCodeExecutionContext);

#if DEBUG
                //_gbcLogger.Info($"parameterName (after 2) = {parameterName}");
#endif

                if (parameterName == null)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    usedParameters.Add(parameterName);

                    varsStorage.SetValue(parameterName, namedParameter.Value);
                }
            }

#if DEBUG
            //_gbcLogger.Info($"usedParameters = {usedParameters.WriteListToString()}");
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

        private static StrongIdentifierValue CheckParameterName(StrongIdentifierValue parameterName, IExecutable function, SynonymsResolver synonymsResolver, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //_gbcLogger.Info($"parameterName = {parameterName}");
#endif

            if(function.ContainsArgument(parameterName))
            {
                return parameterName;
            }

            var synonymsList = synonymsResolver.GetSynonyms(parameterName, localCodeExecutionContext);

#if DEBUG
            //_gbcLogger.Info($"synonymsList = {synonymsList.WriteListToString()}");
#endif

            foreach(var synonym in synonymsList)
            {
#if DEBUG
                //_gbcLogger.Info($"synonym = {synonym}");
#endif

                if (function.ContainsArgument(synonym))
                {
                    return synonym;
                }

                var alternativeSynonym = NameHelper.CreateAlternativeArgumentName(synonym);

#if DEBUG
                //_gbcLogger.Info($"alternativeSynonym = {alternativeSynonym}");
#endif

                if (function.ContainsArgument(alternativeSynonym))
                {
                    return alternativeSynonym;
                }
            }

            var alternativeParameterName = NameHelper.CreateAlternativeArgumentName(parameterName);

#if DEBUG
            //_gbcLogger.Info($"alternativeParameterName = {alternativeParameterName}");
#endif

            synonymsList = synonymsResolver.GetSynonyms(alternativeParameterName, localCodeExecutionContext);

#if DEBUG
            //_gbcLogger.Info($"synonymsList = {synonymsList.WriteListToString()}");
#endif

            foreach (var synonym in synonymsList)
            {
#if DEBUG
                //_gbcLogger.Info($"synonym = {synonym}");
#endif

                if (function.ContainsArgument(synonym))
                {
                    return synonym;
                }

                var alternativeSynonym = NameHelper.CreateAlternativeArgumentName(synonym);

#if DEBUG
                //_gbcLogger.Info($"alternativeSynonym = {alternativeSynonym}");
#endif

                if (function.ContainsArgument(alternativeSynonym))
                {
                    return alternativeSynonym;
                }
            }

            return null;
        }
    }
}

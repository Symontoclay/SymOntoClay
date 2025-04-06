/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeExecution.Helpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Services
{
    public class CodeFrameService : BaseContextComponent, ICodeFrameService
    {
        public CodeFrameService(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            _baseResolver = _context.DataResolversFactory.GetBaseResolver();
        }

        private readonly IMainStorageContext _context;
        private BaseResolver _baseResolver;

        /// <inheritdoc/>
        public CodeFrame ConvertCompiledFunctionBodyToCodeFrame(IMonitorLogger logger, CompiledFunctionBody compiledFunctionBody, ILocalCodeExecutionContext parentLocalCodeExecutionContext)
        {
            var storagesList = parentLocalCodeExecutionContext.Storage.GetStorages(logger);

            var localCodeExecutionContext = new LocalCodeExecutionContext(parentLocalCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(_context, storagesList.ToList(), false);

            var newStorage = new LocalStorage(localStorageSettings);

            localCodeExecutionContext.Storage = newStorage;

            localCodeExecutionContext.Holder = parentLocalCodeExecutionContext.Holder;

            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = compiledFunctionBody;
            codeFrame.LocalContext = localCodeExecutionContext;

            var processInfo = new ProcessInfo(_context.GetCancellationToken(), _context.AsyncEventsThreadPool, _context.ActiveObjectContext);

            codeFrame.ProcessInfo = processInfo;
            processInfo.CodeFrame = codeFrame;

            return codeFrame;
        }

        /// <inheritdoc/>
        public CodeFrame ConvertExecutableToCodeFrame(IMonitorLogger logger, IExecutable function, KindOfFunctionParameters kindOfParameters,
            Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            ILocalCodeExecutionContext parentLocalCodeExecutionContext, ConversionExecutableToCodeFrameAdditionalSettings additionalSettings = null, bool useParentLocalCodeExecutionContextDirectly = false)
        {
            return ConvertExecutableToCodeFrame(logger, string.Empty, function, kindOfParameters,
            namedParameters, positionedParameters,
            parentLocalCodeExecutionContext, additionalSettings, useParentLocalCodeExecutionContextDirectly);
        }

        /// <inheritdoc/>
        public CodeFrame ConvertExecutableToCodeFrame(IMonitorLogger logger, string callMethodId, IExecutable function, KindOfFunctionParameters kindOfParameters,
            Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            ILocalCodeExecutionContext parentLocalCodeExecutionContext, ConversionExecutableToCodeFrameAdditionalSettings additionalSettings = null, bool useParentLocalCodeExecutionContextDirectly = false)
        {
            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = function.CompiledFunctionBody;
            codeFrame.CallMethodId = callMethodId;

            if (useParentLocalCodeExecutionContextDirectly)
            {
                codeFrame.LocalContext = parentLocalCodeExecutionContext;
            }
            else
            {
                var storagesList = parentLocalCodeExecutionContext.Storage.GetStorages(logger);

                var localCodeExecutionContext = new LocalCodeExecutionContext(parentLocalCodeExecutionContext);
                var localStorageSettings = RealStorageSettingsHelper.Create(_context, storagesList.ToList(), additionalSettings?.AllowParentLocalStorages ?? false);

                var newStorage = new LocalStorage(localStorageSettings);

                localCodeExecutionContext.Storage = newStorage;

                var functionHolder = function.Holder;

                if (functionHolder != null)
                {
                    localCodeExecutionContext.Owner = functionHolder;

                    localCodeExecutionContext.OwnerStorage = storagesList.SingleOrDefault(p => p.Kind == KindOfStorage.SuperClass && p.TargetClassName == functionHolder);
                }

                var codeFrameArguments = codeFrame.Arguments;

                switch (kindOfParameters)
                {
                    case KindOfFunctionParameters.NoParameters:
                        if (function.Arguments.Any())
                        {
                            throw new NotImplementedException("69A0AE35-46D8-4D88-B93F-05E7CA4F1766");
                        }
                        break;

                    case KindOfFunctionParameters.PositionedParameters:
                        FillUpPositionedParameters(logger, localCodeExecutionContext, function, positionedParameters, ref codeFrameArguments);
                        break;

                    case KindOfFunctionParameters.NamedParameters:
                        FillUpNamedParameters(logger, localCodeExecutionContext, function, namedParameters, ref codeFrameArguments);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
                }

                localCodeExecutionContext.Holder = parentLocalCodeExecutionContext.Holder;

                codeFrame.LocalContext = localCodeExecutionContext;
            }

            var processInfo = new ProcessInfo(_context.GetCancellationToken(), _context.AsyncEventsThreadPool, _context.ActiveObjectContext);

            codeFrame.ProcessInfo = processInfo;
            processInfo.CodeFrame = codeFrame;

            var codeItem = function.CodeItem;

            codeFrame.Metadata = codeItem;

            var timeout = additionalSettings?.Timeout;

            if (timeout.HasValue)
            {
                codeFrame.TargetDuration = timeout;
                codeFrame.TimeoutCancellationMode = additionalSettings.TimeoutCancellationMode;
            }

            var priority = additionalSettings?.Priority;

            if (priority.HasValue)
            {
                processInfo.Priority = priority.Value;
            }
            else
            {
                var codeItemPriority = codeItem?.Priority;

                if (codeItemPriority != null)
                {
                    var numberValueLinearResolver = _context.DataResolversFactory.GetNumberValueLinearResolver();

                    var numberValue = numberValueLinearResolver.Resolve(logger, codeItemPriority, parentLocalCodeExecutionContext);

                    if (!(numberValue == null || numberValue.KindOfValue == KindOfValue.NullValue))
                    {
                        processInfo.Priority = Convert.ToSingle(numberValue.SystemValue.Value);
                    }
                }
            }

            return codeFrame;
        }

        private void FillUpPositionedParameters(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext, IExecutable function, List<Value> positionedParameters, ref Dictionary<StrongIdentifierValue, Value> codeFrameArguments)
        {
            if(positionedParameters == null)
            {
                return;
            }

            var varsStorage = localCodeExecutionContext.Storage.VarStorage;

            var positionedParametersEnumerator = positionedParameters.GetEnumerator();

            foreach (var argument in function.Arguments)
            {
                if (!positionedParametersEnumerator.MoveNext())
                {
                    if (argument.HasDefaultValue)
                    {
                        varsStorage.SetValue(logger, argument.Name, argument.DefaultValue);
                        codeFrameArguments[argument.Name] = argument.DefaultValue;
                        break;
                    }

                    throw new NotImplementedException("27F6F650-1F3F-405D-9560-3A00F920CD91");
                }

                var parameterItem = positionedParametersEnumerator.Current;

                varsStorage.SetValue(logger, argument.Name, parameterItem);
                codeFrameArguments[argument.Name] = parameterItem;
            }
        }

        private void FillUpNamedParameters(IMonitorLogger logger, ILocalCodeExecutionContext localCodeExecutionContext, IExecutable function, Dictionary<StrongIdentifierValue, Value> namedParameters, ref Dictionary<StrongIdentifierValue, Value> codeFrameArguments)
        {
            var varsStorage = localCodeExecutionContext.Storage.VarStorage;

            var usedParameters = new List<StrongIdentifierValue>();

            var synonymsResolver = _context.DataResolversFactory.GetSynonymsResolver();

            foreach (var namedParameter in namedParameters)
            {
                var parameterName = namedParameter.Key;

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

                parameterName = CheckParameterName(logger, parameterName, function, synonymsResolver, localCodeExecutionContext);

                if (parameterName == null)
                {
                    throw new NotImplementedException("EB12A701-887B-44A7-BFF2-9FC64BBF0286");
                }
                else
                {
                    usedParameters.Add(parameterName);

                    varsStorage.SetValue(logger, parameterName, namedParameter.Value);
                    codeFrameArguments[parameterName] = namedParameter.Value;
                }
            }

            var argumentsList = function.Arguments;

            if (usedParameters.Count < argumentsList.Count)
            {
                foreach (var argument in argumentsList)
                {
                    if (usedParameters.Contains(argument.Name))
                    {
                        continue;
                    }

                    if (argument.HasDefaultValue)
                    {
                        varsStorage.SetValue(logger, argument.Name, argument.DefaultValue);
                        codeFrameArguments[argument.Name] = argument.DefaultValue;
                        continue;
                    }
                    else
                    {
                        throw new NotImplementedException("4FF1D1A4-672E-46B0-9535-40741D41C9A9");
                    }
                }
            }
        }

        private StrongIdentifierValue CheckParameterName(IMonitorLogger logger, StrongIdentifierValue parameterName, IExecutable function, SynonymsResolver synonymsResolver, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            if (function.ContainsArgument(logger, parameterName))
            {
                return parameterName;
            }

            var synonymsList = synonymsResolver.GetSynonyms(logger, parameterName, localCodeExecutionContext);

            foreach (var synonym in synonymsList)
            {
                if (function.ContainsArgument(logger, synonym))
                {
                    return synonym;
                }

                var alternativeSynonym = NameHelper.CreateAlternativeArgumentName(synonym);

                if (function.ContainsArgument(logger, alternativeSynonym))
                {
                    return alternativeSynonym;
                }
            }

            var alternativeParameterName = NameHelper.CreateAlternativeArgumentName(parameterName);

            synonymsList = synonymsResolver.GetSynonyms(logger, alternativeParameterName, localCodeExecutionContext);

            foreach (var synonym in synonymsList)
            {
                if (function.ContainsArgument(logger, synonym))
                {
                    return synonym;
                }

                var alternativeSynonym = NameHelper.CreateAlternativeArgumentName(synonym);

                if (function.ContainsArgument(logger, alternativeSynonym))
                {
                    return alternativeSynonym;
                }
            }

            return null;
        }
    }
}

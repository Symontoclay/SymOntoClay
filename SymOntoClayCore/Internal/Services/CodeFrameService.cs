/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Services
{
    public class CodeFrameService : BaseComponent, ICodeFrameService
    {
        public CodeFrameService(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
            _baseResolver = context.DataResolversFactory.GetBaseResolver();
        }

        private readonly IMainStorageContext _context;
        private readonly BaseResolver _baseResolver;

        /// <inheritdoc/>
        public CodeFrame ConvertCompiledFunctionBodyToCodeFrame(CompiledFunctionBody compiledFunctionBody, ILocalCodeExecutionContext parentLocalCodeExecutionContext)
        {
            var storagesList = parentLocalCodeExecutionContext.Storage.GetStorages();

            var localCodeExecutionContext = new LocalCodeExecutionContext(parentLocalCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(_context, storagesList.ToList(), false);

            var newStorage = new LocalStorage(localStorageSettings);

            localCodeExecutionContext.Storage = newStorage;

            localCodeExecutionContext.Holder = parentLocalCodeExecutionContext.Holder;

            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = compiledFunctionBody;
            codeFrame.LocalContext = localCodeExecutionContext;

            var processInfo = new ProcessInfo();

            codeFrame.ProcessInfo = processInfo;
            processInfo.CodeFrame = codeFrame;

            return codeFrame;
        }

        /// <inheritdoc/>
        public CodeFrame ConvertExecutableToCodeFrame(IExecutable function, KindOfFunctionParameters kindOfParameters,
            Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            ILocalCodeExecutionContext parentLocalCodeExecutionContext, ConversionExecutableToCodeFrameAdditionalSettings additionalSettings = null, bool useParentLocalCodeExecutionContextDirectly = false)
        {
            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = function.CompiledFunctionBody;

            if(useParentLocalCodeExecutionContextDirectly)
            {
                codeFrame.LocalContext = parentLocalCodeExecutionContext;
            }
            else
            {
                var storagesList = parentLocalCodeExecutionContext.Storage.GetStorages();

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

                codeFrame.LocalContext = localCodeExecutionContext;
            }

            var processInfo = new ProcessInfo();

            codeFrame.ProcessInfo = processInfo;
            processInfo.CodeFrame = codeFrame;

            var codeItem = function.CodeItem;

            codeFrame.Metadata = codeItem;

            var timeout = additionalSettings?.Timeout;

            if (timeout.HasValue)
            {
                codeFrame.TargetDuration = timeout;
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

                    var numberValue = numberValueLinearResolver.Resolve(codeItemPriority, parentLocalCodeExecutionContext);

                    if (!(numberValue == null || numberValue.KindOfValue == KindOfValue.NullValue))
                    {
                        processInfo.Priority = Convert.ToSingle(numberValue.SystemValue.Value);
                    }
                }
            }

            return codeFrame;
        }

        private void FillUpPositionedParameters(ILocalCodeExecutionContext localCodeExecutionContext, IExecutable function, List<Value> positionedParameters)
        {
            var varsStorage = localCodeExecutionContext.Storage.VarStorage;

            var positionedParametersEnumerator = positionedParameters.GetEnumerator();

            foreach (var argument in function.Arguments)
            {
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

                varsStorage.SetValue(argument.Name, parameterItem);
            }
        }

        private void FillUpNamedParameters(ILocalCodeExecutionContext localCodeExecutionContext, IExecutable function, Dictionary<StrongIdentifierValue, Value> namedParameters)
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

                parameterName = CheckParameterName(parameterName, function, synonymsResolver, localCodeExecutionContext);

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

        private StrongIdentifierValue CheckParameterName(StrongIdentifierValue parameterName, IExecutable function, SynonymsResolver synonymsResolver, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            if (function.ContainsArgument(parameterName))
            {
                return parameterName;
            }

            var synonymsList = synonymsResolver.GetSynonyms(parameterName, localCodeExecutionContext);

            foreach (var synonym in synonymsList)
            {
                if (function.ContainsArgument(synonym))
                {
                    return synonym;
                }

                var alternativeSynonym = NameHelper.CreateAlternativeArgumentName(synonym);

                if (function.ContainsArgument(alternativeSynonym))
                {
                    return alternativeSynonym;
                }
            }

            var alternativeParameterName = NameHelper.CreateAlternativeArgumentName(parameterName);

            synonymsList = synonymsResolver.GetSynonyms(alternativeParameterName, localCodeExecutionContext);

            foreach (var synonym in synonymsList)
            {
                if (function.ContainsArgument(synonym))
                {
                    return synonym;
                }

                var alternativeSynonym = NameHelper.CreateAlternativeArgumentName(synonym);

                if (function.ContainsArgument(alternativeSynonym))
                {
                    return alternativeSynonym;
                }
            }

            return null;
        }
    }
}

﻿using SymOntoClay.Core.Internal.CodeExecution;
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
        }

        private readonly IMainStorageContext _context;

        /// <inheritdoc/>
        public CodeFrame ConvertCompiledFunctionBodyToCodeFrame(CompiledFunctionBody compiledFunctionBody, LocalCodeExecutionContext parentLocalCodeExecutionContext)
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
            //codeFrame.Metadata = function.CodeItem;

#if DEBUG
            //Log($"codeFrame = {codeFrame}");
#endif

            return codeFrame;
        }

        /// <inheritdoc/>
        public CodeFrame ConvertExecutableToCodeFrame(IExecutable function, KindOfFunctionParameters kindOfParameters,
            Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            LocalCodeExecutionContext parentLocalCodeExecutionContext, ConversionExecutableToCodeFrameAdditionalSettings additionalSettings = null)
        {
#if DEBUG
            //Log($"kindOfParameters = {kindOfParameters}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
            //Log($"parentLocalCodeExecutionContext.Storage.VarStorage.GetHashCode() = {parentLocalCodeExecutionContext.Storage.VarStorage.GetHashCode()}");
            //Log($"additionalSettings = {additionalSettings}");
#endif

            var storagesList = parentLocalCodeExecutionContext.Storage.GetStorages();

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach(var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage.Kind = {tmpStorage.Kind}");
            //    Log($"tmpStorage = {tmpStorage.VarStorage.GetHashCode()}");
            //}
#endif

            var localCodeExecutionContext = new LocalCodeExecutionContext(parentLocalCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(_context, storagesList.ToList(), additionalSettings?.AllowParentLocalStorages ?? false);

            var newStorage = new LocalStorage(localStorageSettings);

            localCodeExecutionContext.Storage = newStorage;

#if DEBUG
            //Log($"localCodeExecutionContext.Storage.VarStorage.GetHashCode() = {localCodeExecutionContext.Storage.VarStorage.GetHashCode()}");
#endif

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

#if DEBUG
            //Log($"localCodeExecutionContext = {localCodeExecutionContext.GetHashCode()}");
            //Log($"additionalSettings = {additionalSettings}");
#endif

            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = function.CompiledFunctionBody;
            codeFrame.LocalContext = localCodeExecutionContext;

            var processInfo = new ProcessInfo();

            codeFrame.ProcessInfo = processInfo;
            processInfo.CodeFrame = codeFrame;

            var codeItem = function.CodeItem;

#if DEBUG
            //Log($"codeItem = {codeItem}");
#endif

            codeFrame.Metadata = codeItem;

            var timeout = additionalSettings?.Timeout;

            if (timeout.HasValue)
            {
                codeFrame.TargetDuration = timeout;
            }

            var priority = additionalSettings?.Priority;

#if DEBUG
            //Log($"priority = {priority}");
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
                    //Log($"codeItemPriority = {codeItemPriority}");
#endif
                    var numberValueLinearResolver = _context.DataResolversFactory.GetNumberValueLinearResolver();

                    var numberValue = numberValueLinearResolver.Resolve(codeItemPriority, parentLocalCodeExecutionContext);

#if DEBUG
                    //Log($"numberValue = {numberValue}");
#endif

                    if (!(numberValue == null || numberValue.KindOfValue == KindOfValue.NullValue))
                    {
                        processInfo.Priority = Convert.ToSingle(numberValue.SystemValue.Value);
                    }
                }
            }


#if DEBUG
            //Log($"processInfo = {processInfo}");
#endif

#if DEBUG
            //Log($"codeFrame = {codeFrame}");
#endif

            return codeFrame;
        }

        private void FillUpPositionedParameters(LocalCodeExecutionContext localCodeExecutionContext, IExecutable function, List<Value> positionedParameters)
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

        private void FillUpNamedParameters(LocalCodeExecutionContext localCodeExecutionContext, IExecutable function, Dictionary<StrongIdentifierValue, Value> namedParameters)
        {
            var varsStorage = localCodeExecutionContext.Storage.VarStorage;

            var usedParameters = new List<StrongIdentifierValue>();

            var synonymsResolver = _context.DataResolversFactory.GetSynonymsResolver();

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

                parameterName = CheckParameterName(parameterName, function, synonymsResolver, localCodeExecutionContext);

#if DEBUG
                //Log($"parameterName (after 2) = {parameterName}");
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

        private StrongIdentifierValue CheckParameterName(StrongIdentifierValue parameterName, IExecutable function, SynonymsResolver synonymsResolver, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"parameterName = {parameterName}");
#endif

            if (function.ContainsArgument(parameterName))
            {
                return parameterName;
            }

            var synonymsList = synonymsResolver.GetSynonyms(parameterName, localCodeExecutionContext);

#if DEBUG
            //Log($"synonymsList = {synonymsList.WriteListToString()}");
#endif

            foreach (var synonym in synonymsList)
            {
#if DEBUG
                //Log($"synonym = {synonym}");
#endif

                if (function.ContainsArgument(synonym))
                {
                    return synonym;
                }

                var alternativeSynonym = NameHelper.CreateAlternativeArgumentName(synonym);

#if DEBUG
                //Log($"alternativeSynonym = {alternativeSynonym}");
#endif

                if (function.ContainsArgument(alternativeSynonym))
                {
                    return alternativeSynonym;
                }
            }

            var alternativeParameterName = NameHelper.CreateAlternativeArgumentName(parameterName);

#if DEBUG
            //Log($"alternativeParameterName = {alternativeParameterName}");
#endif

            synonymsList = synonymsResolver.GetSynonyms(alternativeParameterName, localCodeExecutionContext);

#if DEBUG
            //Log($"synonymsList = {synonymsList.WriteListToString()}");
#endif

            foreach (var synonym in synonymsList)
            {
#if DEBUG
                //Log($"synonym = {synonym}");
#endif

                if (function.ContainsArgument(synonym))
                {
                    return synonym;
                }

                var alternativeSynonym = NameHelper.CreateAlternativeArgumentName(synonym);

#if DEBUG
                //Log($"alternativeSynonym = {alternativeSynonym}");
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
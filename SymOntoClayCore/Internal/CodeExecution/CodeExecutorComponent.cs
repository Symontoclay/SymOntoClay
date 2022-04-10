/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution.Helpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class CodeExecutorComponent: BaseComponent, ICodeExecutorComponent
    {
        public CodeExecutorComponent(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;

            var dataResolversFactory = context.DataResolversFactory;

            _operatorsResolver = dataResolversFactory.GetOperatorsResolver();
            _methodsResolver = dataResolversFactory.GetMethodsResolver();
        }

        private readonly IEngineContext _context;

        private readonly OperatorsResolver _operatorsResolver;
        private readonly MethodsResolver _methodsResolver;

        /// <inheritdoc/>
        public Value ExecuteAsync(ProcessInitialInfo processInitialInfo)
        {
            return ExecuteBatchAsync(new List<ProcessInitialInfo>() { processInitialInfo });
        }

        /// <inheritdoc/>
        public Value ExecuteBatchAsync(List<ProcessInitialInfo> processInitialInfoList)
        {
#if DEBUG
            //Log($"processInitialInfoList = {processInitialInfoList.WriteListToString()}");
#endif

            var codeFramesList = new List<CodeFrame>();

            foreach (var processInitialInfo in processInitialInfoList)
            {
                var codeFrame = new CodeFrame();
                codeFrame.CompiledFunctionBody = processInitialInfo.CompiledFunctionBody;
                codeFrame.LocalContext = processInitialInfo.LocalContext;

                codeFrame.Instance = processInitialInfo.Instance;
                codeFrame.ExecutionCoordinator = processInitialInfo.ExecutionCoordinator;

                var processInfo = new ProcessInfo();

                codeFrame.ProcessInfo = processInfo;
                processInfo.CodeFrame = codeFrame;
                codeFrame.Metadata = processInitialInfo.Metadata;

                _context.InstancesStorage.AppendProcessInfo(processInfo);

                codeFramesList.Add(codeFrame);
            }

#if DEBUG
            //_context.InstancesStorage.PrintProcessesList();
            //Log("L>>>");
#endif

            var threadExecutor = new AsyncThreadExecutor(_context);
            threadExecutor.SetCodeFrames(codeFramesList);

            return threadExecutor.Start();
        }

        /// <inheritdoc/>
        public Value CallOperator(KindOfOperator kindOfOperator, List<Value> paramsList, LocalCodeExecutionContext parentLocalCodeExecutionContext)
        {
            var operatorInfo = _operatorsResolver.GetOperator(kindOfOperator, parentLocalCodeExecutionContext);

#if DEBUG
            //Log($"operatorInfo = {operatorInfo}");
#endif

            return CallExecutableSync(operatorInfo, paramsList, parentLocalCodeExecutionContext);
        }

        /// <inheritdoc/>
        public Value CallExecutableSync(IExecutable executable, List<Value> positionedParameters, LocalCodeExecutionContext parentLocalCodeExecutionContext)
        {
            return CallExecutable(executable, KindOfFunctionParameters.PositionedParameters, null, positionedParameters, true, parentLocalCodeExecutionContext);
        }

        private Value CallExecutable(IExecutable executable, KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters, bool isSync, LocalCodeExecutionContext parentLocalCodeExecutionContext)
        {
            if (executable == null)
            {
                throw new ArgumentNullException(nameof(executable));
            }

            var coordinator = executable.TryActivate(_context);

#if DEBUG
            //Log($"executable.IsSystemDefined = {executable.IsSystemDefined}");
            //Log($"coordinator != null = {coordinator != null}");
            //Log($"isSync = {isSync}");
#endif

            if (executable.IsSystemDefined)
            {
                Value result = null;

                switch (kindOfParameters)
                {
                    case KindOfFunctionParameters.PositionedParameters:
                        result = executable.SystemHandler.Call(positionedParameters, parentLocalCodeExecutionContext);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
                }

#if DEBUG
                //Log($"result = {result}");
#endif

                return result;
            }
            else
            {
                var newCodeFrame = CodeFrameHelper.ConvertExecutableToCodeFrame(executable, kindOfParameters, namedParameters, positionedParameters, parentLocalCodeExecutionContext, _context);

#if DEBUG
                //Log($"newCodeFrame = {newCodeFrame.ToDbgString()}");
#endif

                _context.InstancesStorage.AppendProcessInfo(newCodeFrame.ProcessInfo);

#if DEBUG
                //Log($"isSync = {isSync}");
#endif

                if (isSync)
                {
                    newCodeFrame.ExecutionCoordinator = coordinator;

                    var threadExecutor = new SyncThreadExecutor(_context);
                    threadExecutor.SetCodeFrame(newCodeFrame);

                    threadExecutor.Start();

                    return threadExecutor.ExternalReturn;
                }
                else
                {
                    var threadExecutor = new AsyncThreadExecutor(_context);
                    threadExecutor.SetCodeFrame(newCodeFrame);

                    var task = threadExecutor.Start();

                    return task;
                }
            }
        }

        /// <inheritdoc/>
        public Value CallFunctionSync(Value caller, KindOfFunctionParameters kindOfParameters, List<Value> parameters, LocalCodeExecutionContext parentLocalCodeExecutionContext)
        {
            return CallFunction(caller, kindOfParameters, parameters, parentLocalCodeExecutionContext, true);
        }

        private Value CallFunction(Value caller, KindOfFunctionParameters kindOfParameters, List<Value> parameters, LocalCodeExecutionContext parentLocalCodeExecutionContext, bool isSync)
        {
#if DEBUG
            //Log($"kindOfparameters = {kindOfParameters}");
            //Log($"isSync = {isSync}");
            //Log($"parameters = {parameters.WriteListToString()}");
#endif

            Dictionary<StrongIdentifierValue, Value> namedParameters = null;
            List<Value> positionedParameters = null;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    namedParameters = TakeNamedParameters(parameters);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    positionedParameters = parameters;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

#if DEBUG
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
            //Log($"caller.IsPointRefValue = {caller.IsPointRefValue}");
            //Log($"caller.IsStrongIdentifierValue = {caller.IsStrongIdentifierValue}");
#endif

            if (caller.IsPointRefValue)
            {
                return CallPointRefValue(caller.AsPointRefValue, kindOfParameters, namedParameters, positionedParameters, isSync);
            }

            if (caller.IsStrongIdentifierValue)
            {
                return CallStrongIdentifierValue(caller.AsStrongIdentifierValue, kindOfParameters, namedParameters, positionedParameters, isSync, parentLocalCodeExecutionContext);
            }

            throw new NotImplementedException();
        }

        private Dictionary<StrongIdentifierValue, Value> TakeNamedParameters(List<Value> rawParamsList)
        {
#if DEBUG
            //Log($"rawParamsList = {rawParamsList.WriteListToString()}");
#endif

            var result = new Dictionary<StrongIdentifierValue, Value>();

            var enumerator = rawParamsList.GetEnumerator();

            while (enumerator.MoveNext())
            {
#if DEBUG
                //Log($"enumerator.Current = {enumerator.Current}");
#endif

                var name = enumerator.Current.AsStrongIdentifierValue;

#if DEBUG
                //Log($"name = {name}");
#endif

                enumerator.MoveNext();

                var value = enumerator.Current;

#if DEBUG
                //Log($"value = {value}");
#endif

                result[name] = value;
            }

            return result;
        }

        private Value CallPointRefValue(PointRefValue caller,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            bool isSync)
        {
#if DEBUG
            //Log($"caller.LeftOperand = {caller.LeftOperand}");
#endif

            if (caller.LeftOperand.IsHostValue)
            {
                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private Value CallStrongIdentifierValue(StrongIdentifierValue methodName,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            bool isSync, LocalCodeExecutionContext parentLocalCodeExecutionContext)
        {
#if DEBUG
            //Log($"methodName = {methodName}");
            //Log($"kindOfParameters = {kindOfParameters}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
            //Log($"isSync = {isSync}");
#endif
            IExecutable method = null;

            switch (kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    method = _methodsResolver.Resolve(methodName, parentLocalCodeExecutionContext);
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    method = _methodsResolver.Resolve(methodName, namedParameters, parentLocalCodeExecutionContext);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    method = _methodsResolver.Resolve(methodName, positionedParameters, parentLocalCodeExecutionContext);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

#if DEBUG
            //Log($"method = {method}");
#endif

            if (method == null)
            {
                throw new Exception($"Method '{methodName.NameValue}' is not found.");
            }

            return CallExecutable(method, kindOfParameters, namedParameters, positionedParameters, isSync, parentLocalCodeExecutionContext);
        }
    }
}

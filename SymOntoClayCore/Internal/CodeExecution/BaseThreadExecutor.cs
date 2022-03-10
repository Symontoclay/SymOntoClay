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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public abstract class BaseThreadExecutor: BaseLoggedComponent
    {
        private enum KindOfFunctionParameters
        {
            NoParameters,
            NamedParameters,
            PositionedParameters
        }

        protected BaseThreadExecutor(IEngineContext context, IActivePeriodicObject activeObject, IExecutionCoordinator executionCoordinator)
            :base(context.Logger)
        {
            _context = context;

            _executionCoordinator = executionCoordinator;

            _globalStorage = context.Storage.GlobalStorage;
            _globalLogicalStorage = _globalStorage.LogicalStorage;
            _hostListener = context.HostListener;

            _instancesStorage = _context.InstancesStorage;

            _activeObject = activeObject;
            activeObject.PeriodicMethod = CommandLoop;

            var dataResolversFactory = context.DataResolversFactory;

            _operatorsResolver = dataResolversFactory.GetOperatorsResolver();
            _logicalValueLinearResolver = dataResolversFactory.GetLogicalValueLinearResolver();
            _numberValueLinearResolver = dataResolversFactory.GetNumberValueLinearResolver();
            _strongIdentifierLinearResolver = dataResolversFactory.GetStrongIdentifierLinearResolver();
            _varsResolver = dataResolversFactory.GetVarsResolver();
            _methodsResolver = dataResolversFactory.GetMethodsResolver();
            _logicalSearchResolver = dataResolversFactory.GetLogicalSearchResolver();
        }

        private readonly IEngineContext _context;
        private readonly IExecutionCoordinator _executionCoordinator;
        private readonly IStorage _globalStorage;
        private readonly ILogicalStorage _globalLogicalStorage;
        private readonly IHostListener _hostListener;
        private readonly IInstancesStorageComponent _instancesStorage;

        private readonly OperatorsResolver _operatorsResolver;
        private readonly LogicalValueLinearResolver _logicalValueLinearResolver;
        private readonly NumberValueLinearResolver _numberValueLinearResolver;
        private readonly StrongIdentifierLinearResolver _strongIdentifierLinearResolver;
        private readonly VarsResolver _varsResolver;
        private readonly MethodsResolver _methodsResolver;
        private readonly LogicalSearchResolver _logicalSearchResolver;

        private Stack<CodeFrame> _codeFrames = new Stack<CodeFrame>();
        private CodeFrame _currentCodeFrame;

        private IVarStorage _currentVarStorage;

        private ErrorValue _currentError;
        private bool _isCanceled;

        protected IActivePeriodicObject _activeObject { get; private set; }

        public void SetCodeFrame(CodeFrame codeFrame, bool setAsRunning = true)
        {
#if DEBUG
            //Log($"codeFrame = {codeFrame}");
#endif

            _codeFrames.Push(codeFrame);
            _currentCodeFrame = codeFrame;

            SetUpCurrentCodeFrame();

            if (setAsRunning)
            {
                _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Running;
            }
        }

        public void SetCodeFrames(List<CodeFrame> codeFramesList)
        {
            var reverseCodeFramesList = codeFramesList.ToList();
            reverseCodeFramesList.Reverse();

            foreach(var codeFrame in reverseCodeFramesList)
            {
                codeFrame.ProcessInfo.Status = ProcessStatus.WaitingToRun;

                SetCodeFrame(codeFrame, false);
            }

            _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Running;

#if DEBUG
            //_instancesStorage.PrintProcessesList();
#endif
        }

        public Value Start()
        {
            return _activeObject.Start();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
#if DEBUG
            Log("Dispose()");
#endif

            _activeObject.Dispose();
        }

        private bool CommandLoop()
        {
            try
            {
                if(_executionCoordinator != null && _executionCoordinator.ExecutionStatus != ActionExecutionStatus.Executing)
                {
#if DEBUG
                    //Log("_executionCoordinator != null && _executionCoordinator.ExecutionStatus != ActionExecutionStatus.Executing; return false;");
#endif

                    return false;
                }

                if(_currentCodeFrame == null)
                {
#if DEBUG
                    //Log("_currentCodeFrame == null return false;");
#endif

                    return false;
                }

                var currentPosition = _currentCodeFrame.CurrentPosition;

#if DEBUG
                //Log($"currentPosition = {currentPosition}");
#endif

                var compiledFunctionBodyCommands = _currentCodeFrame.CompiledFunctionBody.Commands;

#if DEBUG
                //Log($"compiledFunctionBodyCommands.Count = {compiledFunctionBodyCommands.Count}");
#endif

                if (currentPosition >= compiledFunctionBodyCommands.Count)
                {
#if DEBUG
                    //Log("currentPosition >= compiledFunctionBodyCommands.Count return true;");
#endif
                    GoBackToPrevCodeFrame();
                    return true;
                }

                var currentCommand = compiledFunctionBodyCommands[currentPosition];

#if DEBUG
                //Log($"currentCommand = {currentCommand}");
                //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                if (!CheckReturnedInfo())
                {
#if DEBUG
                    //Log("!CheckReturnedInfo()");
#endif

                    return false;
                }

                switch (currentCommand.OperationCode)
                {
                    case OperationCode.Nop:
                        _currentCodeFrame.CurrentPosition++;
                        break;

                    case OperationCode.ClearStack:
                        _currentCodeFrame.ValuesStack.Clear();
                        _currentCodeFrame.CurrentPosition++;
                        break;

                    case OperationCode.PushVal:
                        {
                            var value = currentCommand.Value;

                            var kindOfValue = value.KindOfValue;

                            switch(kindOfValue)
                            {
                                case KindOfValue.WaypointSourceValue:
                                    {
                                        var waypointSourceValue = value.AsWaypointSourceValue;

                                        value = waypointSourceValue.ConvertToWaypointValue(_context, _currentCodeFrame.LocalContext);
                                    }
                                    break;

                                case KindOfValue.ConditionalEntitySourceValue:
                                    {
                                        var conditionalEntitySourceValue = value.AsConditionalEntitySourceValue;

                                        value = conditionalEntitySourceValue.ConvertToConditionalEntityValue(_context, _currentCodeFrame.LocalContext);
                                    }
                                    break;

                                default:
                                    break;
                            }

                            _currentCodeFrame.ValuesStack.Push(value);
                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.PushValFromVar:
                        {
                            var varName = currentCommand.Value.AsStrongIdentifierValue;

#if DEBUG
                            //Log($"varName = {varName}");
#endif

                            var targetValue = _varsResolver.GetVarValue(varName, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
                            //Log($"targetValue = {targetValue}");
#endif

                            _currentCodeFrame.ValuesStack.Push(targetValue);

#if DEBUG
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.PushValToVar:
                        {
                            var varName = currentCommand.Value.AsStrongIdentifierValue;

#if DEBUG
                            //Log($"varName = {varName}");
#endif

                            if(varName.KindOfName != KindOfName.Var)
                            {
                                throw new NotImplementedException();
                            }

                            var currentValue = _currentCodeFrame.ValuesStack.Peek();

#if DEBUG
                            //Log($"currentValue = {currentValue}");
#endif
                            _varsResolver.SetVarValue(varName, currentValue, _currentCodeFrame.LocalContext);

#if DEBUG
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.VarDecl:
                        {
                            var valueStack = _currentCodeFrame.ValuesStack;

                            var annotation = valueStack.Pop();

#if DEBUG
                            //Log($"annotation = {annotation}");
#endif

                            var typesCount = currentCommand.CountParams;

#if DEBUG
                            //Log($"typesCount = {typesCount}");
#endif

                            var varPtr = new Var();

                            while(typesCount > 0)
                            {
                                var typeName = valueStack.Pop();

#if DEBUG
                                //Log($"typeName = {typeName}");
#endif

                                if(!typeName.IsStrongIdentifierValue)
                                {
                                    throw new Exception($"Typename should be StrongIdentifierValue.");
                                }

                                varPtr.TypesList.Add(typeName.AsStrongIdentifierValue);

                                typesCount--;
                            }

                            var varName = valueStack.Pop();

#if DEBUG
                            //Log($"varName = {varName}");
#endif

                            if(!varName.IsStrongIdentifierValue)
                            {
                                throw new Exception($"Varname should be StrongIdentifierValue.");
                            }

                            varPtr.Name = varName.AsStrongIdentifierValue;

#if DEBUG
                            //Log($"varPtr = {varPtr}");
#endif

                            _currentVarStorage.Append(varPtr);

#if DEBUG
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.CallBinOp:
                        {
                            var paramsList = TakePositionedParameters(3);

#if DEBUG
                            //Log($"paramsList = {paramsList.WriteListToString()}");
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            var kindOfOperator = currentCommand.KindOfOperator;

                            if(kindOfOperator == KindOfOperator.IsNot)
                            {
                                kindOfOperator = KindOfOperator.Is;
                            }

#if DEBUG
                            //Log($"kindOfOperator = {kindOfOperator}");
#endif

                            var operatorInfo = _operatorsResolver.GetOperator(kindOfOperator, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
                            //Log($"operatorInfo (1) = {operatorInfo}");
#endif

                            CallOperator(operatorInfo, paramsList);

#if DEBUG
                            //Log($"_currentCodeFrame (^) = {_currentCodeFrame.ToDbgString()}");
#endif

                            if(currentCommand.KindOfOperator == KindOfOperator.IsNot)
                            {
                                var result = _currentCodeFrame.ValuesStack.Pop();

#if DEBUG
                                //Log($"result = {result}");
#endif

                                result = result.AsLogicalValue.Inverse();

#if DEBUG
                                //Log($"result (2) = {result}");
#endif

                                _currentCodeFrame.ValuesStack.Push(result);

#if DEBUG
                                //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif
                            }

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.CallUnOp:
                        {
                            var paramsList = TakePositionedParameters(2);

#if DEBUG
                            //Log($"paramsList = {paramsList.WriteListToString()}");
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            var kindOfOperator = currentCommand.KindOfOperator;

                            var operatorInfo = _operatorsResolver.GetOperator(kindOfOperator, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
                            //Log($"operatorInfo (2)= {operatorInfo}");
#endif
                            
                            CallOperator(operatorInfo, paramsList);

#if DEBUG
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.Call:
                        CallFunction(KindOfFunctionParameters.NoParameters, currentCommand.CountParams, true);
                        break;

                    case OperationCode.Call_P:
                        CallFunction(KindOfFunctionParameters.PositionedParameters, currentCommand.CountParams, true);
                        break;

                    case OperationCode.Call_N:
                        CallFunction(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams, true);
                        break;

                    case OperationCode.AsyncCall_N:
                        CallFunction(KindOfFunctionParameters.NamedParameters, currentCommand.CountParams, false);
                        break;

                    case OperationCode.SetInheritance:
                        ProcessSetInheritance();
                        break;

                    case OperationCode.SetNotInheritance:
                        ProcessSetNotInheritance();
                        break;

                    case OperationCode.SetSEHGroup:
                        {
#if DEBUG
                            //Log($"currentCommand = {currentCommand}");
#endif

                            var targetSEHGroup = _currentCodeFrame.CompiledFunctionBody.SEH[currentCommand.TargetPosition];

#if DEBUG
                            //Log($"targetSEHGroup = {targetSEHGroup}");
#endif

                            _currentCodeFrame.CurrentSEHGroup = targetSEHGroup;
                            _currentCodeFrame.SEHStack.Push(targetSEHGroup);

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.RemoveSEHGroup:
                        {
#if DEBUG
                            //Log($"currentCommand = {currentCommand}");
#endif

                            _currentCodeFrame.SEHStack.Pop();

                            if (_currentCodeFrame.SEHStack.Count == 0)
                            {
                                _currentCodeFrame.CurrentSEHGroup = null;
                            }
                            else
                            {
                                _currentCodeFrame.CurrentSEHGroup = _currentCodeFrame.SEHStack.Peek();
                            }

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.JumpTo:
                        {
#if DEBUG
                            //Log($"currentCommand = {currentCommand}");
#endif

                            _currentCodeFrame.CurrentPosition = currentCommand.TargetPosition;
                        }
                        break;

                    case OperationCode.JumpToIfTrue:
                        JumpToIf(1, currentCommand.TargetPosition);
                        break;

                    case OperationCode.JumpToIfFalse:
                        JumpToIf(0, currentCommand.TargetPosition);
                        break;

                    case OperationCode.Await:
                        {
                            var coordinator = _currentCodeFrame.ExecutionCoordinator;

#if DEBUG
                            //Log($"coordinator = {coordinator}");
#endif

                            if (coordinator == null)
                            {
                                _currentCodeFrame.CurrentPosition++;
                                break;
                            }

                            if(coordinator.ExecutionStatus == ActionExecutionStatus.Executing)
                            {
                                break;
                            }

                            if(coordinator.ExecutionStatus == ActionExecutionStatus.Broken)
                            {
                                ProcessError(coordinator.RuleInstance);
                                break;
                            }

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.CompleteAction:
                        {
                            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Complete;

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.BreakActionVal:
                        {
                            var currentValue = _currentCodeFrame.ValuesStack.Peek();

#if DEBUG
                            //Log($"currentValue = {currentValue}");
#endif

                            var ruleInstance = currentValue.AsRuleInstanceValue.RuleInstance;

                            _executionCoordinator.RuleInstance = ruleInstance;

                            _executionCoordinator.ExecutionStatus = ActionExecutionStatus.Broken;

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.Error:
                        {
#if DEBUG
                            //Log("Begin case OperationCode.Error");
#endif

#if DEBUG
                            //Log($"currentCommand = {currentCommand}");
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            var currentValue = _currentCodeFrame.ValuesStack.Peek();

#if DEBUG
                            //Log($"currentValue = {currentValue}");
#endif

                            var ruleInstance = currentValue.AsRuleInstanceValue.RuleInstance;

                            ProcessError(ruleInstance);

#if DEBUG
                            //Log("End case OperationCode.Error");
#endif
                        }
                        break;

                    case OperationCode.Return:
                        {
#if DEBUG
                            //Log("Begin case OperationCode.Return");
#endif

                            _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Completed;

                            GoBackToPrevCodeFrame();

                            _currentCodeFrame.ValuesStack.Push(new NullValue());

#if DEBUG
                            //_instancesStorage.PrintProcessesList();

                            //Log("End case OperationCode.Return");
#endif
                        }
                        break;

                    case OperationCode.ReturnVal:
                        {
#if DEBUG
                            //Log("Begin case OperationCode.ReturnVal");
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            var currentValue = _currentCodeFrame.ValuesStack.Pop();

#if DEBUG
                            //Log($"currentValue = {currentValue}");
#endif

                            GoBackToPrevCodeFrame();

                            _currentCodeFrame.ValuesStack.Push(currentValue);

#if DEBUG
                            //_instancesStorage.PrintProcessesList();

                            //Log("End case OperationCode.Return");
#endif
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(currentCommand.OperationCode), currentCommand.OperationCode, null);
                }

                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                Error(e);
#endif
                
                throw;
            }
        }

        private void JumpToIf(float targetValue, int targetPosition)
        {
#if DEBUG
            //Log($"targetValue = {targetValue}");
#endif

            var currLogicValue = GetLogicalValueFromCurrentStackValue();

#if DEBUG
            //Log($"currLogicValue = {currLogicValue}");
#endif

            if(currLogicValue == targetValue)
            {
#if DEBUG
                //Log("currLogicValue == targetValue");
#endif

                _currentCodeFrame.CurrentPosition = targetPosition;
            }
            else
            {
#if DEBUG
                //Log("currLogicValue != targetValue");
#endif

                _currentCodeFrame.CurrentPosition++;
            }                       
        }

        private float? GetLogicalValueFromCurrentStackValue()
        {
            var currentValue = _currentCodeFrame.ValuesStack.Pop();

#if DEBUG
            //Log($"currentValue = {currentValue}");
#endif

            var kindOfValue = currentValue.KindOfValue;

            switch(kindOfValue)
            {
                case KindOfValue.LogicalValue:
                    return currentValue.AsLogicalValue.SystemValue;

                case KindOfValue.NumberValue:
                    return ValueConvertor.ConvertNumberValueToLogicalValue(currentValue.AsNumberValue, _context).SystemValue;

                case KindOfValue.RuleInstanceValue:
                    {
                        var ruleInstance = currentValue.AsRuleInstanceValue.RuleInstance;

                        var searchOptions = new LogicalSearchOptions();
                        searchOptions.QueryExpression = ruleInstance;
                        searchOptions.LocalCodeExecutionContext = _currentCodeFrame.LocalContext;

                        var searchResult = _logicalSearchResolver.Run(searchOptions);

#if DEBUG
                        //Log($"searchResult = {searchResult}");
                        //Log($"result = {DebugHelperForLogicalSearchResult.ToString(searchResult)}");
#endif

                        if(searchResult.IsSuccess)
                        {
                            return 1;
                        }

                        return 0;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfValue), kindOfValue, null);
            }
        }

        private void ProcessError(RuleInstance ruleInstance)
        {
            RegisterError(ruleInstance);

            if (_currentCodeFrame.CurrentSEHGroup == null)
            {
                _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Faulted;

                GoBackToPrevCodeFrame();
            }
            else
            {
#if DEBUG
                //Log("_currentSEHGroup != null");
#endif

                if (!CheckSEH())
                {
                    _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Faulted;

                    GoBackToPrevCodeFrame();
                }
            }
        }

        private void RegisterError(RuleInstance ruleInstance)
        {
            var errorValue = new ErrorValue(ruleInstance);
            errorValue.CheckDirty();

#if DEBUG
            //Log($"errorValue = {errorValue}");
#endif

            _currentError = errorValue;

            _globalLogicalStorage.Append(ruleInstance);
        }

        private bool CheckSEH()
        {
            var ruleInstance = _currentError.RuleInstance;

#if DEBUG
            //Log($"ruleInstance = {ruleInstance}");
#endif

            var searchOptions = new LogicalSearchOptions();
            searchOptions.TargetStorage = ruleInstance;
            searchOptions.LocalCodeExecutionContext = _currentCodeFrame.LocalContext;

            foreach (var sehItem in _currentCodeFrame.CurrentSEHGroup.Items)
            {
#if DEBUG
                //Log($"sehItem = {sehItem}");
#endif

                if(sehItem.Condition != null)
                {
                    searchOptions.QueryExpression = sehItem.Condition;

                    if (!_logicalSearchResolver.IsTruth(searchOptions))
                    {
                        continue;
                    }
                }

#if DEBUG
                //Log("NEXT");
#endif

                if(sehItem.VariableName != null && !sehItem.VariableName.IsEmpty)
                {
                    _currentVarStorage.SetValue(sehItem.VariableName, _currentError);
                }

                _currentError = null;

                _currentCodeFrame.CurrentPosition = sehItem.TargetPosition;

                return true;
            }

            _currentError = null;

            _currentCodeFrame.CurrentPosition = _currentCodeFrame.CurrentSEHGroup.AfterPosition;

            return true;
        }

        private bool CheckReturnedInfo()
        {
            if (_currentError != null)
            {
                return CheckCurrentError();
            }

            return true;
        }

        private bool CheckCurrentError()
        {
#if DEBUG
            //Log($"_currentError = {_currentError}");
#endif

            if (_currentCodeFrame.CurrentSEHGroup == null)
            {
                return false;
            }

#if DEBUG
            //Log("_currentSEHGroup != null");
#endif

            return CheckSEH();
        }

        private void SetUpCurrentCodeFrame()
        {
            _currentVarStorage = _currentCodeFrame.LocalContext.Storage.VarStorage;
        }

        private void GoBackToPrevCodeFrame()
        {
            _codeFrames.Pop();

            if (_codeFrames.Count == 0)
            {
                _currentCodeFrame = null;
            }
            else
            {
                _currentCodeFrame = _codeFrames.Peek();

#if DEBUG
                //Log($"_isCanceled = {_isCanceled}");
#endif
                
                if(_isCanceled)
                {
                    _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Canceled;

                    GoBackToPrevCodeFrame();
                    return;
                }

                SetUpCurrentCodeFrame();
            }
        }

        private List<Value> TakePositionedParameters(int count)
        {
            var result = new List<Value>();

            var valueStack = _currentCodeFrame.ValuesStack;

            for (var i = 0; i < count; i++)
            {
                result.Add(valueStack.Pop());
            }

            result.Reverse();

            return result;
        }

        private Dictionary<StrongIdentifierValue, Value> TakeNamedParameters(int count)
        {
            var rawParamsList = TakePositionedParameters(count * 2);

            var result = new Dictionary<StrongIdentifierValue, Value>();

            var enumerator = rawParamsList.GetEnumerator();

            while(enumerator.MoveNext())
            {
                var name = enumerator.Current.AsStrongIdentifierValue;

                enumerator.MoveNext();

                var value = enumerator.Current;

                result[name] = value;
            }

            return result;
        }

        private void CallFunction(KindOfFunctionParameters kindOfparameters, int parametersCount, bool isSync)
        {
#if DEBUG
            //Log($"kindOfparameters = {kindOfparameters}");
            //Log($"parametersCount = {parametersCount}");
            //Log($"isSync = {isSync}");
#endif

            var valueStack = _currentCodeFrame.ValuesStack;

            var annotation = valueStack.Pop();

#if DEBUG
            //Log($"annotation = {annotation}");
            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

            var caller = valueStack.Pop();

#if DEBUG
            //Log($"caller = {caller}");
            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

            Dictionary<StrongIdentifierValue, Value> namedParameters = null;
            List<Value> positionedParameters = null;

            switch (kindOfparameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    namedParameters = TakeNamedParameters(parametersCount);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    positionedParameters = TakePositionedParameters(parametersCount);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfparameters), kindOfparameters, null);
            }

#if DEBUG
            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
            //Log($"caller.IsPointRefValue = {caller.IsPointRefValue}");
            //Log($"caller.IsStrongIdentifierValue = {caller.IsStrongIdentifierValue}");
#endif

            if (caller.IsPointRefValue)
            {
                CallPointRefValue(caller.AsPointRefValue, kindOfparameters, namedParameters, positionedParameters, isSync);
                return;
            }

            if(caller.IsStrongIdentifierValue)
            {
                CallStrongIdentifierValue(caller.AsStrongIdentifierValue, kindOfparameters, namedParameters, positionedParameters, isSync);
                return;
            }

            throw new NotImplementedException();
        }

        private void CallPointRefValue(PointRefValue caller, 
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters, 
            bool isSync)
        {
#if DEBUG
            //Log($"caller.LeftOperand = {caller.LeftOperand}");
#endif

            if(caller.LeftOperand.IsHostValue)
            {
                CallHost(caller.RightOperand.AsStrongIdentifierValue, kindOfParameters, namedParameters, positionedParameters, isSync);
                return;
            }

            throw new NotImplementedException();
        }

        private void CallHost(StrongIdentifierValue methodName, 
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            bool isSync)
        {
#if DEBUG
            //Log($"methodName = {methodName}");
            //Log($"kindOfParameters = {kindOfParameters}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
            //Log($"isSync = {isSync}");
#endif

            var command = new Command();
            command.Name = methodName;

            switch(kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    command.ParamsDict = namedParameters.ToDictionary(p => p.Key, p => p.Value);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    command.ParamsList = positionedParameters.ToList();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

#if DEBUG
            //Log($"command = {command}");
#endif
            var processCreatingResult = _hostListener.CreateProcess(command);

#if DEBUG
            //Log($"processCreatingResult = {processCreatingResult}");
#endif

            if(processCreatingResult.IsSuccessful)
            {
                var processInfo = processCreatingResult.Process;
                processInfo.ParentProcessInfo = _currentCodeFrame.ProcessInfo;

                _instancesStorage.AppendAndTryStartProcessInfo(processInfo);

                var localExecutionCoordinator = _currentCodeFrame.ExecutionCoordinator;

                if (isSync)
                {
                    List<IExecutionCoordinator> executionCoordinators = null;

                    if(localExecutionCoordinator != null || _executionCoordinator != null)
                    {
                        executionCoordinators = new List<IExecutionCoordinator>();

                        if(localExecutionCoordinator != null)
                        {
                            executionCoordinators.Add(localExecutionCoordinator);
                        }

                        if(_executionCoordinator != null)
                        {
                            executionCoordinators.Add(_executionCoordinator);
                        }
                    }

                    ProcessInfoHelper.Wait(executionCoordinators, processInfo);

                    //Log($"localExecutionCoordinator = {localExecutionCoordinator}");

                    if(localExecutionCoordinator != null && localExecutionCoordinator.ExecutionStatus == ActionExecutionStatus.Broken)
                    {
                        ProcessError(localExecutionCoordinator.RuleInstance);

                        return;
                    }

                    var status = processInfo.Status;

#if DEBUG
                    //Log($"status = {status}");
#endif

                    switch (status)
                    {
                        case ProcessStatus.Canceled:
                            _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Canceled;
                            _isCanceled = true;

                            GoBackToPrevCodeFrame();
                            return;

                        case ProcessStatus.Faulted:
                            _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Faulted;

                            GoBackToPrevCodeFrame();
                            return;
                    }
                }

                _currentCodeFrame.CurrentPosition++;

                return;
            }

            throw new NotImplementedException();
        }

        private void CallStrongIdentifierValue(StrongIdentifierValue methodName,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters,
            bool isSync)
        {
#if DEBUG
            //Log($"methodName = {methodName}");
            //Log($"kindOfParameters = {kindOfParameters}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
            //Log($"isSync = {isSync}");
#endif
            IExecutable method = null;

            switch(kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    method = _methodsResolver.Resolve(methodName, _currentCodeFrame.LocalContext);
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    method = _methodsResolver.Resolve(methodName, namedParameters, _currentCodeFrame.LocalContext);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    method = _methodsResolver.Resolve(methodName, positionedParameters, _currentCodeFrame.LocalContext);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
            }

#if DEBUG
            //Log($"method = {method}");
#endif

            if(method == null)
            {
                throw new Exception($"Method '{methodName.NameValue}' is not found.");
            }

            CallExecutable(method, kindOfParameters, namedParameters, positionedParameters, isSync);
        }

        private CodeFrame ConvertExecutableToCodeFrame(IExecutable function,
            KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters)
        {
#if DEBUG
            //Log($"kindOfParameters = {kindOfParameters}");
            //Log($"namedParameters = {namedParameters.WriteDict_1_ToString()}");
            //Log($"positionedParameters = {positionedParameters.WriteListToString()}");
#endif

            var currentLocalContext = _currentCodeFrame.LocalContext;

            var storagesList = currentLocalContext.Storage.GetStorages();

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
            //foreach(var tmpStorage in storagesList)
            //{
            //    Log($"tmpStorage = {tmpStorage}");
            //}
#endif

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            var localStorageSettings = RealStorageSettingsHelper.Create(_context, storagesList.ToList());

            var newStorage = new LocalStorage(localStorageSettings);

            localCodeExecutionContext.Storage = newStorage;

            switch(kindOfParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    if(function.Arguments.Any())
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

            localCodeExecutionContext.Holder = currentLocalContext.Holder;

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

        private void FillUpPositionedParameters(LocalCodeExecutionContext localCodeExecutionContext, IExecutable function, List<Value> positionedParameters)
        {
            var varsStorage = localCodeExecutionContext.Storage.VarStorage;

            var positionedParametersEnumerator = positionedParameters.GetEnumerator();

            foreach (var argument in function.Arguments)
            {
#if DEBUG
                //Log($"argument = {argument}");
#endif

                if(!positionedParametersEnumerator.MoveNext())
                {
                    if(argument.HasDefaultValue)
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

            foreach(var namedParameter in namedParameters)
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

            if(usedParameters.Count < argumentsList.Count)
            {
                foreach(var argument in argumentsList)
                {
                    if(usedParameters.Contains(argument.Name))
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

        private void CallOperator(Operator op, List<Value> positionedParameters)
        {
            CallExecutable(op, positionedParameters);
        }

        private void CallExecutable(IExecutable executable, List<Value> positionedParameters)
        {
            CallExecutable(executable, KindOfFunctionParameters.PositionedParameters, null, positionedParameters, true);
        }

        private void CallExecutable(IExecutable executable, KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters, bool isSync)
        {
            if(executable == null)
            {
                throw new ArgumentNullException(nameof(executable));
            }

            var coordinator = executable.TryActivate(_context);

            if (executable.IsSystemDefined)
            {
                Value result = null;

                switch(kindOfParameters)
                {
                    case KindOfFunctionParameters.PositionedParameters:
                        result = executable.SystemHandler.Call(positionedParameters, _currentCodeFrame.LocalContext);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfParameters), kindOfParameters, null);
                }
               
#if DEBUG
                //Log($"result = {result}");
#endif

                _currentCodeFrame.ValuesStack.Push(result);

#if DEBUG
                //Log($"_currentCodeFrame (co) = {_currentCodeFrame.ToDbgString()}");
#endif

                return;
            }
            else
            {
                var newCodeFrame = ConvertExecutableToCodeFrame(executable, kindOfParameters, namedParameters, positionedParameters);

#if DEBUG
                //Log($"newCodeFrame = {newCodeFrame}");
#endif

                _context.InstancesStorage.AppendProcessInfo(newCodeFrame.ProcessInfo);

#if DEBUG
                //Log($"isSync = {isSync}");
#endif

                if (isSync)
                {
                    _currentCodeFrame.CurrentPosition++;

                    newCodeFrame.ExecutionCoordinator = coordinator;

                    SetCodeFrame(newCodeFrame);
                }
                else
                {
                    _currentCodeFrame.CurrentPosition++;

                    var threadExecutor = new AsyncThreadExecutor(_context);
                    threadExecutor.SetCodeFrame(newCodeFrame);

                    var task = threadExecutor.Start();

                    _currentCodeFrame.ValuesStack.Push(task);
                }
            }
        }

        private void ProcessSetInheritance()
        {
            var paramsList = TakePositionedParameters(4);

#if DEBUG
            //Log($"paramsList = {paramsList.WriteListToString()}");
            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

            var inheritenceItem = new InheritanceItem();
            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(inheritenceItem, _currentCodeFrame.LocalContext.Storage.DefaultSettingsOfCodeEntity);

            var subName = _strongIdentifierLinearResolver.Resolve(paramsList[0], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

            var superName = _strongIdentifierLinearResolver.Resolve(paramsList[1], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
            //Log($"paramsList[2] = {paramsList[2]}");
#endif

            var rank = paramsList[2];//_logicalValueLinearResolver.Resolve(paramsList[2], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions(), true);

#if DEBUG
            //Log($"subName = {subName}");
            //Log($"superName = {superName}");
            //Log($"rank = {rank}");
#endif

            inheritenceItem.SubName = subName;
            inheritenceItem.SuperName = superName;
            inheritenceItem.Rank = rank;

#if DEBUG
            //Log($"inheritenceItem = {inheritenceItem}");
#endif

            _globalStorage.InheritanceStorage.SetInheritance(inheritenceItem);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessSetNotInheritance()
        {
            var paramsList = TakePositionedParameters(4);

#if DEBUG
            //Log($"paramsList = {paramsList.WriteListToString()}");
            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

            var inheritenceItem = new InheritanceItem();
            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(inheritenceItem, _currentCodeFrame.LocalContext.Storage.DefaultSettingsOfCodeEntity);

            var subName = paramsList[0].AsStrongIdentifierValue;

            var superName = paramsList[1].AsStrongIdentifierValue;

#if DEBUG
            //Log($"paramsList[2] = {paramsList[2]}");
#endif

            var rank = _logicalValueLinearResolver.Resolve(paramsList[2], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions(), true).Inverse();

#if DEBUG
            //Log($"subName = {subName}");
            //Log($"superName = {superName}");
            //Log($"rank = {rank}");
#endif

            inheritenceItem.SubName = subName;
            inheritenceItem.SuperName = superName;
            inheritenceItem.Rank = rank;

#if DEBUG
            //Log($"inheritenceItem = {inheritenceItem}");
#endif

            _globalStorage.InheritanceStorage.SetInheritance(inheritenceItem);

            _currentCodeFrame.CurrentPosition++;
        }
    }
}

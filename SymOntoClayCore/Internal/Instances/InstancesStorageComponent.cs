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

using NLog;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Instances
{
    public class InstancesStorageComponent: BaseInstancesStorageComponent
    {
        public InstancesStorageComponent(IEngineContext context)
            : base(context)
        {
            _context = context;

            _metadataResolver = context.DataResolversFactory.GetMetadataResolver();

            _projectLoader = new ProjectLoader(context);

            OnIdle += DispatchOnIdle;
        }

        private readonly IEngineContext _context;
        private readonly MetadataResolver _metadataResolver;

        private readonly object _registryLockObj = new object();
        private readonly object _processLockObj = new object();

        private readonly ProjectLoader _projectLoader;

        private Dictionary<StrongIdentifierValue, AppInstance> _namesDict;
        private AppInstance _rootInstanceInfo;

        private List<IProcessInfo> _processesInfoList;
        private Dictionary<int, IProcessInfo> _processesInfoByDevicesDict = new Dictionary<int, IProcessInfo>();
        
        /// <inheritdoc/>
        public override void LoadFromSourceFiles()
        {
            _namesDict = new Dictionary<StrongIdentifierValue, AppInstance>();
            _rootInstanceInfo = null;

            _processesInfoList = new List<IProcessInfo>();
            _processesInfoByDevicesDict = new Dictionary<int, IProcessInfo>();

            var globalStorage = _context.Storage.GlobalStorage;

            globalStorage.VarStorage.SetSystemValue(Logger, _context.CommonNamesStorage.HostSystemVarName, new HostValue());
        }

        /// <inheritdoc/>
        public override void ActivateMainEntity(IMonitorLogger logger)
        {
            var globalStorage = _context.Storage.GlobalStorage;

            var mainEntity = GetOrCreateMainEntity(logger);

            if(mainEntity == null)
            {
                return;
            }

            AppInstance instanceInfo;

            lock (_registryLockObj)
            {
                if (_rootInstanceInfo != null)
                {
                    throw new NotImplementedException();
                }

                var mainEntityName = mainEntity.Name;

                if (_namesDict.ContainsKey(mainEntityName))
                {
                    throw new NotImplementedException();
                }

                instanceInfo = new AppInstance(codeItem: mainEntity, context: _context, parentStorage: globalStorage);
                _rootInstanceInfo = instanceInfo;
                _namesDict[mainEntityName] = instanceInfo;

                globalStorage.VarStorage.SetSystemValue(logger, _context.CommonNamesStorage.SelfSystemVarName, new InstanceValue(instanceInfo));
            }

            instanceInfo.Init(logger);

            Task.Run(() => {
                try
                {
                    Thread.Sleep(100);

                    DispatchIdleActions(logger);
                }
                catch (Exception e)
                {
                    logger.Error("0E28EFF2-FA4B-46F2-A1B5-7CDF5C9B4862", e);
                }
            });
        }

        private void DispatchOnIdle()
        {
            Task.Run(() => {
                try
                {
                    DispatchIdleActions(Logger);
                }
                catch (Exception e)
                {
                    Error("1617E55E-58BC-4B63-9418-D5967AD99678", e);
                }
            });
        }

        private void DispatchIdleActions(IMonitorLogger logger)
        {
            var count = GetCountOfCurrentProcesses(logger);

            if(count > 0)
            {
                return;
            }

            var rawListOfTopIndependentInstances = _rootInstanceInfo.GetTopIndependentInstances(logger);

            if(rawListOfTopIndependentInstances.Count == 1)
            {
                rawListOfTopIndependentInstances.First().ActivateIdleAction(logger);
                return;
            }

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override AppInstance MainEntity => _rootInstanceInfo;

        /// <inheritdoc/>
        public override void ActivateState(IMonitorLogger logger, StateDef state)
        {
            _rootInstanceInfo.ActivateState(logger, state);
        }

        /// <inheritdoc/>
        public override void TryActivateDefaultState(IMonitorLogger logger)
        {
            _rootInstanceInfo.TryActivateDefaultState(logger);
        }

        /// <inheritdoc/>
        public override void AppendProcessInfo(IMonitorLogger logger, IProcessInfo processInfo)
        {
            lock(_processLockObj)
            {
                NTryAppendProcessInfo(logger, processInfo);
            }
        }

        /// <inheritdoc/>
        public override void AppendAndTryStartProcessInfo(IMonitorLogger logger, string callMethodId, IProcessInfo processInfo)
        {
            logger.HostMethodStarting("AC8691B4-D646-4844-B15A-7C18DD1E665E", callMethodId);

            if (processInfo.Devices.IsNullOrEmpty())
            {
                NAppendAndTryStartProcessInfoWithoutDevices(logger, processInfo);

                logger.EndHostMethodStarting("BDBADA62-94E7-416B-A266-C7AFA5DE7BEA", callMethodId);

                return;
            }

            processInfo.Status = ProcessStatus.WaitingForResources;

            lock (_processLockObj)
            {
                if(!NTryAppendProcessInfo(logger, processInfo))
                {
                    logger.EndHostMethodStarting("83DE51D1-341C-4E4E-A9BA-630C68495F8F", callMethodId);

                    return;
                }

                var concurentProcessesInfoList = NGetConcurrentProcessesInfo(logger, processInfo);

#if DEBUG
                //Log($"concurentProcessesInfoList?.Count = {concurentProcessesInfoList?.Count}");
#endif

                if(concurentProcessesInfoList.IsNullOrEmpty())
                {
                    NAppendAndTryStartProcessInfoWithDevices(logger, processInfo);

                    logger.EndHostMethodStarting("FF147E42-AB42-4CA3-95CF-A68A7787B69F", callMethodId);

                    return;
                }

#if DEBUG
                //foreach(var tmpProcessInfo in concurentProcessesInfoList)
                //{
                //    Log($"tmpProcessInfo.EndPointName = {tmpProcessInfo.EndPointName};tmpProcessInfo.GlobalPriority = {tmpProcessInfo.GlobalPriority}");
                //}
#endif

                if (concurentProcessesInfoList.All(p => p.ParentProcessInfo == processInfo.ParentProcessInfo))
                {
                    NAppendAndTryStartProcessInfoWithDevices(logger, processInfo);

                    Task.Run(() => {
                        foreach (var concurentProcessInfo in concurentProcessesInfoList)
                        {
                            concurentProcessInfo.Cancel(logger);
                        }
                    });

                    logger.EndHostMethodStarting("CE34E46B-9E41-419C-87B3-4CC0A172B8B4", callMethodId);

                    return;
                }

                var globalPriority = processInfo.GlobalPriority;

#if DEBUG
                //Log($"globalPriority = {globalPriority}");
#endif

                if (concurentProcessesInfoList.All(p => p.GlobalPriority >= globalPriority))
                {
                    NAppendAndTryStartProcessInfoWithDevices(logger, processInfo);

                    Task.Run(() => { 
                        foreach(var concurentProcessInfo in concurentProcessesInfoList)
                        {
                            concurentProcessInfo.Cancel(logger);
                        }
                    });

                    logger.EndHostMethodStarting("743973AD-3272-42BB-B832-0F7EB63AE9E1", callMethodId);

                    return;
                }
            }

            processInfo.Cancel(logger);

            logger.EndHostMethodStarting("1510E6AF-F4EB-4BA3-AC44-78BDE3E92EE7", callMethodId);
        }

        private bool NTryAppendProcessInfo(IMonitorLogger logger, IProcessInfo processInfo)
        {
            if (_processesInfoList.Contains(processInfo))
            {
                return false;
            }

            _processesInfoList.Add(processInfo);

            processInfo.OnFinish += OnFinishProcessWithoutDevicesHandler;

            return true;
        }

        private void OnFinishProcessWithoutDevicesHandler(IProcessInfo sender)
        {
            lock (_processLockObj)
            {
                _processesInfoList.Remove(sender);

                CheckCountOfActiveProcesses(Logger);
            }
        }

        private void NAppendAndTryStartProcessInfoWithoutDevices(IMonitorLogger logger, IProcessInfo processInfo)
        {
            AppendProcessInfo(logger, processInfo);
            processInfo.Start(logger);
        }

        private void NAppendAndTryStartProcessInfoWithDevices(IMonitorLogger logger, IProcessInfo processInfo)
        {
            foreach (var device in processInfo.Devices)
            {
                _processesInfoByDevicesDict[device] = processInfo;
            }

            processInfo.OnFinish += OnFinishProcessWithDevicesHandler;

            processInfo.Start(logger);

        }

        private void OnFinishProcessWithDevicesHandler(IProcessInfo sender)
        {
            lock (_processLockObj)
            {
                foreach (var device in sender.Devices)
                {
                    if(_processesInfoByDevicesDict.ContainsKey(device) && _processesInfoByDevicesDict[device] == sender)
                    {
                        _processesInfoByDevicesDict.Remove(device);
                    }
                }

                CheckCountOfActiveProcesses(Logger);
            }

        }

        private void CheckCountOfActiveProcesses(IMonitorLogger logger)
        {
            var count = NGetCountOfCurrentProcesses(logger);

            if(count == 0)
            {
                try
                {
                    OnIdle?.Invoke();
                }
                catch
                {
                }
            }
        }

        private int NGetCountOfCurrentProcesses(IMonitorLogger logger)
        {
            return _processesInfoByDevicesDict.Count + _processesInfoList.Count; 
        }

        /// <inheritdoc/>
        public override event Action OnIdle;

        /// <inheritdoc/>
        public override int GetCountOfCurrentProcesses(IMonitorLogger logger)
        {
            lock (_processLockObj)
            {
                return NGetCountOfCurrentProcesses(logger);
            }
        }

        private List<IProcessInfo> NGetConcurrentProcessesInfo(IMonitorLogger logger, IProcessInfo processInfo)
        {
            var result = new List<IProcessInfo>();

            foreach(var device in processInfo.Devices)
            {
                if (_processesInfoByDevicesDict.ContainsKey(device))
                {
                    var otherProcessInfo = _processesInfoByDevicesDict[device];

                    if(!processInfo.IsFriend(logger, otherProcessInfo))
                    {
                        result.Add(otherProcessInfo);
                    }                    
                }
            }

            return result.Distinct().ToList();
        }

        /// <inheritdoc/>
        public override Value CreateInstance(IMonitorLogger logger, StrongIdentifierValue prototypeName, ILocalCodeExecutionContext executionContext)
        {
            var codeItem = _metadataResolver.Resolve(logger, prototypeName, executionContext);

            if(codeItem == null)
            {
                throw new NotImplementedException();
            }

            return NCreateInstance(logger, codeItem, executionContext, false);
        }

        /// <inheritdoc/>
        public override Value CreateInstance(IMonitorLogger logger, InstanceValue instanceValue, ILocalCodeExecutionContext executionContext)
        {
            var codeItem = _metadataResolver.Resolve(logger, instanceValue.InstanceInfo.Name, executionContext);

            if (codeItem == null)
            {
                throw new NotImplementedException();
            }

            return NCreateInstance(logger, codeItem, executionContext, false);
        }

        /// <inheritdoc/>
        public override Value CreateInstance(IMonitorLogger logger, CodeItem codeItem, ILocalCodeExecutionContext executionContext)
        {
            return NCreateInstance(logger, codeItem, executionContext, true);
        }

        private Value NCreateInstance(IMonitorLogger logger, CodeItem codeItem, ILocalCodeExecutionContext executionContext, bool loadCodeItem)
        {
            var targetCodeItem = CreateAndSaveInstanceCodeItem(logger, codeItem, NameHelper.CreateEntityName());

            var instance = new ObjectInstance(targetCodeItem, _context, executionContext.Storage, executionContext);

            if(loadCodeItem)
            {
                _projectLoader.LoadCodeItem(logger, codeItem, executionContext.Storage);
            }

            instance.Init(logger);

            var instanceValue = new InstanceValue(instance);
            instanceValue.CheckDirty();

            return instanceValue;
        }

        /// <inheritdoc/>
        public override Value CreateInstance(IMonitorLogger logger, ActionPtr actionPtr, ILocalCodeExecutionContext executionContext, IExecutionCoordinator executionCoordinator)
        {
            var action = actionPtr.Action;

            var targetCodeItem = CreateAndSaveInstanceCodeItem(logger, action, NameHelper.CreateEntityName());

            var actionInstance = new ActionInstance(targetCodeItem, actionPtr, _context, executionContext.Storage, executionContext, executionCoordinator);

            actionInstance.Init(logger);

            var instanceValue = new ActionInstanceValue(actionInstance);
            instanceValue.CheckDirty();

            return instanceValue;
        }

#if DEBUG
        /// <inheritdoc/>
        public override void PrintProcessesList(IMonitorLogger logger)
        {
            List<IProcessInfo> tmpProcessesInfoList;

            lock (_processLockObj)
            {
                tmpProcessesInfoList = _processesInfoList.ToList();
            }

            uint n = DisplayHelper.IndentationStep;

            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine("Begin ProcessesList");

            foreach(var processInfo in tmpProcessesInfoList)
            {
                sb.AppendLine($"{spaces}{processInfo.Id} {processInfo.Status} {processInfo.Priority} {processInfo.GlobalPriority}");
            }

            sb.AppendLine("End ProcessesList");

            logger.Info("64404666-03BB-4163-996C-7813E654FD6F", sb.ToString());
        }
#endif
        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            foreach (var processInfo in _processesInfoList.ToList())
            {
                processInfo.Dispose();
            }

            base.OnDisposed();
        }
    }
}

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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
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

            ThreadTask.Run(() => {
                var taskId = logger.StartTask("B38CE583-4AE0-42ED-9043-59E038AF094E");

                try
                {
                    Thread.Sleep(100);

                    DispatchIdleActions(logger);
                }
                catch (Exception e)
                {
                    logger.Error("0E28EFF2-FA4B-46F2-A1B5-7CDF5C9B4862", e);
                }

                logger.StopTask("54E83ADE-6CC4-4655-A1D8-68FDA40DBB22", taskId);
            }, _context.AsyncEventsThreadPool, _context.GetCancellationToken());
        }

        private void DispatchOnIdle()
        {
            ThreadTask.Run(() => {
                var taskId = Logger.StartTask("F3A7C7F7-1D36-4321-9467-B5E075A04E6F");

                try
                {
                    DispatchIdleActions(Logger);
                }
                catch (Exception e)
                {
                    Error("1617E55E-58BC-4B63-9418-D5967AD99678", e);
                }

                Logger.StopTask("97C0FBD9-7D7B-4E6B-BEA5-1D6D37FE2005", taskId);
            }, _context.AsyncEventsThreadPool, _context.GetCancellationToken());
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

            processInfo.SetStatus(logger, "1B56EBB2-08E8-47EC-AB31-528709F7A93F", ProcessStatus.WaitingForResources);

            lock (_processLockObj)
            {
                if(!NTryAppendProcessInfo(logger, processInfo))
                {
                    logger.EndHostMethodStarting("83DE51D1-341C-4E4E-A9BA-630C68495F8F", callMethodId);

                    return;
                }

                var concurentProcessesInfoList = NGetConcurrentProcessesInfo(logger, processInfo);

                logger.SystemExpr("CDB5B558-4B52-45B6-8895-1363D5C9CC9A", callMethodId, "concurentProcessesInfoList?.Count", concurentProcessesInfoList?.Count);

#if DEBUG
                //Log($"concurentProcessesInfoList?.Count = {concurentProcessesInfoList?.Count}");
                //Info("DE893262-B4D1-4E7D-AA83-418A86546F3B", processInfo.WriteIProcessInfoToToHumanizedString());
#endif

                if (concurentProcessesInfoList.IsNullOrEmpty())
                {
                    NAppendAndTryStartProcessInfoWithDevices(logger, processInfo);

                    logger.EndHostMethodStarting("FF147E42-AB42-4CA3-95CF-A68A7787B69F", callMethodId);

                    return;
                }

                logger.SystemExpr("D7883965-350D-4E3A-9031-FB5FF76E46D1", callMethodId, "processInfo", processInfo.WriteIProcessInfoToToHumanizedString(logger));

                //logger.SystemExpr("42E919AE-92D5-4CD9-8C01-2A4F58AC677F", callMethodId, "processInfo.Id", processInfo.Id);
                //logger.SystemExpr("0FD04996-7061-45EA-BB11-8FA24E3EECE5", callMethodId, "processInfo.EndPointName", processInfo.EndPointName);
                //logger.SystemExpr("E92FEE12-B3FC-421C-8F26-E0B7437BE000", callMethodId, "processInfo.ParentProcessInfo?.Id", processInfo.ParentProcessInfo?.Id);
                //logger.SystemExpr("95926019-C495-4C1F-B7F5-70ECF36FCDD6", callMethodId, "processInfo.ParentProcessInfo?.EndPointName", processInfo.ParentProcessInfo?.EndPointName);
                //logger.SystemExpr("753A6B07-2245-4665-AFED-D604167812E0", callMethodId, "processInfo.Priority", processInfo.Priority);
                //logger.SystemExpr("D68CB34F-4DFC-4E5D-B195-E7CFF77E6FCD", callMethodId, "processInfo.GlobalPriority", processInfo.GlobalPriority);

                foreach (var concurentProcessInfo in concurentProcessesInfoList)
                {
                    logger.SystemExpr("A815BA9A-C55F-4342-BE3D-1356FC79F277", callMethodId, "concurentProcessInfo", concurentProcessInfo.WriteIProcessInfoToToHumanizedString(logger));
                    //logger.SystemExpr("FE0EF35D-0350-40FE-9CBB-74A5A621A0D7", callMethodId, "concurentProcessInfo.Id", concurentProcessInfo.Id);
                    //logger.SystemExpr("24ACC2B2-813D-4C0E-B090-1A4F93589311", callMethodId, "concurentProcessInfo.EndPointName", concurentProcessInfo.EndPointName);
                    //logger.SystemExpr("85D18388-F4AC-4C83-82B5-5B6BC1574170", callMethodId, "concurentProcessInfo.ParentProcessInfo?.Id", concurentProcessInfo.ParentProcessInfo?.Id);
                    //logger.SystemExpr("B1C490A2-54A9-49D9-9B0D-D474B37784A8", callMethodId, "concurentProcessInfo.ParentProcessInfo?.EndPointName", concurentProcessInfo.ParentProcessInfo?.EndPointName);
                    //logger.SystemExpr("DC95F5D5-2D3F-41F9-B115-E824919E4EF9", callMethodId, "concurentProcessInfo.Priority", concurentProcessInfo.Priority);
                    //logger.SystemExpr("4FD9E70B-C7A0-4BD6-8831-C9D189016C64", callMethodId, "concurentProcessInfo.GlobalPriority", concurentProcessInfo.GlobalPriority);
                }

#if DEBUG
                //foreach(var tmpProcessInfo in concurentProcessesInfoList)
                //{
                //    Log($"tmpProcessInfo.EndPointName = {tmpProcessInfo.EndPointName};tmpProcessInfo.GlobalPriority = {tmpProcessInfo.GlobalPriority}");
                //}
#endif

#if DEBUG
                //Log($"globalPriority = {globalPriority}");
#endif

                if(concurentProcessesInfoList.All(y => ProcessInfoHelper.Compare(logger, processInfo, y) >=0))
                {
                    NAppendAndTryStartProcessInfoWithDevices(logger, processInfo);

                    ThreadTask.Run(() =>
                    {
                        var taskId = logger.StartTask("EDE8166B-FDC9-4DDB-8D5F-76CF87E61A4C");

                        foreach (var concurentProcessInfo in concurentProcessesInfoList)
                        {
                            concurentProcessInfo.Cancel(logger, "74AFBE25-AE89-4971-BC06-716048826194", ReasonOfChangeStatus.ByConcurrentProcess, new Changer(KindOfChanger.ProcessInfo, processInfo.Id), callMethodId);
                        }

                        logger.StopTask("594DAE42-0F2E-42AC-8DA4-0F406DCC227A", taskId);
                    }, _context.AsyncEventsThreadPool, _context.GetCancellationToken());

                    logger.EndHostMethodStarting("A919AF23-C7E3-4678-97AA-1E2E596B9EE1", callMethodId);

                    return;
                }
            }

            processInfo.Cancel(logger, "1EC35613-F68C-4D31-AA72-0B07F9FD10B7", ReasonOfChangeStatus.CouldNotBeStartedByLowPriority, callMethodId: callMethodId);

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
            processInfo.Start(logger, "D2165E11-EE8E-4802-A01E-F01A2F523DA3");
        }

        private void NAppendAndTryStartProcessInfoWithDevices(IMonitorLogger logger, IProcessInfo processInfo)
        {
            foreach (var device in processInfo.Devices)
            {
                _processesInfoByDevicesDict[device] = processInfo;
            }

            processInfo.OnFinish += OnFinishProcessWithDevicesHandler;

            processInfo.Start(logger, "5E6C3B30-3111-49AC-BE18-2CCC6C523277");

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
                sb.AppendLine($"{spaces}{processInfo.Id} {processInfo.Status} [{processInfo.Priority}]");
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

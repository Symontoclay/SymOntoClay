/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
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
        }

        private readonly IEngineContext _context;
        private readonly object _registryLockObj = new object();
        private readonly object _processLockObj = new object();

        private Dictionary<StrongIdentifierValue, InstanceInfo> _namesDict;
        private InstanceInfo _rootInstanceInfo;

        private List<IProcessInfo> _processesInfoList;
        private Dictionary<int, IProcessInfo> _processesInfoByDevicesDict = new Dictionary<int, IProcessInfo>();

        /// <inheritdoc/>
        public override void LoadFromSourceFiles()
        {
#if DEBUG
            //Log("Begin");
#endif

            _namesDict = new Dictionary<StrongIdentifierValue, InstanceInfo>();
            _rootInstanceInfo = null;

            _processesInfoList = new List<IProcessInfo>();
            _processesInfoByDevicesDict = new Dictionary<int, IProcessInfo>();

            var globalStorage = _context.Storage.GlobalStorage;

            globalStorage.VarStorage.SetSystemValue(_context.CommonNamesStorage.HostSystemVarName, new HostValue());

#if IMAGINE_WORKING
            //Log("End");
#else
            throw new NotImplementedException();
#endif
        }

        /// <inheritdoc/>
        public override void ActivateMainEntity()
        {
            var globalStorage = _context.Storage.GlobalStorage;

            var mainEntity = GetOrCreateMainEntity();

#if DEBUG
            //Log($"NEXT mainEntity = {mainEntity}");
#endif

            InstanceInfo instanceInfo;

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

                instanceInfo = new InstanceInfo(name: mainEntity.Name, context: _context, parentStorage: globalStorage);
                _rootInstanceInfo = instanceInfo;
                _namesDict[mainEntityName] = instanceInfo;

                globalStorage.VarStorage.SetSystemValue(_context.CommonNamesStorage.SelfSystemVarName, new InstanceValue(instanceInfo));
            }

#if DEBUG
            //Log($"instanceInfo = {instanceInfo}");
#endif
            instanceInfo.Init();
        }

        /// <inheritdoc/>
        public override void AppendProcessInfo(IProcessInfo processInfo)
        {
            lock(_processLockObj)
            {
#if DEBUG
                //Log($"processInfo = {processInfo}");
#endif

                NTryAppendProcessInfo(processInfo);
            }
        }

        /// <inheritdoc/>
        public override void AppendAndTryStartProcessInfo(IProcessInfo processInfo)
        {
            if(processInfo.Devices.IsNullOrEmpty())
            {
                NAppendAndTryStartProcessInfoWithoutDevices(processInfo);
                return;
            }

            processInfo.Status = ProcessStatus.WaitingForResources;

            lock (_processLockObj)
            {
#if DEBUG
                //Log($"processInfo = {processInfo}");
#endif

                if(!NTryAppendProcessInfo(processInfo))
                {
                    return;
                }

                var concurentProcessesInfoList = NGetConcurrentProcessesInfo(processInfo);

#if DEBUG
                Log($"concurentProcessesInfoList = {concurentProcessesInfoList.WriteListToString()}");
#endif

                if(concurentProcessesInfoList.IsNullOrEmpty())
                {
                    NAppendAndTryStartProcessInfoWithDevices(processInfo);
                    return;
                }

                if(concurentProcessesInfoList.All(p => p.ParentProcessInfo == processInfo.ParentProcessInfo))
                {
#if DEBUG
                    Log("concurentProcessesInfoList.All(p => p.ParentProcessInfo == processInfo.ParentProcessInfo)");
#endif

                    NAppendAndTryStartProcessInfoWithDevices(processInfo);

                    Task.Run(() => {
                        foreach (var concurentProcessInfo in concurentProcessesInfoList)
                        {
                            concurentProcessInfo.Cancel();
                        }
                    });

#if DEBUG
                    //Log("concurentProcessesInfoList.All(p => p.ParentProcessInfo == processInfo.ParentProcessInfo) return;");
#endif

                    return;
                }

                var globalPriority = processInfo.GlobalPriority;

                if (concurentProcessesInfoList.All(p => p.GlobalPriority >= globalPriority))
                {
#if DEBUG
                    //Log("concurentProcessesInfoList.All(p => p.GlobalPriority >= globalPriority)");
#endif

                    NAppendAndTryStartProcessInfoWithDevices(processInfo);

                    Task.Run(() => { 
                        foreach(var concurentProcessInfo in concurentProcessesInfoList)
                        {
                            concurentProcessInfo.Cancel();
                        }
                    });

#if DEBUG
                    //Log("concurentProcessesInfoList.All(p => p.GlobalPriority >= globalPriority) return;");
#endif

                    return;
                }
            }

            processInfo.Cancel();
        }

        private bool NTryAppendProcessInfo(IProcessInfo processInfo)
        {
            if (_processesInfoList.Contains(processInfo))
            {
                return false;
            }

            _processesInfoList.Add(processInfo);

            return true;
        }

        private void NAppendAndTryStartProcessInfoWithoutDevices(IProcessInfo processInfo)
        {
            AppendProcessInfo(processInfo);
            processInfo.Start();
        }

        private void NAppendAndTryStartProcessInfoWithDevices(IProcessInfo processInfo)
        {
#if DEBUG
            //Log($"processInfo = {processInfo}");
#endif

            foreach (var device in processInfo.Devices)
            {
                _processesInfoByDevicesDict[device] = processInfo;
            }

            processInfo.OnFinish += OnFinishProcessWithDevicesHandler;

            processInfo.Start();

#if DEBUG
            //Log($"processInfo.Start()");
#endif
        }

        private void OnFinishProcessWithDevicesHandler(IProcessInfo sender)
        {
#if DEBUG
            //Log($"sender = {sender}");
#endif
            lock (_processLockObj)
            {
                foreach (var device in sender.Devices)
                {
                    if(_processesInfoByDevicesDict.ContainsKey(device) && _processesInfoByDevicesDict[device] == sender)
                    {
                        _processesInfoByDevicesDict.Remove(device);
                    }
                }
            }

#if DEBUG
            //Log("End");
#endif
        }

        private List<IProcessInfo> NGetConcurrentProcessesInfo(IProcessInfo processInfo)
        {
            var result = new List<IProcessInfo>();

            foreach(var device in processInfo.Devices)
            {
#if DEBUG
                Log($"device = {device}");
#endif

                if (_processesInfoByDevicesDict.ContainsKey(device))
                {
                    var otherProcessInfo = _processesInfoByDevicesDict[device];

#if DEBUG
                    Log("_processesInfoByDevicesDict.ContainsKey(device)");
                    Log($"otherProcessInfo = {otherProcessInfo}");
                    Log($"processInfo.IsFriend(otherProcessInfo) = {processInfo.IsFriend(otherProcessInfo)}");
#endif

                    if(!processInfo.IsFriend(otherProcessInfo))
                    {
                        result.Add(otherProcessInfo);
                    }                    
                }
            }

            return result.Distinct().ToList();
        }

#if DEBUG
        /// <inheritdoc/>
        public override void PrintProcessesList()
        {
            List<IProcessInfo> tmpProcessesInfoList;

            lock (_processLockObj)
            {
                tmpProcessesInfoList = _processesInfoList.ToList();
            }

            uint n = 4;

            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine("Begin ProcessesList");

            foreach(var processInfo in tmpProcessesInfoList)
            {
                sb.AppendLine($"{spaces}{processInfo.Id} {processInfo.Status} {processInfo.Priority} {processInfo.GlobalPriority}");
            }

            sb.AppendLine("End ProcessesList");

            Log(sb.ToString());
        }
#endif
        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            foreach (var processInfo in _processesInfoList)
            {
                processInfo.Dispose();
            }

            base.OnDisposed();
        }
    }
}

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
    public class InstancesStorageComponent : BaseComponent, IInstancesStorageComponent
    {
        public InstancesStorageComponent(IEngineContext context)
            : base(context.Logger)
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

        public void LoadFromSourceFiles()
        {
#if DEBUG
            //Log("Begin");
#endif

            _namesDict = new Dictionary<StrongIdentifierValue, InstanceInfo>();
            _rootInstanceInfo = null;

            _processesInfoList = new List<IProcessInfo>();
            _processesInfoByDevicesDict = new Dictionary<int, IProcessInfo>();

            var globalStorage = _context.Storage.GlobalStorage;

            globalStorage.VarStorage.SetSystemValue(_context.CommonNamesStorage.IndexedHostSystemVarName, new HostValue().GetIndexedValue(_context));

#if IMAGINE_WORKING
            //Log("End");
#else
            throw new NotImplementedException();
#endif
        }

        /// <inheritdoc/>
        public void ActivateMainEntity()
        {
            var globalStorage = _context.Storage.GlobalStorage;

            var mainEntity = globalStorage.MetadataStorage.MainCodeEntity;

#if DEBUG
            //Log($"mainEntity = {mainEntity}");
#endif

            if (mainEntity.Name.KindOfName == KindOfName.Entity)
            {
                if(mainEntity.Name.NameValue != _context.Id)
                {
                    throw new Exception("Id of main entity is invalid");
                }
            }
            else
            {
                mainEntity = CreateAndSaveEntity(mainEntity);
            }

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

                globalStorage.VarStorage.SetSystemValue(_context.CommonNamesStorage.IndexedSelfSystemVarName, new InstanceValue(instanceInfo).GetIndexedValue(_context));
            }

#if DEBUG
            //Log($"instanceInfo = {instanceInfo}");
#endif
            instanceInfo.Init();
        }

        private CodeEntity CreateAndSaveEntity(CodeEntity superCodeEntity)
        {
            var result = new CodeEntity();
            result.Kind = KindOfCodeEntity.Instance;

            var newName = NameHelper.CreateName(_context.Id, _context.Dictionary);
            result.Name = newName;

            result.Holder = _context.CommonNamesStorage.DefaultHolder;

            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            inheritanceItem.SubName = newName;
            inheritanceItem.SuperName = superCodeEntity.Name;
            inheritanceItem.Rank = new LogicalValue(1.0F);

            result.InheritanceItems.Add(inheritanceItem);

#if DEBUG
            //Log($"result = {result}");
#endif

            var globalStorage = _context.Storage.GlobalStorage;

            globalStorage.MetadataStorage.Append(result);
            globalStorage.InheritanceStorage.SetInheritance(inheritanceItem);

            return result;
        }

        /// <inheritdoc/>
        public void AppendProcessInfo(IProcessInfo processInfo)
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
        public void AppendAndTryStartProcessInfo(IProcessInfo processInfo)
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
                //Log($"concurentProcessesInfoList = {concurentProcessesInfoList.WriteListToString()}");
#endif

                if(concurentProcessesInfoList.IsNullOrEmpty())
                {
                    NAppendAndTryStartProcessInfoWithDevices(processInfo);
                    return;
                }

                if(concurentProcessesInfoList.All(p => p.ParentProcessInfo == processInfo.ParentProcessInfo))
                {
#if DEBUG
                    //Log("concurentProcessesInfoList.All(p => p.ParentProcessInfo == processInfo.ParentProcessInfo)");
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
            foreach (var device in processInfo.Devices)
            {
                _processesInfoByDevicesDict[device] = processInfo;
            }

            processInfo.OnFinish += OnFinishProcessWithDevicesHandler;

            processInfo.Start();
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
                if(_processesInfoByDevicesDict.ContainsKey(device))
                {
                    result.Add(_processesInfoByDevicesDict[device]);
                }
            }

            return result.Distinct().ToList();
        }

#if DEBUG
        /// <inheritdoc/>
        public void PrintProcessesList()
        {
            List<IProcessInfo> tmpPprocessesInfoList;

            lock (_processLockObj)
            {
                tmpPprocessesInfoList = _processesInfoList.ToList();
            }

            uint n = 4;

            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine("Begin ProcessesList");

            foreach(var processInfo in tmpPprocessesInfoList)
            {
                sb.AppendLine($"{spaces}{processInfo.Id} {processInfo.Status} {processInfo.Priority} {processInfo.GlobalPriority}");
            }

            sb.AppendLine("End ProcessesList");

            Log(sb.ToString());
        }
#endif
    }
}

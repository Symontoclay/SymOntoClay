using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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

        private Dictionary<Name, InstanceInfo> _namesDict;
        private InstanceInfo _rootInstanceInfo;

        public void LoadFromSourceFiles()
        {
#if DEBUG
            //Log("Begin");
#endif

            _namesDict = new Dictionary<Name, InstanceInfo>();
            _rootInstanceInfo = null;

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
    }
}

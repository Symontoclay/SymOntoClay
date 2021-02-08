using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class BaseInstancesStorageComponent : BaseComponent, IInstancesStorageComponent
    {
        public BaseInstancesStorageComponent(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        public virtual void LoadFromSourceFiles()
        {
        }

        /// <inheritdoc/>
        public virtual void ActivateMainEntity()
        {
            GetOrCreateMainEntity();
        }

        protected CodeEntity GetOrCreateMainEntity()
        {
            var globalStorage = _context.Storage.GlobalStorage;

            var mainEntity = globalStorage.MetadataStorage.MainCodeEntity;

#if DEBUG
            //Log($"mainEntity = {mainEntity}");
#endif

            if(mainEntity == null)
            {
                return null;
            }

            if (mainEntity.Name.KindOfName == KindOfName.Entity)
            {
                if (mainEntity.Name.NameValue != _context.Id)
                {
                    throw new Exception("Id of main entity is invalid");
                }
            }
            else
            {
                mainEntity = CreateAndSaveEntity(mainEntity);
            }

            return mainEntity;
        }

        protected CodeEntity CreateAndSaveEntity(CodeEntity superCodeEntity)
        {
            var result = new CodeEntity();
            result.Kind = KindOfCodeEntity.Instance;

            var newName = _context.CommonNamesStorage.SelfName;
            result.Name = newName;

#if DEBUG
            //Log($"newName = {newName}");
#endif

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
        public virtual void AppendProcessInfo(IProcessInfo processInfo)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual void AppendAndTryStartProcessInfo(IProcessInfo processInfo)
        {
            throw new NotImplementedException();
        }

#if DEBUG
        /// <inheritdoc/>
        public virtual void PrintProcessesList()
        {
            throw new NotImplementedException();
        }
#endif
    }
}

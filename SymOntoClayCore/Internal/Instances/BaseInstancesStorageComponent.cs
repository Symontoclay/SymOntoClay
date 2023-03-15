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
            _commonNamesStorage = context.CommonNamesStorage;
        }

        private readonly IMainStorageContext _context;
        protected readonly ICommonNamesStorage _commonNamesStorage;

        public virtual void LoadFromSourceFiles()
        {
        }

        /// <inheritdoc/>
        public virtual void ActivateMainEntity()
        {
            GetOrCreateMainEntity();
        }

        protected CodeItem GetOrCreateMainEntity()
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
                mainEntity = CreateAndSaveInstanceCodeItem(mainEntity);
            }

            return mainEntity;
        }

        protected CodeItem CreateAndSaveInstanceCodeItem(CodeItem superCodeEntity)
        {
            return CreateAndSaveInstanceCodeItem(superCodeEntity, _commonNamesStorage.SelfName);
        }

        protected CodeItem CreateAndSaveInstanceCodeItem(CodeItem superCodeEntity, StrongIdentifierValue name)
        {
            var result = new InstanceCodeItem();

            result.Name = name;

#if DEBUG
            //Log($"result.Name = {result.Name}");
#endif

            FillUpAndRegisterInstance(result, superCodeEntity);

#if DEBUG
            //Log($"result = {result}");
#endif

            return result;
        }

        protected void FillUpAndRegisterInstance(CodeItem instance, CodeItem superCodeItem)
        {
            if(instance.Holder == null)
            {
                instance.Holder = _commonNamesStorage.DefaultHolder;
            }            

            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = false
            };

            inheritanceItem.SubName = instance.Name;
            inheritanceItem.SuperName = superCodeItem.Name;
            inheritanceItem.Rank = new LogicalValue(1.0F);

            instance.InheritanceItems.Add(inheritanceItem);

#if DEBUG
            //Log($"instance = {instance}");
#endif

            var globalStorage = _context.Storage.GlobalStorage;

            globalStorage.MetadataStorage.Append(instance);
            globalStorage.InheritanceStorage.SetInheritance(inheritanceItem);
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

        /// <inheritdoc/>
        public virtual event Action OnIdle;

        /// <inheritdoc/>
        public virtual int GetCountOfCurrentProcesses()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual void ActivateState(StateDef state)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual void TryActivateDefaultState()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual AppInstance MainEntity => null;

        /// <inheritdoc/>
        public virtual Value CreateInstance(StrongIdentifierValue prototypeName, LocalCodeExecutionContext executionContext)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Value CreateInstance(InstanceValue instanceValue, LocalCodeExecutionContext executionContext)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Value CreateInstance(CodeItem codeItem, LocalCodeExecutionContext executionContext)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Value CreateInstance(ActionPtr actionPtr, LocalCodeExecutionContext executionContext)
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

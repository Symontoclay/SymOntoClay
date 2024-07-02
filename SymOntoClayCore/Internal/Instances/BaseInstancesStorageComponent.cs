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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
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
        public virtual void ActivateMainEntity(IMonitorLogger logger)
        {
            GetOrCreateMainEntity(logger);
        }

        protected CodeItem GetOrCreateMainEntity(IMonitorLogger logger)
        {
            var globalStorage = _context.Storage.GlobalStorage;

            var mainEntity = globalStorage.MetadataStorage.MainCodeEntity;

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
                mainEntity = CreateAndSaveInstanceCodeItem(logger, mainEntity);
            }

            return mainEntity;
        }

        protected CodeItem CreateAndSaveInstanceCodeItem(IMonitorLogger logger, CodeItem superCodeEntity)
        {
            return CreateAndSaveInstanceCodeItem(logger, superCodeEntity, _commonNamesStorage.SelfName);
        }

        protected CodeItem CreateAndSaveInstanceCodeItem(IMonitorLogger logger, CodeItem superCodeEntity, StrongIdentifierValue name)
        {
            var result = new InstanceCodeItem();

            result.Name = name;

            FillUpAndRegisterInstance(logger, result, superCodeEntity);

            return result;
        }

        protected void FillUpAndRegisterInstance(IMonitorLogger logger, CodeItem instance, CodeItem superCodeItem)
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

            var globalStorage = _context.Storage.GlobalStorage;

            globalStorage.MetadataStorage.Append(logger, instance);
            globalStorage.InheritanceStorage.SetInheritance(logger, inheritanceItem);
        }

        /// <inheritdoc/>
        public virtual void AppendProcessInfo(IMonitorLogger logger, IProcessInfo processInfo)
        {
            throw new NotImplementedException("B3C9AAAF-122E-4CEA-BCF3-6503DACF962B");
        }

        /// <inheritdoc/>
        public virtual void AppendAndTryStartProcessInfo(IMonitorLogger logger, string callMethodId, IProcessInfo processInfo)
        {
            throw new NotImplementedException("7D9D2B9B-80ED-414E-9750-FEC5153FDAE9");
        }

        /// <inheritdoc/>
        public virtual event Action OnIdle;

        /// <inheritdoc/>
        public virtual int GetCountOfCurrentProcesses(IMonitorLogger logger)
        {
            throw new NotImplementedException("74323836-3AD0-444D-9036-ED6682D58E0A");
        }

        /// <inheritdoc/>
        public virtual void ActivateState(IMonitorLogger logger, StateDef state)
        {
            throw new NotImplementedException("D0276616-3AEE-43E0-A332-BB5C131D11B2");
        }

        /// <inheritdoc/>
        public virtual void TryActivateDefaultState(IMonitorLogger logger)
        {
            throw new NotImplementedException("B0A5AA69-E0D8-4044-9659-B58F289EADE6");
        }

        /// <inheritdoc/>
        public virtual AppInstance MainEntity => null;

        /// <inheritdoc/>
        public virtual Value CreateInstance(IMonitorLogger logger, StrongIdentifierValue prototypeName, ILocalCodeExecutionContext executionContext)
        {
            throw new NotImplementedException("DD08A683-8D27-45F2-A836-73A55D8E83C6");
        }

        /// <inheritdoc/>
        public virtual Value CreateInstance(IMonitorLogger logger, InstanceValue instanceValue, ILocalCodeExecutionContext executionContext)
        {
            throw new NotImplementedException("7747129D-B05A-4CC3-9F96-3FB73B485239");
        }

        /// <inheritdoc/>
        public virtual Value CreateInstance(IMonitorLogger logger, CodeItem codeItem, ILocalCodeExecutionContext executionContextx)
        {
            throw new NotImplementedException("EB8DFE5E-0F92-468D-90D8-71A4CF792D55");
        }

        /// <inheritdoc/>
        public virtual Value CreateInstance(IMonitorLogger logger, ActionPtr actionPtr, ILocalCodeExecutionContext executionContext, IExecutionCoordinator executionCoordinator)
        {
            throw new NotImplementedException("73BBF03C-DFEE-440D-8BC4-7CF887A9BA9B");
        }

#if DEBUG
        /// <inheritdoc/>
        public virtual void PrintProcessesList(IMonitorLogger logger)
        {
            throw new NotImplementedException("EF288776-8AA2-4706-BA80-A884824DA8B2");
        }
#endif
    }
}

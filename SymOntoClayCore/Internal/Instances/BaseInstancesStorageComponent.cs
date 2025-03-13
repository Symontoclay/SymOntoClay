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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class BaseInstancesStorageComponent : BaseContextComponent, IInstancesStorageComponent
    {
        public BaseInstancesStorageComponent(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            _commonNamesStorage = _context.CommonNamesStorage;
        }

        private readonly IMainStorageContext _context;
        protected ICommonNamesStorage _commonNamesStorage;

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
                mainEntity = CreateAndSaveAppInstanceCodeItem(logger, mainEntity);
            }

            return mainEntity;
        }

        protected CodeItem CreateAndSaveAppInstanceCodeItem(IMonitorLogger logger, CodeItem superCodeEntity)
        {
            return CreateAndSaveAppInstanceCodeItem(logger, superCodeEntity, _commonNamesStorage.SelfName);
        }

        protected CodeItem CreateAndSaveAppInstanceCodeItem(IMonitorLogger logger, CodeItem superCodeEntity, StrongIdentifierValue name)
        {
#if DEBUG
            //Info("0885966D-3FAE-4C37-BBD1-567D592D47B4", $"superCodeEntity.GetType().Name = {superCodeEntity.GetType().Name}");
#endif

            var result = new AppInstanceCodeItem();

            if (superCodeEntity.IsApp)
            {
#if DEBUG
                //Info("26BA035B-6FC0-4175-8613-F221AE88682B", $"superCodeEntity.AsApp.RootTasks = {superCodeEntity.AsApp.RootTasks.WriteListToString()}");
#endif
            
                result.RootTasks = superCodeEntity.AsApp.RootTasks?.Select(p => p.Clone())?.ToList() ?? new List<StrongIdentifierValue>();
            }

            FillUpAndRegisterInstance(logger, result, superCodeEntity, name);

            return result;
        }

        protected CodeItem CreateAndSaveInstanceCodeItem(IMonitorLogger logger, CodeItem superCodeEntity, StrongIdentifierValue name)
        {
            var result = new InstanceCodeItem();

            FillUpAndRegisterInstance(logger, result, superCodeEntity, name);

            return result;
        }

        protected void FillUpAndRegisterInstance(IMonitorLogger logger, CodeItem instance, CodeItem superCodeItem, StrongIdentifierValue name)
        {
            instance.Name = name;

            if (instance.Holder == null)
            {
                instance.Holder = _commonNamesStorage.DefaultHolderType;
            }            

            var inheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = false
            };

            inheritanceItem.SubType = instance.TypeInfo;
            inheritanceItem.SuperType = superCodeItem.TypeInfo;
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

        public void AddOnIdleHandler(IOnIdleInstancesStorageComponentHandler handler)
        {
            lock (_onIdleHandlersLockObj)
            {
                if (_onIdleHandlers.Contains(handler))
                {
                    return;
                }

                _onIdleHandlers.Add(handler);
            }
        }

        /// <inheritdoc/>
        public void RemoveOnIdleHandler(IOnIdleInstancesStorageComponentHandler handler)
        {
            lock (_onIdleHandlersLockObj)
            {
                if (_onIdleHandlers.Contains(handler))
                {
                    _onIdleHandlers.Remove(handler);
                }
            }
        }

        protected void EmitOnIdleHandlers()
        {
            DispatchOnIdle();

            lock (_onIdleHandlersLockObj)
            {
                foreach (var handler in _onIdleHandlers)
                {
                    handler.Invoke();
                }
            }
        }

        private object _onIdleHandlersLockObj = new object();
        private List<IOnIdleInstancesStorageComponentHandler> _onIdleHandlers = new List<IOnIdleInstancesStorageComponentHandler>();

        protected virtual void DispatchOnIdle()
        {
        }

        /// <inheritdoc/>
        public virtual void CheckCountOfActiveProcesses(IMonitorLogger logger)
        {
            throw new NotImplementedException("54CEF05C-EB52-4C53-A898-E864785DAEB1");
        }

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

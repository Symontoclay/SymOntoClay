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
                IsSystemDefined = false
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

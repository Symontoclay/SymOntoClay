﻿using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.InheritanceStorage
{
    public class InheritanceStorage: BaseLoggedComponent, IInheritanceStorage
    {
        public InheritanceStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly object _lockObj = new object();

        private readonly KindOfStorage _kind;

        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        public void SetInheritance(IName subItem, InheritanceItem inheritanceItem)
        {
            SetInheritance(subItem, inheritanceItem, true);
        }

        public void SetInheritance(IName subItem, InheritanceItem inheritanceItem, bool isPrimary)
        {
#if DEBUG
            Log($"subItem = {subItem}");
            Log($"inheritanceItem = {inheritanceItem}");
            Log($"isPrimary = {isPrimary}");
#endif


#if IMAGINE_WORKING
            Log("Do");
#else
                throw new NotImplementedException();
#endif
        }
    }
}

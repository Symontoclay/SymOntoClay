using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public abstract class BaseContextComponent: BaseComponent, IBaseContextComponent
    {
        protected BaseContextComponent(IMonitorLogger logger)
            : base(logger)
        {
        }

        void IBaseContextComponent.LinkWithOtherBaseContextComponents()
        {
            LinkWithOtherBaseContextComponents();
        }

        protected virtual void LinkWithOtherBaseContextComponents()
        {
        }

        void IBaseContextComponent.Init()
        {
            Init();
        }

        protected virtual void Init()
        {
        }
    }
}

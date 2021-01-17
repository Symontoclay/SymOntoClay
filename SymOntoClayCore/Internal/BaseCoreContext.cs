using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public class BaseCoreContext: BaseComponent, IBaseCoreContext
    {
        public BaseCoreContext(IEntityLogger logger)
            : base(logger)
        {
        }

        public IEntityDictionary Dictionary { get; set; }
    }
}

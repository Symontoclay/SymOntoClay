using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.PlatformSupport
{
    public class PlatformSupportComponent: BaseComponent, IPlatformSupport
    {
        public PlatformSupportComponent(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;
    }
}

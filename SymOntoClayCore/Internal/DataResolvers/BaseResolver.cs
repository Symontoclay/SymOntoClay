using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public abstract class BaseResolver: BaseLoggedComponent
    {
        protected BaseResolver(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        protected readonly IEngineContext _context;
    }
}

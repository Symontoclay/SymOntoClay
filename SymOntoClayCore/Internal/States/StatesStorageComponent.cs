using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.States
{
    public class StatesStorageComponent : BaseComponent
    {
        public StatesStorageComponent(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;

        public void LoadFromSourceCode()
        {
#if IMAGINE_WORKING
            Log("Do");
#else
                throw new NotImplementedException();
#endif
        }
    }
}

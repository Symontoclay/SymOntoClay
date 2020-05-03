using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public class ImagesRegistry
    {
        private readonly IWorlCoreContext _coreContext;

        public ImagesRegistry(IWorlCoreContext coreContext)
        {
            _coreContext = coreContext;
        }
    }
}

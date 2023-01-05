using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface IServicesFactory
    {
        IEntityConstraintsService GetEntityConstraintsService();
        ICodeFrameService GetCodeFrameService();
    }
}

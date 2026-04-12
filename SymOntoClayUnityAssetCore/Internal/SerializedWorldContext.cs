using System;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public class SerializedWorldContext
    {
        public SerializedWorldContext(IWorldCoreContext coreContext)
        {
            _coreContext = coreContext;
        }

        private IWorldCoreContext _coreContext;

        public void LoadFromSourceCode()
        {
            //throw new NotImplementedException("C0D7FA6C-3CD2-496F-BF2F-A79F7F12B074");
        }
    }
}

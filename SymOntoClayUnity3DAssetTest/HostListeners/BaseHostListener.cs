using SymOntoClay.Core.Tests.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.HostListeners
{
    public abstract class BaseHostListener: ILoggedTestHostListener
    {
        public void SetLogger(IEntityLogger logger)
        {
            _logger = logger;
        }

        protected IEntityLogger _logger;

        private static object _lockObj = new object();

        private static int _methodId;

        protected int GetMethodId()
        {
            lock (_lockObj)
            {
                _methodId++;
                return _methodId;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Numerics;
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

        /// <inheritdoc/>
        public Vector3 ConvertFromRelativeToAbsolute(Vector2 relativeCoordinates)
        {
#if IMAGINE_WORKING
            Log("Do !!!!IMAGINE_WORKING!!!! FIX Me!!!!!!!!!!!!!!!!");
            return new Vector3(666, 999, 0);
#else
                throw new NotImplementedException();
#endif
        }
    }
}

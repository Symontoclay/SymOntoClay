using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.BipedNPC
{
    public class BipedNPCGameComponent: BaseGameComponent
    {
        public BipedNPCGameComponent(BipedNPCSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings.Id, worldContext)
        {
            Log("Ya!!!");
        }

        /// <inheritdoc/>
        public override void BeginStarting()
        {
#if IMAGINE_WORKING
            Log("BeginStarting");
#else
            throw new NotImplementedException();
#endif
        }

        /// <inheritdoc/>
        public override bool IsReadyForActivating
        {
            get
            {
#if IMAGINE_WORKING
                Log("IsReadyForActivating");
                return true;
#else
            throw new NotImplementedException();
#endif
            }
        }
    }
}

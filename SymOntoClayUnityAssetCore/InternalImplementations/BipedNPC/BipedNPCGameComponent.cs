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
        public override void LoadFromSourceCode()
        {
#if IMAGINE_WORKING
            Log("Do");
#else
            throw new NotImplementedException();
#endif
        }

        /// <inheritdoc/>
        public override void BeginStarting()
        {
#if IMAGINE_WORKING
            Log("Do");
#else
            throw new NotImplementedException();
#endif
        }

        /// <inheritdoc/>
        public override bool IsWaited
        {
            get
            {
#if IMAGINE_WORKING
                Log("Do");
                return true;
#else
            throw new NotImplementedException();
#endif
            }
        }
    }
}

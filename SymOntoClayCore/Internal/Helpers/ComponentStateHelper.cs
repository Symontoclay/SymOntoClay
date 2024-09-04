using System;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class ComponentStateHelper
    {
        public static bool IsLoaded(ComponentState state)
        {
            switch(state)
            {
                case ComponentState.Created:
                    return false;

                case ComponentState.Loaded:
                case ComponentState.Started:
                case ComponentState.Stopped:
                case ComponentState.Died:
                case ComponentState.Disposed:
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public static bool IsStopped(ComponentState state)
        {
            switch (state)
            {
                case ComponentState.Created:
                case ComponentState.Loaded:
                case ComponentState.Started:
                    return false;


                case ComponentState.Stopped:
                case ComponentState.Died:
                case ComponentState.Disposed:
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}

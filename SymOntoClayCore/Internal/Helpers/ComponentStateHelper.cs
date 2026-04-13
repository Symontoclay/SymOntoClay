using System;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class ComponentStateHelper
    {
        public static bool IsLoaded(ComponentState state)
        {
            switch (state)
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
                    throw new ArgumentOutOfRangeException(nameof(state), state, "09FC518C-A9DF-4D22-A851-6CF7F80E94E1");
            }
        }

        public static bool IsStopped(ComponentState state)
        {
            switch (state)
            {
                case ComponentState.Started:
                    return false;

                default:
                    return true;
            }
        }
    }
}

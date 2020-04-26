using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public class WorldCore
    {
        private static readonly WorldCore __instance = new WorldCore();

        /// <summary>
        /// Gets instance of the class.
        /// </summary>
        public static WorldCore Instance => __instance;

        private WorldCore()
        {
        }

        /// <summary>
        /// Sets settings into the instance.
        /// </summary>
        /// <param name="settings">Set settings value.</param>
        public void SetSettings(Settings settings)
        {
            throw new NotImplementedException();
        }
    }
}

using SymOntoClay.UnityAsset.Core.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public static class WorldFactory
    {
        private static readonly IWorld __instance = new WorldCore();

        /// <summary>
        /// Gets instance of the class.
        /// </summary>
        public static IWorld WorldInstance => __instance;
    }
}

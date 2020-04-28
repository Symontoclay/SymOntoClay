using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.InternalImplementations.NPC;
using SymOntoClay.UnityAsset.Core.InternalImplementations.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public class WorldCore: IWorldComponentDisposable
    {
        private static readonly WorldCore __instance = new WorldCore();

        /// <summary>
        /// Gets instance of the class.
        /// </summary>
        public static WorldCore Instance => __instance;

        private WorldCore()
        {
            _context = new WorldContext();
        }

        private readonly WorldContext _context;

        /// <summary>
        /// Sets settings into the instance.
        /// </summary>
        /// <param name="settings">Set settings value.</param>
        public void SetSettings(Settings settings)
        {
            //throw new NotImplementedException();
        }

        public bool EnableLogging { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool EnableRemoteConnection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public INPC GetNPC(NPCSettings settings)
        {
            return new NPCImplementation(settings, _context);
        }

        public IPlayer GetPlayer(PlayerSettings settings)
        {
            return new PlayerImlementation(settings, _context);
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public bool IsActive { get => throw new NotImplementedException(); }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool IsDisposed { get => throw new NotImplementedException(); }
    }
}

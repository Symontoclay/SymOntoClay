using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.Place
{
    public class PlaceGameComponent : BaseStoredGameComponent
    {
        public PlaceGameComponent(PlaceSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings, worldContext)
        {
            _allowPublicPosition = settings.AllowPublicPosition;
            _useStaticPosition = settings.UseStaticPosition;

            _platformSupport = settings.PlatformSupport;
        }

        private readonly IPlatformSupport _platformSupport;
        private readonly bool _allowPublicPosition;
        private readonly Vector3? _useStaticPosition;

        public string InsertPublicFact(string text)
        {
            return HostStorage.InsertPublicFact(text);
        }

        public string InsertPublicFact(RuleInstance fact)
        {
            return HostStorage.InsertPublicFact(fact);
        }

        public void RemovePublicFact(string id)
        {
            HostStorage.RemovePublicFact(id);
        }

        /// <inheritdoc/>
        public override bool IsWaited => true;

        /// <inheritdoc/>
        public override bool CanBeTakenBy(IEntity subject)
        {
            return false;
        }

        /// <inheritdoc/>
        public override Vector3? GetPosition()
        {
            if (_allowPublicPosition)
            {
                if (_platformSupport == null)
                {
                    return _useStaticPosition;
                }

                return _platformSupport.GetCurrentAbsolutePosition();
            }

            return null;
        }
    }
}

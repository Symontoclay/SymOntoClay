using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.Vision;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.ConditionalEntityHostSupport
{
    public class ConditionalEntityHostSupportComponent: BaseComponent, IConditionalEntityHostSupport, IEntity
    {
        public ConditionalEntityHostSupportComponent(IEntityLogger logger, HumanoidNPCSettings settings, VisionComponent visionComponent, IHostSupport hostSupport, IWorldCoreGameComponentContext worldContext)
            : base(logger)
        {
            _worldContext = worldContext;
            _visionComponent = visionComponent;
            _hostSupport = hostSupport;

            _instanceId = settings.InstanceId;
            _id = settings.Id;
            _idForFacts = settings.IdForFacts;
        }

        private readonly IWorldCoreGameComponentContext _worldContext;
        private readonly VisionComponent _visionComponent;
        private readonly IHostSupport _hostSupport;

        private readonly int _instanceId;
        private readonly string _id;
        private readonly string _idForFacts;

        /// <inheritdoc/>
        public int InstanceId => _instanceId;

        /// <inheritdoc/>
        public string Id => _id;

        /// <inheritdoc/>
        public string IdForFacts => _idForFacts;

        /// <inheritdoc/>
        public Vector3? Position => _hostSupport.GetCurrentAbsolutePosition();

        /// <inheritdoc/>
        public bool IsEmpty => false;

        /// <inheritdoc/>
        public int GetInstanceId(StrongIdentifierValue id)
        {
            return _worldContext.GetInstanceIdByIdForFacts(id.NormalizedNameValue);
        }

        /// <inheritdoc/>
        public bool IsVisible(int instanceId)
        {
            if(_visionComponent == null)
            {
                return false;
            }

            return _visionComponent.IsVisible(instanceId);
        }

        /// <inheritdoc/>
        public bool CanBeTaken(int instanceId)
        {
            return _worldContext.CanBeTakenBy(instanceId, this);
        }

        /// <inheritdoc/>
        public Vector3? GetPosition(int instanceId)
        {
            if (_visionComponent != null)
            {
                var visionPosition = _visionComponent.GetPosition(instanceId);

                if(visionPosition.HasValue)
                {
                    return visionPosition;
                }
            }

            return _worldContext.GetPosition(instanceId);
        }

        /// <inheritdoc/>
        public (float?, Vector3?) DistanceToAndPosition(int instanceId)
        {
            var position = GetPosition(instanceId);

            if (position.HasValue)
            {
                return (Vector3.Distance(_hostSupport.GetCurrentAbsolutePosition(), position.Value), position);
            }

            return (null, null);
        }

        /// <inheritdoc/>
        public float? DistanceTo(int instanceId)
        {
            return DistanceToAndPosition(instanceId).Item1;
        }

        /// <inheritdoc/>
        public void Specify(params EntityConstraints[] constraints)
        {
        }

        /// <inheritdoc/>
        public void Resolve()
        {
        }

        /// <inheritdoc/>
        public void ResolveIfNeeds()
        {
        }
    }
}

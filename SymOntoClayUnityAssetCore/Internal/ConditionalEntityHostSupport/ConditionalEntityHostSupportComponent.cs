/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal.Vision;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.ConditionalEntityHostSupport
{
    public class ConditionalEntityHostSupportComponent : BaseComponent, IConditionalEntityHostSupport, IEntity
    {
        public ConditionalEntityHostSupportComponent(IMonitorLogger logger, HumanoidNPCSettings settings, VisionComponent visionComponent, IHostSupport hostSupport, IWorldCoreGameComponentContext worldContext)
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
        public Vector3? Position => _hostSupport.GetCurrentAbsolutePosition(Logger);

        /// <inheritdoc/>
        public bool IsEmpty => false;

        /// <inheritdoc/>
        public int GetInstanceId(IMonitorLogger logger, StrongIdentifierValue id)
        {
            return _worldContext.GetInstanceIdByIdForFacts(id.NormalizedNameValue);
        }

        /// <inheritdoc/>
        public bool IsVisible(IMonitorLogger logger, int instanceId)
        {
            if (_visionComponent == null)
            {
                return false;
            }

            return _visionComponent.IsVisible(instanceId);
        }

        /// <inheritdoc/>
        public bool CanBeTaken(IMonitorLogger logger, int instanceId)
        {
            return _worldContext.CanBeTakenBy(instanceId, this);
        }

        /// <inheritdoc/>
        public Vector3? GetPosition(IMonitorLogger logger, int instanceId)
        {
            if (_visionComponent != null)
            {
                var visionPosition = _visionComponent.GetPosition(instanceId);

                if (visionPosition.HasValue)
                {
                    return visionPosition;
                }
            }

            return _worldContext.GetPosition(instanceId);
        }

        /// <inheritdoc/>
        public (float?, Vector3?) DistanceToAndPosition(IMonitorLogger logger, int instanceId)
        {
            var position = GetPosition(logger, instanceId);

            if (position.HasValue)
            {
                return (Vector3.Distance(_hostSupport.GetCurrentAbsolutePosition(logger), position.Value), position);
            }

            return (null, null);
        }

        /// <inheritdoc/>
        public float? DistanceTo(IMonitorLogger logger, int instanceId)
        {
            return DistanceToAndPosition(logger, instanceId).Item1;
        }

        /// <inheritdoc/>
        public bool IsOnceResolved => throw new NotImplementedException("D997A50F-3F55-4239-8B17-F06B7B355501");

        /// <inheritdoc/>
        public void Specify(IMonitorLogger logger, params EntityConstraints[] constraints)
        {
        }

        /// <inheritdoc/>
        public void SpecifyOnce(IMonitorLogger logger, params EntityConstraints[] constraints)
        {
        }

        /// <inheritdoc/>
        public void SpecifyOnce(IMonitorLogger logger, IStorage storage)
        {
        }

        /// <inheritdoc/>
        public void Resolve(IMonitorLogger logger)
        {
        }

        /// <inheritdoc/>
        public void ResolveIfNeeds(IMonitorLogger logger)
        {
        }

        /// <inheritdoc/>
        public IEntity GetNewEntity(IMonitorLogger logger, string id)
        {
            return null;
        }
    }
}

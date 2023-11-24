/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.CodeModel.MonitorSerializableObjects;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class BaseEntityValue: LoggedValue, IEntity, INavTarget
    {
        protected BaseEntityValue(IEngineContext context, ILocalCodeExecutionContext localContext)
            : base(context.Logger)
        {
            _context = context;
            _localContext = localContext;
            _conditionalEntityHostSupport = context.ConditionalEntityHostSupport;
        }

        private IEngineContext _context;
        private ILocalCodeExecutionContext _localContext;
        private IConditionalEntityHostSupport _conditionalEntityHostSupport;
        private readonly Random _random = new Random();

        protected virtual void CheckForUpdates(IMonitorLogger logger)
        {
        }

        /// <inheritdoc/>
        public int InstanceId
        {
            get
            {
                CheckForUpdates(Logger);

                return _instanceId;
            }
        }

        /// <inheritdoc/>
        public string Id
        {
            get
            {
                CheckForUpdates(Logger);

                return _id;
            }
        }

        /// <inheritdoc/>
        public string IdForFacts
        {
            get
            {
                CheckForUpdates(Logger);

                return _idForFacts;
            }
        }

        /// <inheritdoc/>
        public Vector3? Position
        {
            get
            {
                CheckForUpdates(Logger);

                return _position;
            }
        }

        /// <inheritdoc/>
        public bool IsEmpty
        {
            get
            {
                return _isEmpty;
            }
        }

        /// <inheritdoc/>
        public void Specify(IMonitorLogger logger, params EntityConstraints[] constraints)
        {
            _constraints = constraints;
            _onceStorage = null;
            _specifiedOnce = false;
            _needUpdate = true;
        }

        /// <inheritdoc/>
        public void SpecifyOnce(IMonitorLogger logger, params EntityConstraints[] constraints)
        {
            _constraints = constraints;
            _onceStorage = null;
            _specifiedOnce = true;
            _needUpdate = true;
        }

        /// <inheritdoc/>
        public void SpecifyOnce(IMonitorLogger logger, IStorage storage)
        {
            _constraints = null;
            _specifiedOnce = true;
            _onceStorage = storage;
            _needUpdate = true;
        }

        /// <inheritdoc/>
        public void ResolveIfNeeds(IMonitorLogger logger)
        {
            CheckForUpdates(logger);
        }

        /// <inheritdoc/>
        public virtual void Resolve(IMonitorLogger logger)
        {
        }

        public virtual IEntity GetNewEntity(IMonitorLogger logger, string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                throw new NotImplementedException();
            }

            var identifier = NameHelper.CreateName(id);

            if (identifier.KindOfName != KindOfName.Entity)
            {
                throw new NotImplementedException();
            }

            var entityValue = new EntityValue(identifier, _context, _localContext);

            entityValue.Resolve(logger);

            return entityValue;
        }

        protected bool _needUpdate = true;
        protected bool _specifiedOnce;
        protected IStorage _onceStorage;
        protected int _instanceId;
        protected string _id;
        protected StrongIdentifierValue _entityId;
        protected string _idForFacts;
        protected Vector3? _position;
        protected bool _isEmpty = true;
        protected EntityConstraints[] _constraints;

        /// <inheritdoc/>
        KindOfNavTarget INavTarget.Kind => KindOfNavTarget.ByEntity;

        /// <inheritdoc/>
        float INavTarget.Distance => 0f;

        /// <inheritdoc/>
        float INavTarget.HorizontalAngle => 0f;

        /// <inheritdoc/>
        Vector3 INavTarget.AbcoluteCoordinates => Vector3.Zero;

        /// <inheritdoc/>
        IEntity INavTarget.Entity => this;

        public StrongIdentifierValue EntityId => _entityId;

        public StrongIdentifierValue ResolveAndGetEntityId(IMonitorLogger logger)
        {
            if(_entityId == null || _entityId.IsEmpty)
            {
                Resolve(logger);
            }
            else
            {
                ResolveIfNeeds(logger);
            }
            
            return _entityId;
        }

        private struct FilteredItem
        {
            public StrongIdentifierValue Id { get; set; }
            public int InstanceId { get; set; }
            public Vector3? Position { get; set; }
        }

        protected void ProcessIdsList(IMonitorLogger logger, List<StrongIdentifierValue> idsList)
        {
            if (!idsList.Any())
            {
                ResetCurrEntity(logger);
                return;
            }

            if (_constraints.IsNullOrEmpty())
            {
                SetCurrEntity(logger, idsList.First());
                return;
            }

            var filteredItemsList = new List<FilteredItem>();

            float? minDistance = null;

            foreach (var foundId in idsList)
            {
#if DEBUG
                //Log($"foundId = {foundId}");
#endif

                var instanceId = _conditionalEntityHostSupport.GetInstanceId(logger, foundId);

#if DEBUG
                //Log($"instanceId = {instanceId}");
#endif

                if (instanceId == 0)
                {
                    continue;
                }

                var isFit = true;

                float? currentMinDistance = null;
                Vector3? currentPosition = null;
                var clearFilteredItemsList = false;

                foreach (var constraint in _constraints)
                {
                    if (!isFit)
                    {
                        break;
                    }

#if DEBUG
                    //Log($"constraint = {constraint}");
#endif

                    switch (constraint)
                    {
                        case EntityConstraints.CanBeTaken:
                            if (!_conditionalEntityHostSupport.CanBeTaken(logger, instanceId))
                            {
                                isFit = false;
                            }
                            break;

                        case EntityConstraints.OnlyVisible:
                            if (!_conditionalEntityHostSupport.IsVisible(logger, instanceId))
                            {
                                isFit = false;
                            }
                            break;

                        case EntityConstraints.OnlyInvisible:
                            if (_conditionalEntityHostSupport.IsVisible(logger, instanceId))
                            {
                                isFit = false;
                            }
                            break;

                        case EntityConstraints.Nearest:
                            {
                                var distanceToAndPosition = _conditionalEntityHostSupport.DistanceToAndPosition(logger, instanceId);

                                var distance = distanceToAndPosition.Item1;

                                if (distance.HasValue)
                                {
                                    currentPosition = distanceToAndPosition.Item2;

                                    if (minDistance.HasValue)
                                    {
                                        if (minDistance == distance)
                                        {
                                            currentMinDistance = distance;
                                        }
                                        else
                                        {
                                            if (minDistance < distance)
                                            {
                                                currentMinDistance = distance;

                                                clearFilteredItemsList = true;
                                            }
                                            else
                                            {
                                                isFit = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        currentMinDistance = distance;
                                    }

                                    break;
                                }

                                isFit = false;
                            }
                            break;

                        case EntityConstraints.Random:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null);
                    }
                }

                if (!isFit)
                {
                    continue;
                }

                if (clearFilteredItemsList)
                {
                    filteredItemsList.Clear();
                }

                if (currentMinDistance.HasValue && currentMinDistance < minDistance)
                {
                    minDistance = currentMinDistance;
                }

#if DEBUG
                //Log($"currentPosition = {currentPosition}");
#endif

                if(!currentPosition.HasValue)
                {
                    var distanceToAndPosition = _conditionalEntityHostSupport.DistanceToAndPosition(logger, instanceId);
                    currentPosition = distanceToAndPosition.Item2;
                }

#if DEBUG
                //Log($"currentPosition (after) = {currentPosition}");
#endif

                filteredItemsList.Add(new FilteredItem()
                {
                    Id = foundId,
                    InstanceId = instanceId,
                    Position = currentPosition
                });
            }

            if (!filteredItemsList.Any())
            {
                ResetCurrEntity(logger);
                return;
            }

            FilteredItem targetFilteredItem = default;

            if(_constraints.Any(p => p == EntityConstraints.Random))
            {
                var index = _random.Next(filteredItemsList.Count);

                targetFilteredItem = filteredItemsList[index];

#if DEBUG
                //Log($"targetFilteredItem.Id = {targetFilteredItem.Id}");
#endif
            }
            else
            {
                targetFilteredItem = filteredItemsList.First();
            }

            SetCurrEntity(logger, targetFilteredItem.Id, targetFilteredItem.InstanceId, targetFilteredItem.Position);
        }

        protected void SetCurrEntity(IMonitorLogger logger, StrongIdentifierValue id)
        {
            var instanceId = _conditionalEntityHostSupport.GetInstanceId(logger, id);

            if (instanceId == 0)
            {
                ResetCurrEntity(logger);
                return;
            }

            SetCurrEntity(logger, id, instanceId, null);
        }

        private void SetCurrEntity(IMonitorLogger logger, StrongIdentifierValue id, int instanceId, Vector3? position)
        {
            if (!position.HasValue)
            {
                position = _conditionalEntityHostSupport.GetPosition(logger, instanceId);
            }

            _entityId = id;
            _id = id.NameValue;
            _idForFacts = _id;
            _instanceId = instanceId;
            _position = position;
            _isEmpty = false;
        }

        protected void ResetCurrEntity(IMonitorLogger logger)
        {
            _id = string.Empty;
            _entityId = StrongIdentifierValue.Empty;
            _idForFacts = string.Empty;
            _instanceId = 0;
            _position = null;
            _isEmpty = true;
        }

        protected void FillUpBaseEntityValueMonitorSerializableObject(BaseEntityValueMonitorSerializableObject obj, IMonitorLogger logger)
        {
            obj.EntityId = _entityId?.ToHumanizedString();
            obj.Id = _id;
            obj.IdForFacts = _idForFacts;
            obj.InstanceId = _instanceId;
            obj.Position = _position;
            obj.IsEmpty = _isEmpty;
            obj.Constraints = _constraints?.Select(p => (int)p).ToArray();
            obj.SpecifiedOnce = _specifiedOnce;
            obj.OnceStorage = _onceStorage?.ToBriefString();
        }
    }
}

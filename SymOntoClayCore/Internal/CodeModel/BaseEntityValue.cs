/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using SymOntoClay.CoreHelper.CollectionsHelpers;
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
        protected BaseEntityValue(IEngineContext context, LocalCodeExecutionContext localContext)
            : base(context.Logger)
        {
            _context = context;
            _localContext = localContext;
            _conditionalEntityHostSupport = context.ConditionalEntityHostSupport;
        }

        private IEngineContext _context;
        private LocalCodeExecutionContext _localContext;
        private IConditionalEntityHostSupport _conditionalEntityHostSupport;
        private readonly Random _random = new Random();

        protected virtual void CheckForUpdates()
        {
        }

        /// <inheritdoc/>
        public int InstanceId
        {
            get
            {
                CheckForUpdates();

                return _instanceId;
            }
        }

        /// <inheritdoc/>
        public string Id
        {
            get
            {
                CheckForUpdates();

                return _id;
            }
        }

        /// <inheritdoc/>
        public string IdForFacts
        {
            get
            {
                CheckForUpdates();

                return _idForFacts;
            }
        }

        /// <inheritdoc/>
        public Vector3? Position
        {
            get
            {
                CheckForUpdates();

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
        public void Specify(params EntityConstraints[] constraints)
        {
            _constraints = constraints;
            _onceStorage = null;
            _specifiedOnce = false;
            _needUpdate = true;
        }

        /// <inheritdoc/>
        public void SpecifyOnce(params EntityConstraints[] constraints)
        {
            _constraints = constraints;
            _onceStorage = null;
            _specifiedOnce = true;
            _needUpdate = true;
        }

        /// <inheritdoc/>
        public void SpecifyOnce(IStorage storage)
        {
            _constraints = null;
            _specifiedOnce = true;
            _onceStorage = storage;
            _needUpdate = true;
        }

        /// <inheritdoc/>
        public void ResolveIfNeeds()
        {
            CheckForUpdates();
        }

        /// <inheritdoc/>
        public virtual void Resolve()
        {
        }

        public virtual IEntity GetNewEntity(string id)
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

            entityValue.Resolve();

            return entityValue;
        }

        protected bool _needUpdate = true;
        protected bool _specifiedOnce;
        protected IStorage _onceStorage;
        protected int _instanceId;
        protected string _id;
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

        private struct FilteredItem
        {
            public StrongIdentifierValue Id { get; set; }
            public int InstanceId { get; set; }
            public Vector3? Position { get; set; }
        }

        protected void ProcessIdsList(List<StrongIdentifierValue> idsList)
        {
#if DEBUG
            //Log($"idsList = {JsonConvert.SerializeObject(idsList?.Select(p => p.NameValue))}");
#endif

            if (!idsList.Any())
            {
                ResetCurrEntity();
                return;
            }

            if (_constraints.IsNullOrEmpty())
            {
                SetCurrEntity(idsList.First());
                return;
            }

            var filteredItemsList = new List<FilteredItem>();

            float? minDistance = null;

            foreach (var foundId in idsList)
            {
#if DEBUG
                //Log($"foundId = {foundId}");
#endif

                var instanceId = _conditionalEntityHostSupport.GetInstanceId(foundId);

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
                            if (!_conditionalEntityHostSupport.CanBeTaken(instanceId))
                            {
                                isFit = false;
                            }
                            break;

                        case EntityConstraints.OnlyVisible:
                            if (!_conditionalEntityHostSupport.IsVisible(instanceId))
                            {
                                isFit = false;
                            }
                            break;

                        case EntityConstraints.OnlyInvisible:
                            if (_conditionalEntityHostSupport.IsVisible(instanceId))
                            {
                                isFit = false;
                            }
                            break;

                        case EntityConstraints.Nearest:
                            var distanceToAndPosition = _conditionalEntityHostSupport.DistanceToAndPosition(instanceId);

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
                            break;

                        case EntityConstraints.Random:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null);
                    }
                }

#if DEBUG
                //Log($"isFit = {isFit}");
#endif

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

                filteredItemsList.Add(new FilteredItem()
                {
                    Id = foundId,
                    InstanceId = instanceId,
                    Position = currentPosition
                });
            }

            if (!filteredItemsList.Any())
            {
                ResetCurrEntity();
                return;
            }

            FilteredItem targetFilteredItem = default;

            if(_constraints.Any(p => p == EntityConstraints.Random))
            {
                var index = _random.Next(filteredItemsList.Count);

                targetFilteredItem = filteredItemsList[index];
            }
            else
            {
                targetFilteredItem = filteredItemsList.First();
            }

            SetCurrEntity(targetFilteredItem.Id, targetFilteredItem.InstanceId, targetFilteredItem.Position);
        }

        protected void SetCurrEntity(StrongIdentifierValue id)
        {
            var instanceId = _conditionalEntityHostSupport.GetInstanceId(id);

            if (instanceId == 0)
            {
                ResetCurrEntity();
                return;
            }

            SetCurrEntity(id, instanceId, null);
        }

        private void SetCurrEntity(StrongIdentifierValue id, int instanceId, Vector3? position)
        {
            if (!position.HasValue)
            {
                position = _conditionalEntityHostSupport.GetPosition(instanceId);
            }

            _id = id.NameValue;
            _idForFacts = _id;
            _instanceId = instanceId;
            _position = position;
            _isEmpty = false;
        }

        protected void ResetCurrEntity()
        {
            _id = string.Empty;
            _idForFacts = string.Empty;
            _instanceId = 0;
            _position = null;
            _isEmpty = true;
        }
    }
}

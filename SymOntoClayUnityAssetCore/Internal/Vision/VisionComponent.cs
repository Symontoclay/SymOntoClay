/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using Newtonsoft.Json;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Internal.Vision
{
    public class VisionComponent : BaseComponent
    {
        public VisionComponent(IMonitorLogger logger, IVisionProvider visionProvider, HumanoidNPCGameComponentContext internalContext, IWorldCoreGameComponentContext worldContext)
            : base(logger)
        {
            _internalContext = internalContext;
            _selfInstanceId = internalContext.SelfInstanceId;
            _worldContext = worldContext;
            _visionProvider = visionProvider;

            _activePeriodicObjectContext = new ActivePeriodicObjectContext(worldContext.SyncContext);
            _activeObject = new AsyncActivePeriodicObject(_activePeriodicObjectContext);
            _activeObject.PeriodicMethod = CommandLoop;
        }

        private readonly HumanoidNPCGameComponentContext _internalContext;
        private readonly IWorldCoreGameComponentContext _worldContext;
        private readonly int _selfInstanceId;
        private readonly IVisionProvider _visionProvider;
        private readonly IActivePeriodicObjectContext _activePeriodicObjectContext;
        private readonly AsyncActivePeriodicObject _activeObject;
        private readonly object _lockObj = new object();
        private string _idForFacts;
        private Engine _coreEngine;

        private Dictionary<int, VisibleItem> _visibleObjectsRegistry;
        private Dictionary<int, Vector3> _visibleObjectsPositionRegistry;
        private Dictionary<int, string> _visibleObjectsSeeFactsIdRegistry;
        private Dictionary<int, string> _visibleObjectsFocusFactsIdRegistry;
        private Dictionary<int, string> _visibleObjectsDistanceFactsIdRegistry;
        private Dictionary<int, IStorage> _visibleObjectsStoragesRegistry;
        private Dictionary<int, string> _visibleObjectsIdForFactsRegistry;

        private Dictionary<int, int> _lifeTimeCycleOfVisibleObjectsRegistry;

        private const int DEFAULT_INITIAL_TIME = 10;

        public void LoadFromSourceCode()
        {
            lock (_lockObj)
            {
                _visibleObjectsPositionRegistry = new Dictionary<int, Vector3>();
            }

            _visibleObjectsRegistry = new Dictionary<int, VisibleItem>();
            _visibleObjectsSeeFactsIdRegistry = new Dictionary<int, string>();
            _visibleObjectsFocusFactsIdRegistry = new Dictionary<int, string>();
            _visibleObjectsDistanceFactsIdRegistry = new Dictionary<int, string>();
            _visibleObjectsStoragesRegistry = new Dictionary<int, IStorage>();
            _visibleObjectsIdForFactsRegistry = new Dictionary<int, string>();

            _lifeTimeCycleOfVisibleObjectsRegistry = new Dictionary<int, int>();

            _idForFacts = _internalContext.IdForFacts;
            _coreEngine = _internalContext.CoreEngine;
        }

        public Vector3? GetPosition(int instanceId)
        {
            lock (_lockObj)
            {
                if (_visibleObjectsPositionRegistry.ContainsKey(instanceId))
                {
                    return _visibleObjectsPositionRegistry[instanceId];
                }

                return null;
            }
        }

        private bool CommandLoop(CancellationToken cancellationToken)
        {
            Thread.Sleep(200);
            
            var visibleItemsList = _visionProvider.GetCurrentVisibleItems();

            var availableInstanceIdList = _worldContext.AvailableInstanceIdList.Where(p => p != _selfInstanceId).ToList();

            visibleItemsList = visibleItemsList.Where(p => availableInstanceIdList.Contains(p.InstanceId)).ToList();
            var removedInstancesIdList = visibleItemsList.Select(p => p.InstanceId).ToList();

            var newVisibleItemsList = new List<VisibleItem>();
            var changedAddFocusVisibleItemsList = new List<VisibleItem>();
            var changedRemoveFocusVisibleItemsList = new List<VisibleItem>();
            var changedDistanceVisibleItemsList = new List<VisibleItem>();

            if (visibleItemsList.Any())
            {
                foreach (var visibleItem in visibleItemsList)
                {
                    var instanceId = visibleItem.InstanceId;

                    removedInstancesIdList.Remove(instanceId);

                    _lifeTimeCycleOfVisibleObjectsRegistry[instanceId] = DEFAULT_INITIAL_TIME;

                    if (_visibleObjectsRegistry.ContainsKey(instanceId))
                    {
                        var currentItem = _visibleObjectsRegistry[instanceId];

                        if (currentItem.IsInFocus != visibleItem.IsInFocus)
                        {
                            if(visibleItem.IsInFocus)
                            {
                                changedAddFocusVisibleItemsList.Add(visibleItem);
                            }
                            else
                            {
                                changedRemoveFocusVisibleItemsList.Add(visibleItem);
                            }                            
                        }
                        else
                        {
                            if(currentItem.MinDistance != visibleItem.MinDistance)
                            {
                                changedDistanceVisibleItemsList.Add(visibleItem);
                            }
                            else
                            {
                                if (currentItem.Position != visibleItem.Position)
                                {
                                    lock (_lockObj)
                                    {
                                        _visibleObjectsPositionRegistry[instanceId] = visibleItem.Position;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        newVisibleItemsList.Add(visibleItem);                        

                        lock (_lockObj)
                        {
                            _visibleObjectsPositionRegistry[instanceId] = visibleItem.Position;
                        }
                    }

                    _visibleObjectsRegistry[instanceId] = visibleItem;
                }
            }
            else
            {
                if (_visibleObjectsRegistry.Count > 0)
                {
                    removedInstancesIdList = _visibleObjectsRegistry.Keys.ToList();
                }
            }

            if (removedInstancesIdList.Any())
            {
                foreach (var removedInstancesId in removedInstancesIdList)
                {
                    var lifeCycle = _lifeTimeCycleOfVisibleObjectsRegistry[removedInstancesId];

                    if (lifeCycle > 0)
                    {
                        _lifeTimeCycleOfVisibleObjectsRegistry[removedInstancesId] = lifeCycle - 1;
                        continue;
                    }

                    _visibleObjectsRegistry.Remove(removedInstancesId);

                    lock (_lockObj)
                    {
                        _visibleObjectsPositionRegistry.Remove(removedInstancesId);
                    }

                    var storage = _visibleObjectsStoragesRegistry[removedInstancesId];

                    _visibleObjectsStoragesRegistry.Remove(removedInstancesId);

                    _coreEngine.RemoveVisibleStorage(Logger, storage);

                    _visibleObjectsIdForFactsRegistry.Remove(removedInstancesId);

                    if(_visibleObjectsSeeFactsIdRegistry.ContainsKey(removedInstancesId))
                    {
                        var factId = _visibleObjectsSeeFactsIdRegistry[removedInstancesId];
                        _visibleObjectsSeeFactsIdRegistry.Remove(removedInstancesId);

                        _coreEngine.RemovePerceptedFact(Logger, factId);
                    }

                    if(_visibleObjectsFocusFactsIdRegistry.ContainsKey(removedInstancesId))
                    {
                        var factId = _visibleObjectsFocusFactsIdRegistry[removedInstancesId];
                        _visibleObjectsFocusFactsIdRegistry.Remove(removedInstancesId);

                        _coreEngine.RemovePerceptedFact(Logger, factId);
                    }

                    if(_visibleObjectsDistanceFactsIdRegistry.ContainsKey(removedInstancesId))
                    {
                        var factId = _visibleObjectsDistanceFactsIdRegistry[removedInstancesId];
                        _visibleObjectsDistanceFactsIdRegistry.Remove(removedInstancesId);

                        _coreEngine.RemovePerceptedFact(Logger, factId);
                    }
                }
            }

            if (newVisibleItemsList.Any())
            {
                foreach(var newVisibleItem in newVisibleItemsList)
                {
                    var instanceId = newVisibleItem.InstanceId;

                    var storage = _worldContext.GetPublicFactsStorageByInstanceId(instanceId);

                    _visibleObjectsStoragesRegistry[instanceId] = storage;

                    _coreEngine.AddVisibleStorage(Logger, storage);

                    var idForFacts = _worldContext.GetIdForFactsByInstanceId(instanceId);

                    _visibleObjectsIdForFactsRegistry[instanceId] = idForFacts;

                    var seeFactStr = $"see(I, {idForFacts})";

                    _visibleObjectsSeeFactsIdRegistry[instanceId] = _coreEngine.InsertPerceptedFact(Logger, seeFactStr);

                    if (newVisibleItem.IsInFocus)
                    {
                        var focusFactStr = $"focus(I, {idForFacts})";

                        _visibleObjectsFocusFactsIdRegistry[instanceId] = _coreEngine.InsertPerceptedFact(Logger, focusFactStr);
                    }

                    var distanceFactStr = $"distance(I, {idForFacts}, {newVisibleItem.MinDistance.ToString("G", CultureInfo.InvariantCulture)})";

                    _visibleObjectsDistanceFactsIdRegistry[instanceId] = _coreEngine.InsertPerceptedFact(Logger, distanceFactStr);
                }
            }

            if(changedAddFocusVisibleItemsList.Any())
            {
                foreach(var changedAddFocusVisibleItem in changedAddFocusVisibleItemsList)
                {
                    var instanceId = changedAddFocusVisibleItem.InstanceId;

                    var idForFacts = _visibleObjectsIdForFactsRegistry[instanceId];

                    var focusFactStr = $"focus(I, {idForFacts})";

                    _visibleObjectsFocusFactsIdRegistry[instanceId] = _coreEngine.InsertPerceptedFact(Logger, focusFactStr);
                }
            }

            if (changedRemoveFocusVisibleItemsList.Any())
            {
                foreach (var changedRemoveFocusVisibleItem in changedRemoveFocusVisibleItemsList)
                {
                    var instanceId = changedRemoveFocusVisibleItem.InstanceId;

                    var factId = _visibleObjectsFocusFactsIdRegistry[instanceId];
                    _visibleObjectsFocusFactsIdRegistry.Remove(instanceId);

                    _coreEngine.RemovePerceptedFact(Logger, factId);
                }
            }

            if (changedDistanceVisibleItemsList.Any())
            {
                foreach(var item in changedDistanceVisibleItemsList)
                {
                    var instanceId = item.InstanceId;

                    _coreEngine.RemovePerceptedFact(Logger, _visibleObjectsDistanceFactsIdRegistry[instanceId]);

                    var idForFacts = _worldContext.GetIdForFactsByInstanceId(instanceId);

                    var distanceFactStr = $"distance(I, {idForFacts}, {item.MinDistance.ToString("G", CultureInfo.InvariantCulture)})";

                    _visibleObjectsDistanceFactsIdRegistry[instanceId] = _coreEngine.InsertPerceptedFact(Logger, distanceFactStr);
                }
            }

            return true;
        }

        public bool IsVisible(int instanceId)
        {
            if(_visibleObjectsRegistry.ContainsKey(instanceId))
            {
                return true;
            }

            return false;
        }

        public void BeginStarting()
        {
            _activeObject.Start();
        }

        public bool IsWaited
        {
            get
            {
                return _activeObject.IsWaited;
            }
        }

        public void Die()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _activeObject.Dispose();

            base.OnDisposed();
        }
    }
}

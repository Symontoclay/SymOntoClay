/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
        public VisionComponent(IEntityLogger logger, IVisionProvider visionProvider, HumanoidNPCGameComponentContext internalContext, IWorldCoreGameComponentContext worldContext)
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

        private bool CommandLoop()
        {
            Thread.Sleep(200);
            
#if DEBUG
            //Log("Do");
#endif

            var visibleItemsList = _visionProvider.GetCurrentVisibleItems();

#if DEBUG
            //Log($"visibleItemsList = {visibleItemsList.WriteListToString()}");
#endif

            var availableInstanceIdList = _worldContext.AvailableInstanceIdList.Where(p => p != _selfInstanceId).ToList();

#if DEBUG
            //Log($"availableInstanceIdList = [{string.Join(",",availableInstanceIdList)}]");
#endif

            visibleItemsList = visibleItemsList.Where(p => availableInstanceIdList.Contains(p.InstanceId)).ToList();
            var removedInstancesIdList = visibleItemsList.Select(p => p.InstanceId).ToList();

#if DEBUG
            //Log($"visibleItemsList (after) = {visibleItemsList.WriteListToString()}");
#endif

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
#if DEBUG
                            //Log("currentItem.IsInFocus != visibleItem.IsInFocus");
#endif

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
#if DEBUG
                                Log("currentItem.MinDistance != visibleItem.MinDistance");
#endif
                                changedDistanceVisibleItemsList.Add(visibleItem);
                            }
                            else
                            {
                                if (currentItem.Position != visibleItem.Position)
                                {
#if DEBUG
                                    //Log($"currentItem.Position = {currentItem.Position}");
                                    //Log($"visibleItem.Position = {visibleItem.Position}");
                                    //Log("currentItem.Position != visibleItem.Position");
#endif
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

#if DEBUG
            //Log($"removedInstancesIdList.Count = {removedInstancesIdList.Count}");
            //Log($"newVisibleItemsList.Count = {newVisibleItemsList.Count}");
            //Log($"changedAddFocusVisibleItemsList.Count = {changedAddFocusVisibleItemsList.Count}");
            //Log($"changedRemoveFocusVisibleItemsList.Count = {changedRemoveFocusVisibleItemsList.Count}");
            //Log($"changedDistanceVisibleItemsList.Count = {changedDistanceVisibleItemsList.Count}");
#endif

            if (removedInstancesIdList.Any())
            {
                foreach (var removedInstancesId in removedInstancesIdList)
                {
#if DEBUG
                    //Log($"removedInstancesId = {removedInstancesId}");
#endif

                    var lifeCycle = _lifeTimeCycleOfVisibleObjectsRegistry[removedInstancesId];

#if DEBUG
                    //Log($"lifeCycle = {lifeCycle}");
#endif

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

                    _coreEngine.RemoveVisibleStorage(storage);

                    _visibleObjectsIdForFactsRegistry.Remove(removedInstancesId);

                    if(_visibleObjectsSeeFactsIdRegistry.ContainsKey(removedInstancesId))
                    {
                        var factId = _visibleObjectsSeeFactsIdRegistry[removedInstancesId];
                        _visibleObjectsSeeFactsIdRegistry.Remove(removedInstancesId);

                        _coreEngine.RemovePerceptedFact(factId);
                    }

                    if(_visibleObjectsFocusFactsIdRegistry.ContainsKey(removedInstancesId))
                    {
                        var factId = _visibleObjectsFocusFactsIdRegistry[removedInstancesId];
                        _visibleObjectsFocusFactsIdRegistry.Remove(removedInstancesId);

                        _coreEngine.RemovePerceptedFact(factId);
                    }

                    if(_visibleObjectsDistanceFactsIdRegistry.ContainsKey(removedInstancesId))
                    {
                        var factId = _visibleObjectsDistanceFactsIdRegistry[removedInstancesId];
                        _visibleObjectsDistanceFactsIdRegistry.Remove(removedInstancesId);

                        _coreEngine.RemovePerceptedFact(factId);
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

                    _coreEngine.AddVisibleStorage(storage);

                    var idForFacts = _worldContext.GetIdForFactsByInstanceId(instanceId);

                    _visibleObjectsIdForFactsRegistry[instanceId] = idForFacts;

                    var seeFactStr = $"see(I, {idForFacts})";

#if DEBUG
                    //Log($"seeFactStr = {seeFactStr}");
#endif

                    _visibleObjectsSeeFactsIdRegistry[instanceId] = _coreEngine.InsertPerceptedFact(seeFactStr);

                    if (newVisibleItem.IsInFocus)
                    {
                        var focusFactStr = $"focus(I, {idForFacts})";

#if DEBUG
                        //Log($"focusFactStr = {focusFactStr}");
#endif

                        _visibleObjectsFocusFactsIdRegistry[instanceId] = _coreEngine.InsertPerceptedFact(focusFactStr);
                    }

                    var distanceFactStr = $"distance(I, {idForFacts}, {newVisibleItem.MinDistance.ToString("G", CultureInfo.InvariantCulture)})";

#if DEBUG
                    //Log($"distanceFactStr = {distanceFactStr}");
#endif

                    _visibleObjectsDistanceFactsIdRegistry[instanceId] = _coreEngine.InsertPerceptedFact(distanceFactStr);
                }
                //throw new NotImplementedException();
            }

            if(changedAddFocusVisibleItemsList.Any())
            {
                foreach(var changedAddFocusVisibleItem in changedAddFocusVisibleItemsList)
                {
                    var instanceId = changedAddFocusVisibleItem.InstanceId;

                    var idForFacts = _visibleObjectsIdForFactsRegistry[instanceId];

                    var focusFactStr = $"focus(I, {idForFacts})";

#if DEBUG
                    //Log($"focusFactStr = {focusFactStr}");
#endif

                    _visibleObjectsFocusFactsIdRegistry[instanceId] = _coreEngine.InsertPerceptedFact(focusFactStr);
                }
            }

            if (changedRemoveFocusVisibleItemsList.Any())
            {
                foreach (var changedRemoveFocusVisibleItem in changedRemoveFocusVisibleItemsList)
                {
                    var instanceId = changedRemoveFocusVisibleItem.InstanceId;

                    var factId = _visibleObjectsFocusFactsIdRegistry[instanceId];
                    _visibleObjectsFocusFactsIdRegistry.Remove(instanceId);

                    _coreEngine.RemovePerceptedFact(factId);
                }
            }

            if (changedDistanceVisibleItemsList.Any())
            {
                foreach(var item in changedDistanceVisibleItemsList)
                {
                    var instanceId = item.InstanceId;

                    _coreEngine.RemovePerceptedFact(_visibleObjectsDistanceFactsIdRegistry[instanceId]);

                    var idForFacts = _worldContext.GetIdForFactsByInstanceId(instanceId);

                    var distanceFactStr = $"distance(I, {idForFacts}, {item.MinDistance.ToString("G", CultureInfo.InvariantCulture)})";

#if DEBUG
                    Log($"distanceFactStr = {distanceFactStr}");
#endif

                    _visibleObjectsDistanceFactsIdRegistry[instanceId] = _coreEngine.InsertPerceptedFact(distanceFactStr);
                }
            }

            return true;
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

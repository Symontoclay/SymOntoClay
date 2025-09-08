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

using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
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
            _dateTimeProvider = worldContext.DateTimeProvider;
            _standardFactsBuilder = worldContext.StandardFactsBuilder;

            _activeObjectContext = new ActiveObjectContext(worldContext.SyncContext, internalContext.CancellationToken);
            _activeObject = new AsyncActivePeriodicObject(_activeObjectContext, null, logger);
            _activeObject.PeriodicMethod = CommandLoop;
        }

        private readonly HumanoidNPCGameComponentContext _internalContext;
        private readonly IWorldCoreGameComponentContext _worldContext;
        private readonly IStandardFactsBuilder _standardFactsBuilder;
        private readonly int _selfInstanceId;
        private readonly IVisionProvider _visionProvider;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IActiveObjectContext _activeObjectContext;
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

            var currentTimeStamp = _dateTimeProvider.CurrentTicks;

            var visibleItemsList = _visionProvider.GetCurrentVisibleItems();

#if DEBUG
            Info("B80531CD-1049-4780-9D76-C794FB7EA29C", $"visibleItemsList.Count = {visibleItemsList.Count}");
            Info("96741E62-DF10-4353-8051-23688BD76B70", $"_visibleObjectsIdForFactsRegistry.Count = {_visibleObjectsIdForFactsRegistry.Count}");
#endif

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
                    var visibleItemInstanceId = visibleItem.InstanceId;

                    removedInstancesIdList.Remove(visibleItemInstanceId);

                    _lifeTimeCycleOfVisibleObjectsRegistry[visibleItemInstanceId] = DEFAULT_INITIAL_TIME;

                    if (_visibleObjectsRegistry.ContainsKey(visibleItemInstanceId))
                    {
                        var currentItem = _visibleObjectsRegistry[visibleItemInstanceId];

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
                                        _visibleObjectsPositionRegistry[visibleItemInstanceId] = visibleItem.Position;
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
                            _visibleObjectsPositionRegistry[visibleItemInstanceId] = visibleItem.Position;
                        }
                    }

                    _visibleObjectsRegistry[visibleItemInstanceId] = visibleItem;
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
                    var visibleItemInstanceId = newVisibleItem.InstanceId;

                    var storage = _worldContext.GetPublicFactsStorageByInstanceId(visibleItemInstanceId);

                    _visibleObjectsStoragesRegistry[visibleItemInstanceId] = storage;

                    _coreEngine.AddVisibleStorage(Logger, storage);

                    var visibleItemIdForFacts = _worldContext.GetIdForFactsByInstanceId(visibleItemInstanceId);

                    _visibleObjectsIdForFactsRegistry[visibleItemInstanceId] = visibleItemIdForFacts;

#if DEBUG
                    Info("2AEAE968-5454-4DA6-A7C1-4D45FF825E94", $"visibleItemIdForFacts = {visibleItemIdForFacts}");
                    storage.DbgPrintFactsAndRules(Logger);
#endif

                    var seeFact = _standardFactsBuilder.BuildSeeFactInstance(visibleItemIdForFacts);
                    seeFact.TimeStamp = currentTimeStamp;

#if DEBUG
                    Info("B1284244-F00E-44AA-93F0-335F4B0D7BD3", $"seeFact = {seeFact.ToHumanizedString()}");
#endif

                    _visibleObjectsSeeFactsIdRegistry[visibleItemInstanceId] = _coreEngine.InsertPerceptedFact(Logger, seeFact);

                    if (newVisibleItem.IsInFocus)
                    {
                        var focusFact = _standardFactsBuilder.BuildFocusFactInstance(visibleItemIdForFacts);
                        focusFact.TimeStamp = currentTimeStamp;

#if DEBUG
                        Info("7791F7AE-F1CA-4C31-8396-F231DBE8A924", $"focusFact = {focusFact.ToHumanizedString()}");
#endif

                        _visibleObjectsFocusFactsIdRegistry[visibleItemInstanceId] = _coreEngine.InsertPerceptedFact(Logger, focusFact);
                    }

                    var distanceFactStr = $"distance(I, {visibleItemIdForFacts}, {newVisibleItem.MinDistance.ToString("G", CultureInfo.InvariantCulture)})";

#if DEBUG
                    Info("C41A8ACD-5ED8-4DAD-8309-DB85FF481F65", $"distanceFactStr = {distanceFactStr}");
#endif

                    _visibleObjectsDistanceFactsIdRegistry[visibleItemInstanceId] = _coreEngine.InsertPerceptedFact(Logger, distanceFactStr);
                }
            }

            if(changedAddFocusVisibleItemsList.Any())
            {
                foreach(var changedAddFocusVisibleItem in changedAddFocusVisibleItemsList)
                {
                    var visibleItemInstanceId = changedAddFocusVisibleItem.InstanceId;

                    var visibleItemIdForFacts = _visibleObjectsIdForFactsRegistry[visibleItemInstanceId];

                    var focusFact = _standardFactsBuilder.BuildFocusFactInstance(visibleItemIdForFacts);
                    focusFact.TimeStamp = currentTimeStamp;

                    _visibleObjectsFocusFactsIdRegistry[visibleItemInstanceId] = _coreEngine.InsertPerceptedFact(Logger, focusFact);
                }
            }

            if (changedRemoveFocusVisibleItemsList.Any())
            {
                foreach (var changedRemoveFocusVisibleItem in changedRemoveFocusVisibleItemsList)
                {
                    var visibleItemInstanceId = changedRemoveFocusVisibleItem.InstanceId;

                    var factId = _visibleObjectsFocusFactsIdRegistry[visibleItemInstanceId];
                    _visibleObjectsFocusFactsIdRegistry.Remove(visibleItemInstanceId);

                    _coreEngine.RemovePerceptedFact(Logger, factId);
                }
            }

            if (changedDistanceVisibleItemsList.Any())
            {
                foreach(var item in changedDistanceVisibleItemsList)
                {
                    var visibleItemInstanceId = item.InstanceId;

                    _coreEngine.RemovePerceptedFact(Logger, _visibleObjectsDistanceFactsIdRegistry[visibleItemInstanceId]);

                    var visibleItemIdForFacts = _worldContext.GetIdForFactsByInstanceId(visibleItemInstanceId);

                    var distanceFact = _standardFactsBuilder.BuildDistanceFactInstance(visibleItemIdForFacts, item.MinDistance);
                    distanceFact.TimeStamp = currentTimeStamp;

                    _visibleObjectsDistanceFactsIdRegistry[visibleItemInstanceId] = _coreEngine.InsertPerceptedFact(Logger, distanceFact);
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
            _activeObject.Dispose();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _activeObject.Dispose();

            base.OnDisposed();
        }
    }
}

using Newtonsoft.Json;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.MonoBehaviourTesting
{
    public class TstRayScaner : IVisionProvider
    {
        private readonly IEntityLogger _logger = new LoggerImpementation();

        public TstRayScaner()
        {
            for (var i = 2; i <= 3; i++)
            {
                _fakeIdsList.Add(i);
            }
        }

        private readonly List<int> _fakeIdsList = new List<int>();

        public void SetNPC(IHumanoidNPC npc)
        {
            _npc = npc;
        }

        private IHumanoidNPC _npc;
        private List<TstUVisibleItem> _rawVisibleItemsList = new List<TstUVisibleItem>();
        private bool _isRawVisibleItemsListChanged;
        private readonly object _lockObj = new object();
        private readonly Random _random = new Random();
        private List<VisibleItem> _result = new List<VisibleItem>();

        public void Scan()
        {
            //_logger.Log("Begin");

            var targetFakeIdsList = new List<int>();

            var targetFakeIdsPositions = new Dictionary<int, Vector3>();

            for (var i = 1; i <= 5; i++)
            {
                var targetFakeId = _fakeIdsList[_random.Next(0, _fakeIdsList.Count - 1)];

                targetFakeIdsList.Add(targetFakeId);

                var position = new Vector3(_random.Next(0, 1000), _random.Next(0, 1000), 0);

                targetFakeIdsPositions[targetFakeId] = position;
            }

            targetFakeIdsList = targetFakeIdsList.Distinct().ToList();

            //_logger.Log($"targetFakeIdsList = {JsonConvert.SerializeObject(targetFakeIdsList, Formatting.Indented)}");
            //_logger.Log($"targetFakeIdsPositions = {JsonConvert.SerializeObject(targetFakeIdsPositions, Formatting.Indented)}");

            var newRawVisibleItemsList = new List<TstUVisibleItem>();

            var minDistanceRange = _random.Next(5, 100);

            for (var i = 1; i <= 1000; i++)
            {
                //_logger.Log($"i = {i}");

                var visibleItem = new TstUVisibleItem();
                var targetFakeId = targetFakeIdsList[_random.Next(0, targetFakeIdsList.Count - 1)];
                visibleItem.InstanceId = targetFakeId;
                visibleItem.Position = targetFakeIdsPositions[targetFakeId];
                visibleItem.Distance = _random.Next(minDistanceRange, 100);
                visibleItem.IsInFocus = (i > 500 && i < 600);

                newRawVisibleItemsList.Add(visibleItem);
            }

            //_logger.Log($"newRawVisibleItemsList = {newRawVisibleItemsList.WriteListToString()}");

            lock (_lockObj)
            {
                _rawVisibleItemsList = newRawVisibleItemsList;
                _isRawVisibleItemsListChanged = true;
            }

            //_logger.Log("End");
        }

        public IList<VisibleItem> GetCurrentVisibleItems()
        {
            List<TstUVisibleItem> rawVisibleItemsList;

            lock (_lockObj)
            {
                if(!_isRawVisibleItemsListChanged)
                {
                    return _result;
                }

                _isRawVisibleItemsListChanged = false;

                rawVisibleItemsList = _rawVisibleItemsList;
            }

            var result = new List<VisibleItem>();

            var rawVisibleItemsDict = rawVisibleItemsList.GroupBy(p => p.InstanceId);

            foreach(var rawVisibleItemsKVPItem in rawVisibleItemsDict)
            {
                var firstElem = rawVisibleItemsKVPItem.First();

                var item = new VisibleItem();
                item.InstanceId = firstElem.InstanceId;
                item.Position = firstElem.Position;
                item.IsInFocus = rawVisibleItemsKVPItem.Any(p => p.IsInFocus);
                item.MinDistance = rawVisibleItemsKVPItem.Min(p => p.Distance);

                result.Add(item);
            }

            _result = result;

            return result;
        }
    }
}

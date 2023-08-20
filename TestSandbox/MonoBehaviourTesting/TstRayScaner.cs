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

using Newtonsoft.Json;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
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
        private readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

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


            var newRawVisibleItemsList = new List<TstUVisibleItem>();

            var minDistanceRange = _random.Next(5, 100);

            for (var i = 1; i <= 1000; i++)
            {

                var visibleItem = new TstUVisibleItem();
                var targetFakeId = targetFakeIdsList[_random.Next(0, targetFakeIdsList.Count - 1)];
                visibleItem.InstanceId = targetFakeId;
                visibleItem.Position = targetFakeIdsPositions[targetFakeId];
                visibleItem.Distance = _random.Next(minDistanceRange, 100);
                visibleItem.IsInFocus = (i > 500 && i < 600);

                newRawVisibleItemsList.Add(visibleItem);
            }


            lock (_lockObj)
            {
                _rawVisibleItemsList = newRawVisibleItemsList;
                _isRawVisibleItemsListChanged = true;
            }

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

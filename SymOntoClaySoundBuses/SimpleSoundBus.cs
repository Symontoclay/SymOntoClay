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

using NLog;
using SymOntoClay.Common.Disposing;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Threading;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.SoundBuses
{
    public class SimpleSoundBus : Disposable, ISoundBus
    {
        public SimpleSoundBus(SimpleSoundBusSettings settings = null)
        {
            _settings = settings;

            _cancellationTokenSource = new CancellationTokenSource();
            _linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, settings?.CancellationToken ?? CancellationToken.None);

            var threadingSettings = settings?.ThreadingSettings?.AsyncEvents;

            _threadPool = new CustomThreadPool(threadingSettings?.MinThreadsCount ?? DefaultCustomThreadPoolSettings.MinThreadsCount,
                threadingSettings?.MaxThreadsCount ?? DefaultCustomThreadPoolSettings.MaxThreadsCount,
                _linkedCancellationTokenSource.Token);
        }

        private readonly SimpleSoundBusSettings _settings;
        private readonly ICustomThreadPool _threadPool;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationTokenSource _linkedCancellationTokenSource;

        /// <inheritdoc/>
        public void AddReceiver(ISoundReceiver receiver)
        {
            lock (_lockObj)
            {
                if (_soundReceivers.Contains(receiver))
                {
                    return;
                }

                _soundReceivers.Add(receiver);
            }
        }

        /// <inheritdoc/>
        public void RemoveReceiver(ISoundReceiver receiver)
        {
            lock (_lockObj)
            {
                if (_soundReceivers.Contains(receiver))
                {
                    _soundReceivers.Remove(receiver);
                }
            }
        }

        /// <inheritdoc/>
        public void PushSound(int instanceId, float power, Vector3 position, string query)
        {
            lock (_lockObj)
            {
                if (!query.StartsWith("{:"))
                {
                    query = $"{{: {query} :}}";
                }

                foreach (var receiver in _soundReceivers)
                {
                    if (receiver.InstanceId == instanceId)
                    {
                        continue;
                    }

                    var distance = Vector3.Distance(receiver.Position, position);

                    var targetPower = power - 0.04 * distance;

                    if (targetPower < receiver.Threshold)
                    {
                        continue;
                    }

                    var logger = receiver.Logger;

                    SymOntoClay.Core.Internal.Threads.ThreadTask.Run(() => {
                        var taskId = logger.StartTask("A513EA05-6D7D-4EAD-A9CE-EDC57E09B067");

                        try
                        {
                            receiver.CallBack(targetPower, distance, position, query);
                        }
                        catch (Exception e)
                        {
                            logger.Error("A05B5697-3825-42D3-9CAD-3C07A8AE4BC0", e);
                        }

                        logger.StopTask("C9EA86C3-305A-4B5C-8A44-645155B32619", taskId);
                    }, _threadPool, _linkedCancellationTokenSource.Token);
                }
            }
        }

        public void PushSound(int instanceId, float power, Vector3 position, RuleInstance fact)
        {
            lock (_lockObj)
            {
                foreach (var receiver in _soundReceivers)
                {
                    if (receiver.InstanceId == instanceId)
                    {
                        continue;
                    }

                    var distance = Vector3.Distance(receiver.Position, position);

                    var targetPower = power - 0.04 * distance;

                    if (targetPower < receiver.Threshold)
                    {
                        continue;
                    }

                    var logger = receiver.Logger;

                    SymOntoClay.Core.Internal.Threads.ThreadTask.Run(() => {
                        var taskId = logger.StartTask("3E6AB919-D75A-4A56-8C8A-D6F0C12C1B7E");

                        try
                        {
                            receiver.CallBack(targetPower, distance, position, fact);
                        }
                        catch (Exception e)
                        {
                            logger.Error("40199609-AAAF-4DFB-A694-14295A8B6EC0", e);
                        }

                        logger.StopTask("FAB648D7-58F3-4986-A7A3-8C8BCC6D6DCB", taskId);
                    }, _threadPool, _linkedCancellationTokenSource.Token);
                }
            }
        }

        private object _lockObj = new object();
        private List<ISoundReceiver> _soundReceivers = new List<ISoundReceiver>();

        /// <inheritdoc/>

        protected override void OnDisposing()
        {
            _cancellationTokenSource?.Cancel();

            base.OnDisposing();
        }
    }
}

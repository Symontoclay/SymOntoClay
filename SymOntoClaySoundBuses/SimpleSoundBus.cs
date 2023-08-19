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

using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.SoundBuses
{
    public class SimpleSoundBus : ISoundBus
    {
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

                Task.Run(() => {
                    try
                    {
                        receiver.CallBack(targetPower, distance, position, query);
                    }
                    catch (Exception e)
                    {
                        receiver.Logger.Error("A05B5697-3825-42D3-9CAD-3C07A8AE4BC0", e);
                    }
                });
            }
        }

        public void PushSound(int instanceId, float power, Vector3 position, RuleInstance fact)
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

                Task.Run(() => {
                    try
                    {
                        receiver.CallBack(targetPower, distance, position, fact);
                    }
                    catch (Exception e)
                    {
                        receiver.Logger.Error("40199609-AAAF-4DFB-A694-14295A8B6EC0", e);
                    }
                });
            }
        }

        private object _lockObj = new object();
        private List<ISoundReceiver> _soundReceivers = new List<ISoundReceiver>();
    }
}

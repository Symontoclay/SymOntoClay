using NLog;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.SoundBusHandler
{
    public class SimpleSoundBus : ISoundBus
    {
#if DEBUG
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        /// <inheritdoc/>
        public void AddReceiver(ISoundReceiver receiver)
        {
            lock(_lockObj)
            {
                if(_soundReceivers.Contains(receiver))
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
#if DEBUG
            //_logger.Info($"instanceId = {instanceId}");
            //_logger.Info($"power = {power}");
            //_logger.Info($"position = {position}");
            //_logger.Info($"query = {query}");
#endif

            foreach(var receiver in _soundReceivers)
            {
#if DEBUG
                //_logger.Info($"receiver.Position = {receiver.Position}");
                //_logger.Info($"receiver.Threshold = {receiver.Threshold}");
#endif

                if(receiver.InstanceId == instanceId)
                {
                    continue;
                }

                var distance = Vector3.Distance(receiver.Position, position);

#if DEBUG
                //_logger.Info($"distance = {distance}");
#endif

                var targetPower = power - 0.04 * distance;

#if DEBUG
                //_logger.Info($"targetPower = {targetPower}");
#endif

                if(targetPower < receiver.Threshold)
                {
                    continue;
                }

                Task.Run(() => {
                    try
                    {
                        receiver.CallBack(targetPower, distance, position, query);
                    }
                    catch(Exception e)
                    {
                        receiver.Logger.Error(e.ToString());
                    }                    
                });
            }
        }

        private object _lockObj = new object();
        private List<ISoundReceiver> _soundReceivers = new List<ISoundReceiver>();
    }
}

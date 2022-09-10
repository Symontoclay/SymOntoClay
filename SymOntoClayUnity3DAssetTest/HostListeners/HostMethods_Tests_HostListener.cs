using SymOntoClay.Core.Tests.Helpers;
using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.HostListeners
{
    public class HostMethods_Tests_HostListener : BaseHostListener
    {
        [BipedEndpoint("Boo", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void BooImpl(CancellationToken cancellationToken)
        {
            _logger.Log("BooImpl Begin");
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, float? direction)
        {
            _logger.Log("RotateImpl Begin");
            _logger.Log(direction.ToString());
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateToEntityImpl(CancellationToken cancellationToken, IEntity entity,
            float speed = 2)
        {
            _logger.Log("RotateToEntityImpl Begin");
        }

        [BipedEndpoint("Take", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeImpl(CancellationToken cancellationToken, IEntity entity)
        {
            _logger.Log("TakeImpl Begin");
            _logger.Log($"(entity == null) = {entity == null}");

            if (entity == null)
            {
                return;
            }

            entity.Specify(EntityConstraints.CanBeTaken, EntityConstraints.Nearest);

            entity.Resolve();

            _logger.Log(entity.InstanceId.ToString());
            _logger.Log(entity.Id);
            _logger.Log(entity.Position.ToString());

            _logger.Log($"TakeImpl End");
        }
    }
}

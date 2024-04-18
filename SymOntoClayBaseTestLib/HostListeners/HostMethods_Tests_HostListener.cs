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

using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.BaseTestLib.HostListeners
{
    public class HostMethods_Tests_HostListener : BaseHostListener
    {
        [BipedEndpoint("Boo", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void BooImpl(CancellationToken cancellationToken, IMonitorLogger logger)
        {
            logger.Output("92F545C4-2D3A-42BF-84FC-B7F9F51A830A", "BooImpl Begin");
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateImpl(CancellationToken cancellationToken, IMonitorLogger logger, float? direction)
        {
            logger.Output("369BBB1E-9DA6-480C-8BA2-A941382481B2", "RotateImpl Begin");
            logger.Output("6048DAC0-371D-4859-B83D-AF8645D3C1C5", direction.ToString());
        }

        [BipedEndpoint("Rotate", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void RotateToEntityImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity,
            float speed = 2)
        {
            logger.Output("453A45AD-5878-4F88-BD91-BA5643831B48", "RotateToEntityImpl Begin");
        }

        [BipedEndpoint("Take", DeviceOfBiped.RightHand, DeviceOfBiped.LeftHand)]
        public void TakeImpl(CancellationToken cancellationToken, IMonitorLogger logger, IEntity entity)
        {
            logger.Output("D87D0436-F1B1-4D81-9D0E-8A45ECFE5C14", "TakeImpl Begin");
            logger.Output("075D7DF5-0EAC-4C0A-AB07-EDB9B897EF52", $"(entity == null) = {entity == null}");

            if (entity == null)
            {
                return;
            }

            entity.Specify(logger, EntityConstraints.CanBeTaken, EntityConstraints.Nearest);

            entity.Resolve(logger);

            logger.Output("A77DC48E-F86E-4CF6-9341-3B62A18EFC4C", entity.InstanceId.ToString());
            logger.Output("C56BCDE0-9BB8-455C-8434-E2F881E13248", entity.Id);
            logger.Output("4161FB63-DAC2-4821-BF40-D99F6115FC3C", entity.Position.ToString());

            logger.Output("E596E7CE-E5F4-40DC-B54E-24819A36424E", $"TakeImpl End");
        }
    }
}

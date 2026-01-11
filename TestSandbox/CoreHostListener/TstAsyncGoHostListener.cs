/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.BaseTestLib.HostListeners;
using SymOntoClay.Core;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestSandbox.CoreHostListener
{
    public class TstAsyncGoHostListener : BaseHostListener
    {
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public async Task GoToImpl(CancellationToken cancellationToken, IMonitorLogger logger,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            logger.Info("13095D9A-60D2-4F90-B4D0-B86347BFE14B", $"GoToImpl Begin");
            logger.Info("2D0BBA68-71A8-4634-ACDF-F698184559EF", $"navTarget.Kind = {navTarget.Kind}");
            var entity = navTarget.Entity;
            logger.Info("7170FAF4-E2C1-4B2E-BED5-62923C162CD8", $"entity.InstanceId = {entity.InstanceId}");
            logger.Info("30532211-925F-4E4C-A615-952BC335C12B", $"entity.Id = {entity.Id}");
            logger.Info("DE7754AD-469B-4BAD-9ADC-D8051147CE3B", $"entity.Position = {entity.Position}");

            await SomeMethod();

            logger.Info("729E564E-FFA8-456B-A09C-D9BE819C73DA", $"GoToImpl End");
        }

        private Task SomeMethod()
        {
            return Task.Run(() => { Thread.Sleep(10000); });
        }

        /*
         [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public async void GoToImpl(CancellationToken cancellationToken, IMonitorLogger logger,
        [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget target,
        float speed = 12)
         */
    }
}

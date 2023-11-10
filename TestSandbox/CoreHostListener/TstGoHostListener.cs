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

using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.BaseTestLib.HostListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SymOntoClay.Monitor.Common;

namespace TestSandbox.CoreHostListener
{
    public class TstGoHostListener : BaseHostListener
    {
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl(CancellationToken cancellationToken, IMonitorLogger logger,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            logger.Info("FDAF0616-4EEB-4FF6-A30D-789B10DE2472", $"GoToImpl Begin");
            logger.Info("35A18046-C1D0-49BB-817A-4EEA05007A30", $"navTarget.Kind = {navTarget.Kind}");
            var entity = navTarget.Entity;
            logger.Info("B2201525-B34E-4E68-9F64-7B33525B08C6", $"entity.InstanceId = {entity.InstanceId}");
            logger.Info("03B35534-77FF-45E0-B362-3F5216521200", $"entity.Id = {entity.Id}");
            logger.Info("54C307CD-DA2A-48E4-B420-F438A0C6D61B", $"entity.Position = {entity.Position}");

            Thread.Sleep(10000);

            logger.Info("AAFCC9C1-A54B-4C0D-A698-7938FA8F1133", $"GoToImpl End");
        }
    }
}

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
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.HostListeners
{
    public class Exec_Tests_HostListener4 : BaseHostListener
    {
        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl(CancellationToken cancellationToken, IMonitorLogger logger,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            logger.Output("D0B89970-6652-453E-A51E-765EB7FF7762", $"GoToImpl Begin");
            logger.Output("A41F05F0-2C5A-45B3-ACB2-146F4D4D35CB", navTarget.Kind.ToString());
            var entity = navTarget.Entity;
            logger.Output("8DB4669E-0D54-4536-A544-5E8E0BA28BA4", entity.InstanceId.ToString());
            logger.Output("498ED940-F675-4AC3-99C4-BEE8134F4929", entity.Id);
            logger.Output("785435A4-1581-4382-9A8F-5EC9DB9ED9FE", entity.Position.ToString());
            logger.Output("2DA83A73-CC23-4D67-B489-478DA0EDEF34", $"GoToImpl End");
        }
    }
}

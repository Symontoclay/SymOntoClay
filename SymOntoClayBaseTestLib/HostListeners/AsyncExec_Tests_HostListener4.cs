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

using SymOntoClay.Core;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib.HostListeners
{
    public class AsyncExec_Tests_HostListener4 : BaseHostListener
    {
        public AsyncExec_Tests_HostListener4()
            : this(null)
        {
        }

        public AsyncExec_Tests_HostListener4(int? millisecondsTimeout) 
        {
            _millisecondsTimeout = millisecondsTimeout;
        }

        private readonly int? _millisecondsTimeout;

        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public async Task GoToImpl(CancellationToken cancellationToken, IMonitorLogger logger,
            [EndpointParam("To", KindOfEndpointParam.Position)] INavTarget navTarget,
            float speed = 12)
        {
            logger.Output("B94C216F-4175-4EB1-9350-6B12E98A2E5E", $"GoToImpl Begin");
            logger.Output("524B99F0-B4C8-4E58-AEB2-AE1BC0802C5C", navTarget.Kind.ToString());
            var entity = navTarget.Entity;
            logger.Output("1F72AAAB-1C46-4C41-8CCA-E9321D269885", entity.InstanceId.ToString());
            logger.Output("CB89278E-8427-4979-840F-ED8F1B676279", entity.Id);
            logger.Output("34637740-A848-41E8-A445-DB51CAB2FDED", entity.Position.ToString());

            await SomeMethod(cancellationToken);

            logger.Output("2A2734D1-BB06-4B8E-B59F-BE37318A8751", $"GoToImpl End");
        }

        private Task SomeMethod(CancellationToken cancellationToken)
        {
            return Task.Run(() => { 
                if(_millisecondsTimeout.HasValue)
                {
                    Sleep(_millisecondsTimeout.Value, cancellationToken);
                }                
            });
        }
    }
}

using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.CoreHostListener
{
    public class EndPointActivator : BaseLoggedComponent
    {
        public EndPointActivator(IEntityLogger logger, IPlatformTypesConvertorsRegistry platformTypesConvertorsRegistry)
            : base(logger)
        {
            _platformTypesConvertorsRegistry = platformTypesConvertorsRegistry;
        }

        private readonly IPlatformTypesConvertorsRegistry _platformTypesConvertorsRegistry;

        public IProcessInfo Activate(EndpointInfo endpointInfo, ICommand command)
        {
#if DEBUG
            Log($"endpointInfo = {endpointInfo}");
            Log($"command = {command}");
#endif

            throw new NotImplementedException();
        }
    }
}

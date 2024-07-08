using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Serialization.Functors.MethodResponses
{
    public class BaseFunctorWithoutResultMethodResponse: IMethodResponse
    {
        public BaseFunctorWithoutResultMethodResponse(BaseFunctorWithoutResult source)
        {
            _source = source;
        }

        private BaseFunctorWithoutResult _source;
    }
}

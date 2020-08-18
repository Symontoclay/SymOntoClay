using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class EndpointParamAttribute : Attribute
    {
        public EndpointParamAttribute(string name)
        {
        }

        public EndpointParamAttribute(KindOfEndpointParam kindOfParameter)
        {
        }

        public EndpointParamAttribute(string name, KindOfEndpointParam kindOfParameter)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.CoreHostListener
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class PlatformEndpointParameterAttribute : Attribute
    {
        public PlatformEndpointParameterAttribute(string name)
        {
        }

        public PlatformEndpointParameterAttribute(KindOfPlatformEndpointParameter kindOfParameter)
        {
        }

        public PlatformEndpointParameterAttribute(string name, KindOfPlatformEndpointParameter kindOfParameter)
        {
        }
    }
}

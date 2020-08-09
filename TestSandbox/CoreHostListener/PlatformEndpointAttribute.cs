using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.CoreHostListener
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PlatformEndpointAttribute: Attribute
    {
        public PlatformEndpointAttribute()
        {
        }

        public PlatformEndpointAttribute(bool useMainThread)
        {
        }

        public PlatformEndpointAttribute(params int[] devices)
        {
        }

        public PlatformEndpointAttribute(bool useMainThread, params int[] devices)
        {
        }

        public PlatformEndpointAttribute(string methodName)
        {
        }

        public PlatformEndpointAttribute(string methodName, bool useMainThread)
        {
        }

        public PlatformEndpointAttribute(string methodName, params int[] devices)
        {
        }

        public PlatformEndpointAttribute(string methodName, bool useMainThread, params int[] devices)
        {
        }
    }
}

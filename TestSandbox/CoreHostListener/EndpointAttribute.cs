using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.CoreHostListener
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class EndpointAttribute: Attribute
    {
        public EndpointAttribute()
        {
        }

        public EndpointAttribute(bool useMainThread)
        {
        }

        public EndpointAttribute(params int[] devices)
        {
        }

        public EndpointAttribute(bool useMainThread, params int[] devices)
        {
        }

        public EndpointAttribute(string methodName)
        {
        }

        public EndpointAttribute(string methodName, bool useMainThread)
        {
        }

        public EndpointAttribute(string methodName, params int[] devices)
        {
        }

        public EndpointAttribute(string methodName, bool useMainThread, params int[] devices)
        {
        }
    }
}

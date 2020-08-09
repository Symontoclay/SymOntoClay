using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.CoreHostListener
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BipedPlatformEndpointAttribute : Attribute
    {
        public BipedPlatformEndpointAttribute()
        {
        }

        public BipedPlatformEndpointAttribute(bool useMainThread)
        {
        }

        public BipedPlatformEndpointAttribute(params TstDevices[] devices)
        {
        }

        public BipedPlatformEndpointAttribute(bool useMainThread, params TstDevices[] devices)
        {
        }

        public BipedPlatformEndpointAttribute(string methodName)
        {
        }

        public BipedPlatformEndpointAttribute(string methodName, bool useMainThread)
        {
        }

        public BipedPlatformEndpointAttribute(string methodName, params TstDevices[] devices)
        {
        }

        public BipedPlatformEndpointAttribute(string methodName, bool useMainThread, params TstDevices[] devices)
        {
        }
    }
}

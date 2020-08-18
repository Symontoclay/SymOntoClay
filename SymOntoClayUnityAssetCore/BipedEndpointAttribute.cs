using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BipedEndpointAttribute : Attribute
    {
        public BipedEndpointAttribute()
        {
        }

        public BipedEndpointAttribute(bool useMainThread)
        {
        }

        public BipedEndpointAttribute(params BipedDevices[] devices)
        {
        }

        public BipedEndpointAttribute(bool useMainThread, params BipedDevices[] devices)
        {
        }

        public BipedEndpointAttribute(string methodName)
        {
        }

        public BipedEndpointAttribute(string methodName, bool useMainThread)
        {
        }

        public BipedEndpointAttribute(string methodName, params BipedDevices[] devices)
        {
        }

        public BipedEndpointAttribute(string methodName, bool useMainThread, params BipedDevices[] devices)
        {
        }
    }
}

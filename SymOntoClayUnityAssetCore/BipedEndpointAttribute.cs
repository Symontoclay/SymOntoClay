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

        public BipedEndpointAttribute(params DeviceOfBiped[] devices)
        {
        }

        public BipedEndpointAttribute(bool useMainThread, params DeviceOfBiped[] devices)
        {
        }

        public BipedEndpointAttribute(string methodName)
        {
        }

        public BipedEndpointAttribute(string methodName, bool useMainThread)
        {
        }

        public BipedEndpointAttribute(string methodName, params DeviceOfBiped[] devices)
        {
        }

        public BipedEndpointAttribute(string methodName, bool useMainThread, params DeviceOfBiped[] devices)
        {
        }
    }
}

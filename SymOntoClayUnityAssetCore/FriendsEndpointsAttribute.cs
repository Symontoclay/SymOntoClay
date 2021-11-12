using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class FriendsEndpointsAttribute : Attribute
    {
        public FriendsEndpointsAttribute(params string[] friendsEnpoints)
        {
        }
    }
}

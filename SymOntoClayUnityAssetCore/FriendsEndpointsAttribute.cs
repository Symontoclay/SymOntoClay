using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Marks C# method as friends for marked method.
    /// Both friends methods and marked method should be also marked as external methods for SymOntoClay DSL.
    /// Friends methods can be executed as parallel with such NPC devices.
    /// For example, NPC can aim to something and shoot as parallel. 
    /// And these host methods use NPC's hands. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class FriendsEndpointsAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="friendsEnpoints">Names of friends methods.</param>
        public FriendsEndpointsAttribute(params string[] friendsEnpoints)
        {
        }
    }
}

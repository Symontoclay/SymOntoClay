/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Marks C# method as external method for SymOntoClay DSL.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BipedEndpointAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the class with default properties.
        /// </summary>
        public BipedEndpointAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="useMainThread"><b>true</b> if marked method requires calling in the main thread only. <b>false</b> if the marked method can be called in any thread.</param>
        public BipedEndpointAttribute(bool useMainThread)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="devices">Describes parts of body which will be used during execution of the marked method.</param>
        public BipedEndpointAttribute(params DeviceOfBiped[] devices)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="useMainThread"><b>true</b> if marked method requires calling in the main thread only. <b>false</b> if the marked method can be called in any thread.</param>
        /// <param name="devices">Describes parts of body which will be used during execution of the marked method.</param>
        public BipedEndpointAttribute(bool useMainThread, params DeviceOfBiped[] devices)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="methodName">Alias of the marked method in SymOntoClay DSL.</param>
        public BipedEndpointAttribute(string methodName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="useMainThread"><b>true</b> if marked method requires calling in the main thread only. <b>false</b> if the marked method can be called in any thread.</param>
        public BipedEndpointAttribute(string methodName, bool useMainThread)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="methodName">Alias of the marked method in SymOntoClay DSL.</param>
        /// <param name="devices">Describes parts of body which will be used during execution of the marked method.</param>
        public BipedEndpointAttribute(string methodName, params DeviceOfBiped[] devices)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="methodName">Alias of the marked method in SymOntoClay DSL.</param>
        /// <param name="useMainThread"><b>true</b> if marked method requires calling in the main thread only. <b>false</b> if the marked method can be called in any thread.</param>
        /// <param name="devices">Describes parts of body which will be used during execution of the marked method.</param>
        public BipedEndpointAttribute(string methodName, bool useMainThread, params DeviceOfBiped[] devices)
        {
        }
    }
}

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
using System.Linq;
using System.Reflection;
using System.Text;

namespace SymOntoClay.CoreHelper
{
    public static class AppDomainTypesEnumerator
    {
        public static IList<Type> GetTypes()
        {
            var currAppDomain = AppDomain.CurrentDomain;

            var assembliesList = currAppDomain.GetAssemblies();

            var processedAssebbliesList = new List<AssemblyName>();
            var result = new List<Type>();

            foreach (var assembly in assembliesList)
            {
                ProcessAssembly(assembly, result, processedAssebbliesList);
            }

            return result.Distinct().ToList();
        }

        private static void ProcessAssembly(Assembly assembly, List<Type> result, List<AssemblyName> processedAssebbliesList)
        {
            if (IsFilteredByName(assembly.FullName))
            {
                return;
            }

            var assemblyName = assembly.GetName();

            if (processedAssebbliesList.Contains(assemblyName))
            {
                return;
            }

            processedAssebbliesList.Add(assemblyName);

            var typesList = assembly.GetTypes();

            result.AddRange(typesList);

            var referencedAssembliesNamesList = assembly.GetReferencedAssemblies();

            foreach (var referencedAssemblyName in referencedAssembliesNamesList)
            {
                if (IsFilteredByName(referencedAssemblyName.FullName))
                {
                    continue;
                }

                var referencedAssembly = Assembly.Load(referencedAssemblyName);

                ProcessAssembly(referencedAssembly, result, processedAssebbliesList);
            }
        }

        private static bool IsFilteredByName(string name)
        {
            if (name.StartsWith("System."))
            {
                return true;
            }

            if (name.StartsWith("netstandard"))
            {
                return true;
            }

            if (name.StartsWith("NLog"))
            {
                return true;
            }

            if (name.StartsWith("Newtonsoft"))
            {
                return true;
            }

            if (name.StartsWith("Microsoft"))
            {
                return true;
            }

            if (name.StartsWith("nunit"))
            {
                return true;
            }

            return false;
        }
    }
}

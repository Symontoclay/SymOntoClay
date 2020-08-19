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

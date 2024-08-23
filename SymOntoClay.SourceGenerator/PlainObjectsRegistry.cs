using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.SourceGenerator
{
    public class PlainObjectsRegistry
    {
        private Dictionary<string, Dictionary<int, string>> _plainObjectsNamesDict = new Dictionary<string, Dictionary<int, string>>();

        public void Add(string className, int paramsCount, string plainObjectClassName)
        {
            if (_plainObjectsNamesDict.TryGetValue(className, out var dict))
            {
                dict[paramsCount] = plainObjectClassName;
            }
            else
            {
                _plainObjectsNamesDict[className] = new Dictionary<int, string>()
                {
                    {paramsCount, plainObjectClassName}
                };
            }
        }

        public string Get(IEnumerable<string> usingsList, string className, int paramsCount)
        {
            var result = Get(className, paramsCount);

            if (!string.IsNullOrWhiteSpace(result))
            {
                return result;
            }

            foreach (var item in usingsList ?? Enumerable.Empty<string>())
            {
                result = Get($"{item}.{className}", paramsCount);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    return result;
                }
            }

            return string.Empty;
        }

        public string Get(string className, int paramsCount)
        {
            if (_plainObjectsNamesDict.TryGetValue(className, out var dict))
            {
                if (dict.TryGetValue(paramsCount, out var plainObjectClassName))
                {
                    return plainObjectClassName;
                }
            }

            return string.Empty;
        }
    }
}

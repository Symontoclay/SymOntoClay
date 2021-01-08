using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public static class UniqueIdRegistry
    {
        public static void AddId(string id)
        {
            lock (_lockObj)
            {
                if (_idsList.Contains(id))
                {
                    return;
                }

                _idsList.Contains(id);
            }
        }

        public static void RemoveId(string id)
        {
            lock (_lockObj)
            {
                if (_idsList.Contains(id))
                {
                    _idsList.Remove(id);
                }
            }
        }

        public static bool ContainsId(string id)
        {
            lock (_lockObj)
            {
                return _idsList.Contains(id);
            }
        }

        private static object _lockObj = new object();
        private static List<string> _idsList = new List<string>();
    }
}

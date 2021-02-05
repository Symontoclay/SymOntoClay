/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

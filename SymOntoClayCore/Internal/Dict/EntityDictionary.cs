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

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Dict
{
    public class EntityDictionary: BaseComponent, IEntityDictionary
    {
        public EntityDictionary(IEntityLogger logger)
            : base(logger)
        {
        }

        /// <inheritdoc/>
        public string Name => _name;

        /// <inheritdoc/>
        public ulong GetKey(string name)
        {
#if DEBUG
            //Log($"name = {name}");
#endif

            if (string.IsNullOrWhiteSpace(name))
            {
                return 0ul;
            }

            name = name.ToLower().Replace("`", string.Empty).Trim();

            lock (_lockObj)
            {
                if (_caseInsensitiveWordsDict.ContainsKey(name))
                {
                    return _caseInsensitiveWordsDict[name];
                }

                _currIndex++;
                _caseInsensitiveWordsDict[name] = _currIndex;
                _caseInsensitiveBackWordsDict[_currIndex] = name;

                return _currIndex;
            }
        }

        /// <inheritdoc/>
        public string GetName(ulong key)
        {
#if DEBUG
            //Log($"key = {key}");
#endif

            lock (_lockObj)
            {
                if (_caseInsensitiveBackWordsDict.ContainsKey(key))
                {
                    return _caseInsensitiveBackWordsDict[key];
                }
                return string.Empty;
            }
        }

#if DEBUG
        /// <inheritdoc/>
        public string GetDbgStr()
        {
            var spaces = DisplayHelper.Spaces(4u);
            var sb = new StringBuilder();

            sb.AppendLine("Begin Dictionary");
            foreach(var item in _caseInsensitiveBackWordsDict)
            {
                sb.AppendLine($"{spaces}{item.Key}: '{item.Value}'");
            }
            sb.AppendLine("End Dictionary");

            return sb.ToString();
        }
#endif

        public void LoadFromSourceCode()
        {
            //Log("Do");

            _name = Guid.NewGuid().ToString("D");
            _currIndex = 0;
            _caseInsensitiveWordsDict = new Dictionary<string, ulong>();
            _caseInsensitiveBackWordsDict = new Dictionary<ulong, string>();
        }

        public void LoadFromImage(string path)
        {
#if IMAGINE_WORKING
            Log($"path = {path}");
#else
            throw new NotImplementedException();
#endif
        }

        public void SaveToImage(string path)
        {
#if IMAGINE_WORKING
            Log($"path = {path}");
#else
            throw new NotImplementedException();
#endif
        }

        #region Private Members
        private object _lockObj = new object();
        #endregion

        #region Private Data
        private string _name;
        private Dictionary<string, ulong> _caseInsensitiveWordsDict;
        private Dictionary<ulong, string> _caseInsensitiveBackWordsDict;
        private ulong _currIndex;
        #endregion
    }
}

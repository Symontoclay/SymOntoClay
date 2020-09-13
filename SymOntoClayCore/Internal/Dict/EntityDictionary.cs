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

            name = name.ToLower().Trim();

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

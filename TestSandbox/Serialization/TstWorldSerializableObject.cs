using NLog;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Serialization;
using System;
using System.Text;

namespace TestSandbox.Serialization
{
    public class TstWorldSerializableObject : IObjectToString
    {
#if DEBUG
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
#endif

        public TstWorldSerializableObject(TstExternalSettings settings) 
        {
            _internal = new TstInternalWorldSerializableObject(settings);
        }

        public void Save(SerializationSettings settings)
        {
            throw new NotImplementedException("B0AD3C35-AAFD-4E6B-A6E8-665B2E672E03");
        }

        public void SetSomeValue(int value)
        {
            _internal.SetSomeValue(value);
        }

        private TstInternalWorldSerializableObject _internal;

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }
        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }
        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintObjProp(n, nameof(_internal), _internal);
            return sb.ToString();
        }
    }
}

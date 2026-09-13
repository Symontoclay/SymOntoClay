using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.CoreHelper.SerializationToImage;
using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core;
using System.Text;

namespace SymOntoClay.NLP.SerializationData
{
    public class NLPConverterFactorySerializationData : IObjectToString, IClassSerializationData
    {
        [MemberWithExternalValue]
        public INLPConverterProvider Provider { get; set; }

        /// <inheritdoc/>
        IParentInClassSerializationData IClassSerializationData.Parent => Provider;

        public IMonitorLogger Logger { get; set; }

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(Provider), Provider);
            sb.PrintExisting(n, nameof(Logger), Logger);

            return sb.ToString();
        }
    }
}

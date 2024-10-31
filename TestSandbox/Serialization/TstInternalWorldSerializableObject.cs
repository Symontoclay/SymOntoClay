using NLog;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Serialization;
using SymOntoClay.Serialization.SmartValues;
using System.Data.Common;
using System.Runtime;
using System.Text;

namespace TestSandbox.Serialization
{
    public class TstInternalWorldSerializableObject : IObjectToString
    {
#if DEBUG
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
#endif

        public TstInternalWorldSerializableObject(TstExternalSettings settings) 
        {
            _settings = settings;

            _context = new TstEngineContext();

            _context.Prop1 = new ExternalSettingsSmartValue<string>(settings.Prop1, typeof(TstExternalSettings), GetType(), string.Empty);

            _sub = new TstWorldSubSerializableObject(_context);
        }

        [SocSerializableExternalSettings("")]
        private readonly TstExternalSettings _settings;

        private TstEngineContext _context;
        private TstWorldSubSerializableObject _sub;

        public void SetSomeValue(int value)
        {
            _sub.SetSomeValue(value);
        }

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
            sb.PrintObjProp(n, nameof(_context), _context);
            sb.PrintObjProp(n, nameof(_sub), _sub);
            //sb.AppendLine($"{spaces}{nameof()} = {}");
            return sb.ToString();
        }
    }
}

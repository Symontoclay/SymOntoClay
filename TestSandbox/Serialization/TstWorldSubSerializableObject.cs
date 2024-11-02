using NLog;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Serialization.SmartValues;
using System.Data;
using System.Speech.Synthesis;
using System.Text;

namespace TestSandbox.Serialization
{
    public class TstWorldSubSerializableObject : IObjectToString
    {
#if DEBUG
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
#endif

        public TstWorldSubSerializableObject()
        {
        }

        public TstWorldSubSerializableObject(TstEngineContext context) 
        {
            _context = context;
            Prop1 = context.Prop1;
        }

        private TstEngineContext _context;
        private SmartValue<string> Prop1 { get; set; }
        private int SomeValueProp { get; set; }

        public void SetSomeValue(int value)
        {
            SomeValueProp = value;
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
            sb.PrintObjProp(n, nameof(Prop1), Prop1);
            //sb.AppendLine($"{spaces}{nameof(Prop1)} = {Prop1}");
            sb.AppendLine($"{spaces}{nameof(SomeValueProp)} = {SomeValueProp}");
            sb.PrintObjProp(n, nameof(_context), _context);
            //sb.AppendLine($"{spaces}{nameof()} = {}");
            return sb.ToString();
        }
    }
}

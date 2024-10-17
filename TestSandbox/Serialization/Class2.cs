using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Serialization;
using SymOntoClay.Serialization.Settings;
using SymOntoClay.Threading;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;

namespace TestSandbox.Serialization
{
    public class Class2: IObjectToString
    {
        public Class2()
        {
        }

        public Class2(bool value)
        {
            _noneCancelationToken = CancellationToken.None;

            var minThreadsCount = 1;
            var maxThreadsCount = 5;

            _customThreadPoolSerializationSettings = new CustomThreadPoolSerializationSettings()
            {
                MaxThreadsCount = maxThreadsCount,
                MinThreadsCount = minThreadsCount,
                CancellationToken = _noneCancelationToken
            };

            _threadPool = new FakeCustomThreadPool(minThreadsCount, maxThreadsCount, _noneCancelationToken);
        }

        private CancellationToken _noneCancelationToken;

        private CustomThreadPoolSerializationSettings _customThreadPoolSerializationSettings;

        [SocObjectSerializationSettings(nameof(_customThreadPoolSerializationSettings))]
        private IFakeCustomThreadPool _threadPool;

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
            sb.PrintObjProp(n, nameof(_customThreadPoolSerializationSettings), _customThreadPoolSerializationSettings);
            //sb.AppendLine($"{spaces}{nameof(Prop1)} = {Prop1}");
            //sb.AppendLine($"{spaces}{nameof(Prop2)} = {Prop2}");
            //sb.AppendLine($"{spaces}{nameof(Prop3)} = {Prop3}");
            //sb.AppendLine($"{spaces}{nameof(Prop4)} = {Prop4}");
            //sb.AppendLine($"{spaces}{nameof(Field1)} = {Field1}");
            //sb.AppendLine($"{spaces}{nameof(Field2)} = {Field2}");
            //sb.AppendLine($"{spaces}{nameof(Field3)} = {Field3}");
            //sb.AppendLine($"{spaces}{nameof(Field4)} = {Field4}");
            //sb.AppendLine($"{spaces}{nameof(Field5)} = {Field5}");
            return sb.ToString();
        }
    }
}

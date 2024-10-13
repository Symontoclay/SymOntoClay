using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Common;
using System.Text;
using SymOntoClay.Serialization;

namespace TestSandbox.Serialization
{
    public class ParentClass1
    {
        public int PropP1 { get; set; }

        private int FieldP1;
    }

    public class Class1 : ParentClass1, IObjectToString
    {
        public static int SProp1 { get; set; }

        [SocNoSerializable]
        public int Prop1 { get; set; }
        public int Prop2 { get; set; } = 12;
        public int Prop3 => Field2;
        public int Prop4
        {
            get
            {
                return Field1;
            }

            set
            {
                if (Field1 == value)
                {
                    return;
                }

                Field1 = value;
            }
        }

        [SocNoSerializable]
        private int Field1;
        private int Field2 = 16;
        private readonly int Field3;

        protected int Field4;

        public int Field5;

        private static int SField5;

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
            sb.AppendLine($"{spaces}{nameof(Prop1)} = {Prop1}");
            sb.AppendLine($"{spaces}{nameof(Prop2)} = {Prop2}");
            sb.AppendLine($"{spaces}{nameof(Prop3)} = {Prop3}");
            sb.AppendLine($"{spaces}{nameof(Prop4)} = {Prop4}");
            sb.AppendLine($"{spaces}{nameof(Field1)} = {Field1}");
            sb.AppendLine($"{spaces}{nameof(Field2)} = {Field2}");
            sb.AppendLine($"{spaces}{nameof(Field3)} = {Field3}");
            sb.AppendLine($"{spaces}{nameof(Field4)} = {Field4}");
            sb.AppendLine($"{spaces}{nameof(Field5)} = {Field5}");
            return sb.ToString();
        }
    }
}

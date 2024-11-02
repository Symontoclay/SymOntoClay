using Newtonsoft.Json.Linq;
using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Serialization.SmartValues
{
    public class ConstSmartValue<T>: SmartValue<T>
    {
        public ConstSmartValue() 
        { 
        }

        public ConstSmartValue(T value)
        { 
            _value = value; 
        }

        private T _value;

        /// <inheritdoc/>
        public override T Value => _value;

        /// <inheritdoc/>
        public override void SetValue(T value)
        {
            _value = value;
        }
    }
}

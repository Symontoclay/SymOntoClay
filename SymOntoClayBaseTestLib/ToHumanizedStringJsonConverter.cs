using Newtonsoft.Json;
using SymOntoClay.Core.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib
{
    public class ToHumanizedStringJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterface("IObjectToHumanizedString") != null;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var val = (IObjectToHumanizedString)value;

            writer.WriteValue(val.ToHumanizedString());
        }
    }
}

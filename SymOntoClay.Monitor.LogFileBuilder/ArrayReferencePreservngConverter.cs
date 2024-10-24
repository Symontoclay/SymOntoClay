/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    //https://stackoverflow.com/questions/41293407/cannot-preserve-reference-to-array-or-readonly-list-or-list-created-from-a-non
    public class ArrayReferencePreservngConverter : JsonConverter
    {
        const string refProperty = "$ref";
        const string idProperty = "$id";
        const string valuesProperty = "$values";

        public override bool CanConvert(Type objectType)
        {
            // byte [] is serialized as a Base64 string so incorporate fix by https://stackoverflow.com/users/13109224/dalecooper from https://stackoverflow.com/a/66177664
            if (objectType == typeof(byte[]))
                return false; // He would kill a Byte[] and you'll wonder, why the JSON Deserializer will return NULL on Byte[] :-)
                              // Not implemented for multidimensional arrays.
            return objectType.IsArray && objectType.GetArrayRank() == 1;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Debug.Assert(CanConvert(objectType));
            if (reader.TokenType == JsonToken.Null)
                return null;
            else if (reader.TokenType == JsonToken.StartArray)
            {
                // No $ref.  Deserialize as a List<T> to avoid infinite recursion and return as an array.
                var elementType = objectType.GetElementType();
                var listType = typeof(List<>).MakeGenericType(elementType);
                var list = (IList)serializer.Deserialize(reader, listType);
                if (list == null)
                    return null;
                var array = Array.CreateInstance(elementType, list.Count);
                list.CopyTo(array, 0);
                return array;
            }
            else
            {
                var obj = JObject.Load(reader);
                var refId = (string)obj[refProperty];
                if (refId != null)
                {
                    var reference = serializer.ReferenceResolver.ResolveReference(serializer, refId);
                    if (reference != null)
                        return reference;
                }
                var values = obj[valuesProperty];
                if (values == null || values.Type == JTokenType.Null)
                    return null;
                if (!(values is JArray))
                {
                    throw new JsonSerializationException(string.Format("{0} was not an array", values));
                }
                var count = ((JArray)values).Count;

                var elementType = objectType.GetElementType();
                var array = Array.CreateInstance(elementType, count);

                var objId = (string)obj[idProperty];
                if (objId != null)
                {
                    // Add the empty array into the reference table BEFORE poppulating it,
                    // to handle recursive references.
                    serializer.ReferenceResolver.AddReference(serializer, objId, array);
                }

                var listType = typeof(List<>).MakeGenericType(elementType);
                using (var subReader = values.CreateReader())
                {
                    var list = (IList)serializer.Deserialize(subReader, listType);
                    list.CopyTo(array, 0);
                }

                return array;
            }
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    [SerializeWithDataCreation]
    public class SerializeWithDataCreationStub
    {
        public static SerializeWithDataCreationStub Instance { get; set; } = new SerializeWithDataCreationStub();

        private SerializeWithDataCreationStub()
        {
        }
    }
}

using SymOntoClay.CoreHelper.SerializationToImage.Attributes;

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

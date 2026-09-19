using SymOntoClay.CoreHelper.SerializationToImage.Attributes;

namespace SymOntoClay.CoreHelper.SerializationToImage.Stubs
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

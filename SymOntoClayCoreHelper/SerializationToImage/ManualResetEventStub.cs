namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class ManualResetEventStub
    {
        public static ManualResetEventStub Instance { get; set; } = new ManualResetEventStub();

        private ManualResetEventStub()
        {
        }
    }
}

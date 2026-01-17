namespace TestSandbox.MessagePacking
{
    public interface IMessageSerializer
    {
        byte[] Serialize<T>(T obj);
    }
}

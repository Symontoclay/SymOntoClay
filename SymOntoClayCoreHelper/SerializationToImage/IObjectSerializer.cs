using System.Reflection;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IObjectSerializer
    {
        SerializedValue SerializeValue(object obj, string path = "", SerializeValueMode serializeValueMode = SerializeValueMode.General, ObjMemberRef objMember = null);
    }
}

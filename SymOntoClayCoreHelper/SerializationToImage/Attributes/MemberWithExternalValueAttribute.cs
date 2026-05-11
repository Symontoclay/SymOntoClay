using System;

namespace SymOntoClay.CoreHelper.SerializationToImage.Attributes
{
    public class MemberWithExternalValueAttribute: Attribute
    {
        public KindOfStructuralContext KindOfStructuralContext { get; set; } = KindOfStructuralContext.All;
    }
}

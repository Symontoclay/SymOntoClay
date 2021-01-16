using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface ISerializableWithImageEngine
    {
        void LoadFromImage(string path);
        void SaveToImage(string path);
    }
}

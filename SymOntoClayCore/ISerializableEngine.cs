using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface ISerializableEngine
    {
        void LoadFromSourceCode();
        void LoadFromImage(string path);
        void SaveToImage(string path);
    }
}

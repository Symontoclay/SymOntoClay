using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface IMetadataStorage : ISpecificStorage
    {
        void Append(CodeEntity codeEntity);
        CodeEntity MainCodeEntity { get; }
    }
}

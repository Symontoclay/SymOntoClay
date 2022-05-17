using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public interface IPackedRelationsResolver
    {
        RelationDescription GetRelation(StrongIdentifierValue name, int paramsCount);
    }
}

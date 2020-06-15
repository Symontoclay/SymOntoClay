using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking
{
    public interface IAstNode : IObjectToString
    {
        IAstNode Left { get; set; }
        IAstNode Right { get; set; }
    }
}

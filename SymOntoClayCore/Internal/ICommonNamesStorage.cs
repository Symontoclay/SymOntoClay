﻿using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface ICommonNamesStorage
    {
        Name ApplicationName { get; }
        Name ClassName { get; }
        Name DefaultHolder { get; }
    }
}

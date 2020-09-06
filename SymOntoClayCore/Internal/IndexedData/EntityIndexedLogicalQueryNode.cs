﻿using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class EntityIndexedLogicalQueryNode : BaseKeyRefIndexedLogicalQueryNode
    {
        /// <inheritdoc/>
        public override KindOfLogicalQueryNode Kind => KindOfLogicalQueryNode.Entity;
    }
}
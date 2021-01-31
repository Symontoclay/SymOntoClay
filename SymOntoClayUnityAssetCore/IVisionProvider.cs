﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IVisionProvider
    {
        IList<VisibleItem> GetCurrentVisibleItems();
    }
}
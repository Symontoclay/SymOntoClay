﻿using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public class Settings: IObjectToString
    {
        public LoggingSettings Logging { get; set; }

        /// <inheritdoc/>
        public string PropertiesToString(uint n)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            throw new NotImplementedException();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.Threads
{
    public interface IActivePeriodicObject
    {
        PeriodicDelegate PeriodicMethod { get; set; }
        void Start();
    }
}

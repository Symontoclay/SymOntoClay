﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IHostListener
    {
        ICommandInfo GetCommandInfo(ICommand command);
        ICommandCallResult CallCommand(ICommand command);
    }
}

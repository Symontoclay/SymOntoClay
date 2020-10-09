/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core
{
    public delegate void ProcessInfoEvent(IProcessInfo sender);

    public interface IProcessInfo : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        string Id { get; }
        ProcessStatus Status { get; set; }
        bool IsFinished { get; }
        IReadOnlyList<int> Devices { get; }
        void Start();
        void Cancel();
        event ProcessInfoEvent OnFinish;
        float Priority { get; }
        float GlobalPriority { get; }
        IProcessInfo ParentProcessInfo { get; set; }
        IReadOnlyList<IProcessInfo> GetChildrenProcessInfoList { get; }
        void AddChild(IProcessInfo processInfo);
        void RemoveChild(IProcessInfo processInfo);
    }
}

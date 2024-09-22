/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Common;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public interface IProcessInfo : IDisposable, IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToHumanizedString, IMonitoredHumanizedObject
    {
        string Id { get; }
        string EndPointName { get; }
        
        ProcessStatus Status { get; }

        void SetStatus(IMonitorLogger logger, string messagePointId, ProcessStatus status);

        bool IsFinished(IMonitorLogger logger);
        IReadOnlyList<int> Devices { get; }
        void Start(IMonitorLogger logger, string messagePointId);
        void Cancel(IMonitorLogger logger, string messagePointId, ReasonOfChangeStatus reasonOfChangeStatus, Changer changer = null, string callMethodId = "");
        void Cancel(IMonitorLogger logger, string messagePointId, ReasonOfChangeStatus reasonOfChangeStatus, List<Changer> changers, string callMethodId = "");
        void WeakCancel(IMonitorLogger logger, string messagePointId, ReasonOfChangeStatus reasonOfChangeStatus, Changer changer = null, string callMethodId = "");
        void WeakCancel(IMonitorLogger logger, string messagePointId, ReasonOfChangeStatus reasonOfChangeStatus, List<Changer> changers, string callMethodId = "");

        void AddOnFinishHandler(IOnFinishProcessInfoHandler handler);
        void RemoveOnFinishHandler(IOnFinishProcessInfoHandler handler);

        void AddOnCompleteHandler(IOnCompleteProcessInfoHandler handler);
        void RemoveOnCompleteHandler(IOnCompleteProcessInfoHandler handler);

        void AddOnWeakCanceledHandler(IOnWeakCanceledProcessInfoHandler handler);
        void RemoveOnWeakCanceledHandler(IOnWeakCanceledProcessInfoHandler handler);

        float Priority { get; }
        IReadOnlyList<string> Friends { get; }
        bool IsFriend(IMonitorLogger logger, IProcessInfo other);
        IProcessInfo ParentProcessInfo { get; set; }
        IReadOnlyList<IProcessInfo> GetChildrenProcessInfoList { get; }
        void AddChild(IMonitorLogger logger, IProcessInfo processInfo);
        void RemoveChild(IMonitorLogger logger, IProcessInfo processInfo);
        void AddOnFinishHandler(IMonitorLogger logger, IProcessInfoEventHandler handler);
        void RemoveOnFinishHandler(IMonitorLogger logger, IProcessInfoEventHandler handler);
        void AddOnCompleteHandler(IMonitorLogger logger, IProcessInfoEventHandler handler);
        void RemoveOnCompleteHandler(IMonitorLogger logger, IProcessInfoEventHandler handler);
        void AddOnWeakCanceledHandler(IMonitorLogger logger, IProcessInfoEventHandler handler);
        void RemoveOnWeakCanceledHandler(IMonitorLogger logger, IProcessInfoEventHandler handler);
        List<MonitoredHumanizedLabel> ToChainOfProcessInfoLabels(IMonitorLogger logger);
        void CollectChainOfProcessInfoLabels(IMonitorLogger logger, IList<MonitoredHumanizedLabel> result, IList<IProcessInfo> usedProcessInfo);
    }
}

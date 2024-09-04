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

using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Common.Disposing;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IWorldComponent : ISymOntoClayDisposable, IExecutorInMainThread
    {
        string Id { get; }
        string IdForFacts { get; }
        int InstanceId { get; }
        IMonitorLogger Logger { get; }

        [Obsolete("Serialization Refactoring", true)]
        string OldInsertPublicFact(IMonitorLogger logger, string text);

        IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text);

        [Obsolete("Serialization Refactoring", true)]
        string OldInsertPublicFact(IMonitorLogger logger, RuleInstance fact);

        IMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact);

        [Obsolete("Serialization Refactoring", true)]
        void OldRemovePublicFact(IMonitorLogger logger, string id);

        IMethodResponse RemovePublicFact(IMonitorLogger logger, string id);

        [Obsolete("Serialization Refactoring", true)]
        void OldPushSoundFact(float power, string text);

        IMethodResponse PushSoundFact(float power, string text);

        [Obsolete("Serialization Refactoring", true)]
        void OldPushSoundFact(float power, RuleInstance fact);

        IMethodResponse PushSoundFact(float power, RuleInstance fact);

        IStandardFactsBuilder StandardFactsBuilder { get; }

        [Obsolete("Serialization Refactoring", true)]
        void OldAddCategory(IMonitorLogger logger, string category);

        IMethodResponse AddCategory(IMonitorLogger logger, string category);

        [Obsolete("Serialization Refactoring", true)]
        void OldAddCategories(IMonitorLogger logger, List<string> categories);

        IMethodResponse AddCategories(IMonitorLogger logger, List<string> categories);

        [Obsolete("Serialization Refactoring", true)]
        void OldRemoveCategory(IMonitorLogger logger, string category);

        IMethodResponse RemoveCategory(IMonitorLogger logger, string category);

        [Obsolete("Serialization Refactoring", true)]
        void OldRemoveCategories(IMonitorLogger logger, List<string> categories);

        IMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories);

        bool EnableCategories { get; set; }
    }
}

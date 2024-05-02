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

using SymOntoClay.Common.Disposing;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IWorldComponent : ISymOntoClayDisposable, IExecutorInMainThread
    {
        string Id { get; }
        string IdForFacts { get; }
        int InstanceId { get; }
        IMonitorLogger Logger { get; }
        string InsertPublicFact(IMonitorLogger logger, string text);
        string InsertPublicFact(IMonitorLogger logger, RuleInstance fact);
        void RemovePublicFact(IMonitorLogger logger, string id);
        void PushSoundFact(float power, string text);
        void PushSoundFact(float power, RuleInstance fact);
        IStandardFactsBuilder StandardFactsBuilder { get; }
        void AddCategory(IMonitorLogger logger, string category);
        void AddCategories(IMonitorLogger logger, List<string> categories);
        void RemoveCategory(IMonitorLogger logger, string category);
        void RemoveCategories(IMonitorLogger logger, List<string> categories);
        bool EnableCategories { get; set; }
    }
}

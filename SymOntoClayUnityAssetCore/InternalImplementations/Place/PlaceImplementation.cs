/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.Place
{
    /// <inheritdoc/>
    public class PlaceImplementation: IPlace, IDeferredInitialized
    {
        public PlaceImplementation(PlaceSettings settings, IWorldCoreGameComponentContext context)
        {

        }

        public PlaceImplementation(PlaceSettings settings)
        {

        }

        void IDeferredInitialized.Initialize(IWorldCoreGameComponentContext worldContext)
        {

        }

        /// <inheritdoc/>
        public IEntityLogger Logger => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsDisposed => throw new NotImplementedException();

        /// <inheritdoc/>
        public void RunInMainThread(Action function)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string InsertPublicFact(string text)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string InsertPublicFact(RuleInstance fact)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemovePublicFact(string id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void PushSoundFact(float power, string text)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void PushSoundFact(float power, RuleInstance fact)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IStandardFactsBuilder StandardFactsBuilder => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

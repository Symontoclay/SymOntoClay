/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

namespace SymOntoClay.Core.Internal.Storage.Factories
{
    public class StorageFactories: BaseContextComponent, IStorageFactories
    {
        public StorageFactories(IEngineContext context)
            : base(context.Logger)
        {
        }

        private readonly IStorageFactory _appInstanceStorageFactory = new AppInstanceStorageFactory();
        private readonly IStorageFactory _objectStorageFactory = new ObjectStorageFactory();
        private readonly IStorageFactory _stateStorageFactory = new StateStorageFactory();
        private readonly IStorageFactory _actionStorageFactory = new ActionStorageFactory();
        private readonly IStorageFactory _rootTaskInstanceStorageFactory = new RootTaskInstanceStorageFactory();
        private readonly IStorageFactory _strategicTaskInstanceStorageFactory = new StrategicTaskInstanceStorageFactory();
        private readonly IStorageFactory _tacticalTaskInstanceStorageFactory = new TacticalTaskInstanceStorageFactory();
        private readonly IStorageFactory _compoundTaskInstanceStorageFactory = new CompoundTaskInstanceStorageFactory();

        /// <inheritdoc/>
        public IStorageFactory AppInstanceStorageFactory => _appInstanceStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory ObjectStorageFactory => _objectStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory StateStorageFactory => _stateStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory ActionStorageFactory => _actionStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory RootTaskInstanceStorageFactory => _rootTaskInstanceStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory StrategicTaskInstanceStorageFactory => _strategicTaskInstanceStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory TacticalTaskInstanceStorageFactory => _tacticalTaskInstanceStorageFactory;

        /// <inheritdoc/>
        public IStorageFactory CompoundTaskInstanceStorageFactory => _compoundTaskInstanceStorageFactory;
    }
}

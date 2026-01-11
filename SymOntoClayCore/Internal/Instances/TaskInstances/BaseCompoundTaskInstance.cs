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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Instances.TaskInstances
{
    public abstract class BaseCompoundTaskInstance: BaseInstance
    {
        protected BaseCompoundTaskInstance(BaseCompoundHtnTask baseCompoundHtnTask, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext, IExecutionCoordinator parentExecutionCoordinator, IStorageFactory storageFactory)
            : base(baseCompoundHtnTask, context, parentStorage, parentCodeExecutionContext, parentExecutionCoordinator, storageFactory, null)
        {
            _baseCompoundHtnTask = baseCompoundHtnTask;
        }

        private readonly BaseCompoundHtnTask _baseCompoundHtnTask;
        private readonly List<CompoundHtnTaskBackgroundTriggerInstance> _compoundHtnTaskBackgroundTriggerInstancesList = new List<CompoundHtnTaskBackgroundTriggerInstance>();

        /// <inheritdoc/>
        protected override void CreateConditionalTriggers(IMonitorLogger logger)
        {
            base.CreateConditionalTriggers(logger);

#if DEBUG
            //Info("AF9E476B-3E6B-4BAF-ACCE-9A7F9523A6AB", $"BaseCompoundTaskInstance CreateConditionalTriggers");
#endif

            var backgrounds = _baseCompoundHtnTask.Backgrounds;

            if (!backgrounds.IsNullOrEmpty())
            {
                foreach(var targetBackground in backgrounds)
                {
                    var backgroundInstance = new CompoundHtnTaskBackgroundTriggerInstance(background: targetBackground, parent: this, context: _context, parentStorage: _storage, parentCodeExecutionContext: _localCodeExecutionContext);
                    _compoundHtnTaskBackgroundTriggerInstancesList.Add(backgroundInstance);

                    backgroundInstance.Init(logger);
                }

                //throw new NotImplementedException("53FF3664-A55F-41CC-880F-804F41FEC037");
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
#if DEBUG
            //Info("129AC325-98AB-40F8-A65D-D85084488B67", $"BaseCompoundTaskInstance OnDisposed");
#endif

            foreach(var backgroundInstance in _compoundHtnTaskBackgroundTriggerInstancesList)
            {
                backgroundInstance.Dispose();
            }

            _compoundHtnTaskBackgroundTriggerInstancesList.Clear();

            base.OnDisposed();
        }
    }
}

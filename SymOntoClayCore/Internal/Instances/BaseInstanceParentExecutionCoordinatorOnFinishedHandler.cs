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

using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;

namespace SymOntoClay.Core.Internal.Instances
{
    public class BaseInstanceParentExecutionCoordinatorOnFinishedHandler : IOnFinishedExecutionCoordinatorHandler,
        IBaseInstanceParentExecutionCoordinatorOnFinishedHandlerSerializedEventsHandler
    {
        public BaseInstanceParentExecutionCoordinatorOnFinishedHandler(IMonitorLogger logger, IExecutionCoordinator executionCoordinator,
            IActiveObjectContext activeObjectContext, ICustomThreadPool threadPool, SerializationAnchor serializationAnchor)
        {
            _logger = logger;
            _executionCoordinator = executionCoordinator;
            _activeObjectContext = activeObjectContext;
            _threadPool = threadPool;
            _serializationAnchor = serializationAnchor;
        }

        private IMonitorLogger _logger;
        private IExecutionCoordinator _executionCoordinator;
        private IActiveObjectContext _activeObjectContext;
        private ICustomThreadPool _threadPool;
        private SerializationAnchor _serializationAnchor;

        void IOnFinishedExecutionCoordinatorHandler.Invoke()
        {
            LoggedFunctorWithoutResult<IBaseInstanceParentExecutionCoordinatorOnFinishedHandlerSerializedEventsHandler>.Run(_logger, "008333AA-D2FE-4DF7-8874-E0325A0767EA", this,
                (IMonitorLogger loggerValue, IBaseInstanceParentExecutionCoordinatorOnFinishedHandlerSerializedEventsHandler instanceValue) => {
                    instanceValue.NParentExecutionCoordinator_OnFinished();
                },
                _activeObjectContext, _threadPool, _serializationAnchor);
        }

        void IBaseInstanceParentExecutionCoordinatorOnFinishedHandlerSerializedEventsHandler.NParentExecutionCoordinator_OnFinished()
        {
            _executionCoordinator.SetExecutionStatus(_logger, "898C6E87-54F5-41CA-8B3F-08B8BBF95135", ActionExecutionStatus.WeakCanceled);
        }
    }
}

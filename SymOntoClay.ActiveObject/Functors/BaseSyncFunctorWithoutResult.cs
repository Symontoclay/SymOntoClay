using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.Functors
{
    public partial class BaseSyncFunctorWithoutResult : IBaseFunctor
    {
        public BaseSyncFunctorWithoutResult(IMonitorLogger logger, string functorId, Action action, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);

            _functorId = functorId;

            _action = action;
            _logger = logger;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private ISerializationAnchor _serializationAnchor;

        private readonly Action _action;

        private IMonitorLogger _logger;

        private bool _isFinished;

        public void Run();
    }
}

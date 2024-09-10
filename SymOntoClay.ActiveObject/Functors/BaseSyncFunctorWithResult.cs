using SymOntoClay.Monitor.Common;
using SymOntoClay.Serialization;
using System;
using static System.Collections.Specialized.BitVector32;

namespace SymOntoClay.ActiveObject.Functors
{
    public partial class BaseSyncFunctorWithResult<TResult> : IBaseFunctor
    {
        public BaseSyncFunctorWithResult(IMonitorLogger logger, string functorId, Func<TResult> func, ISerializationAnchor serializationAnchor)
        {
            _serializationAnchor = serializationAnchor;
            serializationAnchor.AddFunctor(this);
            _functorId = functorId;

            _func = func;
            _logger = logger;
        }

        [SocSerializableActionKey]
        private string _functorId;

        private ISerializationAnchor _serializationAnchor;

        private readonly Func<TResult> _func;

        private IMonitorLogger _logger;

        private bool _isFinished;

        private TResult _result;

        public void Run();
    }
}

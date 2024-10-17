using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Serialization;
using System;

namespace TestSandbox.TpmActions
{
    [SocSerializableAction(Id = "2FFB6DA6-3048-4310-9BCC-9944BC0A87F6")]
    public class SocSerializableActionFactory2FFB6DA6304843109BCC9944BC0A87F6 : ISocSerializableActionFactory
    {
        public string Id => "2FFB6DA6-3048-4310-9BCC-9944BC0A87F6";

        private Action<NLog.ILogger> _action = (NLog.ILogger logger) => {
                logger.Info("Hi");
            };

        private Func<int> _func = int () => {
            return 16;
        };

        public object GetAction(int index)
        {
            switch(index)
            {
                case 0: return _action;
                case 1: return _func;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }
    }
}

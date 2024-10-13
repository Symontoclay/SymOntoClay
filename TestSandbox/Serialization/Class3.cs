using SymOntoClay.Serialization;
using System;

namespace TestSandbox.Serialization
{
    public class Class3
    {
        public Class3()
        {
        }

        public Class3(bool value)
        {
            _action = (NLog.ILogger logger) => {
                logger.Info("Hi");
            };
        }

        [SocSerializableActionKey]
        private string _functorId = "2FFB6DA6-3048-4310-9BCC-9944BC0A87F6";

        private Action<NLog.ILogger> _action;
        private Func<int> _func;
    }
}

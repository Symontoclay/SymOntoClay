using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal
{
    public class MessageNumberGenerator
    {
        private ulong _messageNumber;
        private object _messageNumberLockObj = new object();

        public ulong GetMessageNumber()
        {
            lock (_messageNumberLockObj)
            {
                _messageNumber++;
                return _messageNumber;
            }
        }
    }
}

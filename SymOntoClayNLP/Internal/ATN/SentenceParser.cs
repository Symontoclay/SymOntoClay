using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ATN
{
    public class SentenceParser: BaseParser
    {
        public enum State
        {
            Init
        }

        public override void SetStateAsInt32(int state)
        {
            _state = (State)state;
        }

        private State _state;
    }
}

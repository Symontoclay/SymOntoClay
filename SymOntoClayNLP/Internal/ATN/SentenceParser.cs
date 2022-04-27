using SymOntoClay.NLP.CommonDict;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public override void OnRun(ATNToken token)
        {
#if DEBUG
            _logger.Log($"_state = {_state}");
            _logger.Log($"token = {token}");
#endif

            switch (_state)
            {
                case State.Init:
                    {
                        throw new NotImplementedException();
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}

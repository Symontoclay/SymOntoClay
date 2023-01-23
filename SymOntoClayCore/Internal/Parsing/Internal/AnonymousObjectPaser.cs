using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class AnonymousObjectPaser : BaseObjectParser
    {
        private enum State
        {
            Init,
            ContentStarted
        }

        public AnonymousObjectPaser(InternalParserContext context)
            : base(context, KindOfCodeEntity.AnonymousObject)
        {
        }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            base.OnFinish();

            Result.IsAnonymous = true;
            Result.Name = NameHelper.CreateEntityName();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"_state = {_state}");
            //Log($"Result = {Result}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state= State.ContentStarted;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.ContentStarted:
                    ProcessGeneralContent();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}

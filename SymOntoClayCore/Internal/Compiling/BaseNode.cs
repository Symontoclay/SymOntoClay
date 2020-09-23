using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling
{
    public abstract class BaseNode: BaseLoggedComponent
    {
        protected BaseNode(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        protected readonly IMainStorageContext _context;

        public List<ScriptCommand> Result => _result;

        private readonly List<ScriptCommand> _result = new List<ScriptCommand>();

        protected void AddCommand(ScriptCommand command)
        {
            _result.Add(command);
        }

        protected void AddCommands(List<ScriptCommand> commands)
        {
            _result.AddRange(commands);
        }

        protected void CompileValue(Value value)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            if(value.IsStrongIdentifierValue)
            {
                var name = value.AsStrongIdentifierValue;

                var kindOfName = name.KindOfName;

                switch(kindOfName)
                {
                    case KindOfName.Concept:
                    case KindOfName.Channel:
                    case KindOfName.Entity:
                        CompilePushVal(value);
                        break;

                    case KindOfName.SystemVar:
                    case KindOfName.Var:
                        CompilePushValFromVar(value);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
                }

                return;
            }

            var command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = value.GetIndexedValue(_context);

            AddCommand(command);
        }

        protected void CompilePushVal(Value value)
        {
            var command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = value.GetIndexedValue(_context);

            AddCommand(command);
        }

        protected void CompilePushValFromVar(Value value)
        {
            var command = new ScriptCommand();
            command.OperationCode = OperationCode.PushValFromVar;
            command.Value = value.GetIndexedValue(_context);

            AddCommand(command);
        }

        protected void CompilePushAnnotation(AnnotatedItem annotatedItem)
        {
            var command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = annotatedItem.GetAnnotationValue().GetIndexedValue(_context);

            AddCommand(command);
        }
    }
}

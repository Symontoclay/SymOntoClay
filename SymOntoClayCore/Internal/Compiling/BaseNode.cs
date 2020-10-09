/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

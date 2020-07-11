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
    }
}

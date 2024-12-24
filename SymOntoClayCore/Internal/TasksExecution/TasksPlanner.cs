using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.TasksExecution
{
    public class TasksPlanner: BaseComponent
    {
        public TasksPlanner(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;

        public TasksPlan BuildPlan()
        {
            throw new NotImplementedException("FF8CD857-079E-49A1-8C06-32D475C38D56");
        }
    }
}

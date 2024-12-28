using SymOntoClay.ActiveObject.Threads;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.TasksExecution
{
    public class TasksPlanRunner: BaseComponent
    {
        public TasksPlanRunner(IEngineContext context, IActivePeriodicObject activeObject)
            : base(context.Logger)
        {
            _context = context;

            _activeObject = activeObject;
            activeObject.PeriodicMethod = CommandLoop;
        }

        private readonly IEngineContext _context;
        private readonly IActivePeriodicObject _activeObject;

        public void Run(TasksPlan plan)
        {
#if DEBUG
            Info("2AC6C566-EA83-476B-92E6-8F955F0B0935", $"plan = {plan.ToDbgString()}");
#endif



            throw new NotImplementedException("03EEE01B-1763-49D4-AC9B-2A071C887248");
        }

        private bool CommandLoop(CancellationToken cancellationToken)
        {
#if DEBUG
            Info("BB2C24F5-B3FA-4394-BC6F-7BD5EF4010B7", "Begin");
#endif

            try
            {
                return true;
            }
            catch (Exception e)
            {
                Error("C4FD1E93-98B8-46C1-8FB5-89B7FB675709", e);

                throw;
            }
        }
    }
}

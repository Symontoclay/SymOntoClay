using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.Core.Internal.Instances
{
    public class CompoundHtnTaskBackgroundTriggerInstance : BaseConditionalTriggerInstance
    {
        public CompoundHtnTaskBackgroundTriggerInstance(CompoundHtnTaskBackground background, BaseInstance parent, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext)
            : base(parent, context, parentStorage, parentCodeExecutionContext)
        {
        }

        protected override void DoSearch(IMonitorLogger logger)
        {
#if DEBUG
            Info("A113FF1E-EF91-480A-BED9-A888FC27CC10", "Run DoSearch");
#endif
        }
    }
}

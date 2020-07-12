using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Serialization
{
    public class ActiveLoaderFromSourceCode: BaseLoaderFromSourceCode
    {
        public ActiveLoaderFromSourceCode(IEngineContext context)
            : base(context)
        {
            _context = context;
        }

        private readonly IEngineContext _context;

        /// <inheritdoc/>
        public override void LoadFromSourceFiles()
        {
#if DEBUG
            //Log("Begin");
#endif

            base.LoadFromSourceFiles();

#if DEBUG
            //Log("Next --");
#endif

            var instancesStorage = _context.InstancesStorage;

            instancesStorage.ActivateMainEntity();

#if DEBUG
            //Log("End");
#endif
        }
    }
}

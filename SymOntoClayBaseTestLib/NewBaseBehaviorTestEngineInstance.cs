using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib
{
    public abstract class NewBaseBehaviorTestEngineInstance : INewBehaviorTestEngineInstance
    {
        protected NewBaseBehaviorTestEngineInstance(string fileContent,
            string rootDir,
            KindOfUsingStandardLibrary useStandardLibrary)
        {
            _internalInstance = new AdvancedBehaviorTestEngineInstance(rootDir, useStandardLibrary);
            _internalInstance.WriteFile(fileContent);
        }

        protected AdvancedBehaviorTestEngineInstance _internalInstance;

        /// <inheritdoc/>
        public abstract bool Run();

        private bool _isDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            _internalInstance.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.BaseTestLib
{
    public abstract class BaseBehaviorTestEngineInstance : IBehaviorTestEngineInstance
    {
        protected BaseBehaviorTestEngineInstance(string fileContent,
            string rootDir,
            KindOfUsingStandardLibrary useStandardLibrary,
            bool enableNLP,
            bool enableCategories,
            List<string> categories)
        {
            AdvancedBehaviorTestEngineInstanceSettings advancedBehaviorTestEngineInstanceSettings = null;

            if(enableCategories)
            {
                advancedBehaviorTestEngineInstanceSettings = new AdvancedBehaviorTestEngineInstanceSettings
                {
                    EnableCategories = enableCategories,
                    Categories = categories
                };
            }
            
            _internalInstance = new AdvancedBehaviorTestEngineInstance(rootDir, enableNLP, useStandardLibrary, advancedBehaviorTestEngineInstanceSettings);
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

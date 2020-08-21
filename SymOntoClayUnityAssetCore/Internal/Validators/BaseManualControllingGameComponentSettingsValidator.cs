using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.Validators
{
    public static class BaseManualControllingGameComponentSettingsValidator
    {
        public static void Validate(BaseManualControllingGameComponentSettings settings)
        {
            if (settings == null)
            {
                throw new ValidationException("Base manual controlling game component settings can not be null.");
            }

            if(settings.HostListener == null)
            {
                throw new ValidationException("HostListener can not be null.");
            }
        }
    }
}

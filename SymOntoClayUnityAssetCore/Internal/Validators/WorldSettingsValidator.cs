using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.Validators
{
    public static class WorldSettingsValidator
    {
        public static void Validate(WorldSettings settings)
        {
            if(settings == null)
            {
                throw new ValidationException("World settings can not be null.");
            }

            var sourceFilesDirs = settings.SourceFilesDirs;

            if (sourceFilesDirs == null || !sourceFilesDirs.Any() || sourceFilesDirs.All(p => string.IsNullOrWhiteSpace(p)))
            {
                throw new ValidationException("'SourceFilesDirs' can not be null, empty or contain all null or empty strings.");
            }

            //throw new NotImplementedException();
        }

        public static void Validate(LoggingSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}

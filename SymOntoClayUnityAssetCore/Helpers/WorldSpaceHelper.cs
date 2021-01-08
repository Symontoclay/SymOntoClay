using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Helpers
{
    public static class WorldSpaceHelper
    {
#if DEBUG
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static string GetRootWorldSpaceDir(string input)
        {
            var fileInfo = new FileInfo(input);

            return NGetRootWorldSpaceDir(fileInfo.Directory);
        }

        private static string NGetRootWorldSpaceDir(DirectoryInfo directory)
        {
#if DEBUG
            _logger.Info($"directory.FullName = {directory.FullName}");
#endif

            if(directory.GetFiles().Any(p => p.Name.EndsWith(".wspace")))
            {
                return directory.FullName;
            }

            var parentDirectory = directory.Parent;

            if(parentDirectory == null)
            {
                return string.Empty;
            }

            return NGetRootWorldSpaceDir(parentDirectory);
        }
    }
}

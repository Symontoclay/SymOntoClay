using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.CLI
{
    public static class RunCommandFilesSearcher
    {
#if DEBUG
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public static RunCommandFiles Run(CLICommand command)
        {
            var inputFile = command.InputFile;

#if DEBUG
            _logger.Info($"inputFile = {inputFile}");
#endif

            var fileInfo = new FileInfo(inputFile);

            var wSpaceFile = FindWSpaceFile(fileInfo.Directory);

#if DEBUG
            _logger.Info($"wSpaceFile = {wSpaceFile}");
#endif

            if(wSpaceFile == null)
            {
                throw new NotImplementedException();
            }

            var baseDir = wSpaceFile.Directory.FullName;

#if DEBUG
            _logger.Info($"baseDir = {baseDir}");
#endif

            var worldDir = Path.Combine(baseDir, "World");

#if DEBUG
            _logger.Info($"worldDir = {worldDir}");
#endif

            var worldDirInfo = new DirectoryInfo(worldDir);

            var worldFile = worldDirInfo.EnumerateFiles().Single(p => p.Name.EndsWith(".world"));

#if DEBUG
            _logger.Info($"worldFile = {worldFile}");
#endif

            var sourceFilesDir = Path.Combine(baseDir, "Modules");

#if DEBUG
            _logger.Info($"sourceFilesDir = {sourceFilesDir}");
#endif

            var imagesRootDir = Path.Combine(baseDir, "Images");

#if DEBUG
            _logger.Info($"imagesRootDir = {imagesRootDir}");
#endif

            var tmpDir = Path.Combine(baseDir, "Tmp");

#if DEBUG
            _logger.Info($"tmpDir = {tmpDir}");
#endif

            throw new NotImplementedException();
        }

        private static FileInfo FindWSpaceFile(DirectoryInfo dir)
        {
            var wSpaceFilesList = dir.EnumerateFiles().Where(p => p.Name.EndsWith(".wspace"));

            var count = wSpaceFilesList.Count();

            switch (count)
            {
                case 0:
                    {
                        var parentDir = dir.Parent;

                        if(parentDir == null)
                        {
                            return null;
                        }

                        return FindWSpaceFile(parentDir);
                    }

                case 1:
                    return wSpaceFilesList.First();

                default:
                    throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
        }
    }
}

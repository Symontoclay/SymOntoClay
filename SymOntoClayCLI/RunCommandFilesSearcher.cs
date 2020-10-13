/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public static RunCommandFiles Run(CLICommand command)
        {
            var result = new RunCommandFiles();

            var inputFile = command.InputFile;

#if DEBUG
            //_logger.Info($"inputFile = {inputFile}");
#endif

            result.LogicFile = inputFile;

            var fileInfo = new FileInfo(inputFile);

            var wSpaceFile = FindWSpaceFile(fileInfo.Directory);

#if DEBUG
            //_logger.Info($"wSpaceFile = {wSpaceFile}");
#endif

            if(wSpaceFile == null)
            {
                throw new NotImplementedException();
            }

            var baseDir = wSpaceFile.Directory.FullName;

#if DEBUG
            //_logger.Info($"baseDir = {baseDir}");
#endif

            var worldDir = Path.Combine(baseDir, "World");

#if DEBUG
            //_logger.Info($"worldDir = {worldDir}");
#endif

            var worldDirInfo = new DirectoryInfo(worldDir);

            var worldFile = worldDirInfo.EnumerateFiles().Single(p => p.Name.EndsWith(".world"));

#if DEBUG
            //_logger.Info($"worldFile = {worldFile}");
#endif

            result.WorldFile = worldFile.FullName;

            result.SharedModulesDir = Path.Combine(baseDir, "Modules");

#if DEBUG
            //_logger.Info($"result.SharedModulesDir = {result.SharedModulesDir}");
#endif

            result.ImagesRootDir = Path.Combine(baseDir, "Images");

#if DEBUG
            //_logger.Info($"result.ImagesRootDir = {result.ImagesRootDir}");
#endif

            result.TmpDir = Path.Combine(baseDir, "Tmp");

#if DEBUG
            //_logger.Info($"result.TmpDir = {result.TmpDir}");
#endif

            return result;
        }

        public static FileInfo FindWSpaceFile(string dir)
        {
            return FindWSpaceFile(new DirectoryInfo(dir));
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

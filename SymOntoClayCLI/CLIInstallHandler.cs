/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using NLog;
using SymOntoClay.ProjectFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.CLI
{
    public class CLIInstallHandler
    {
#if DEBUG
        //private readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public void Run(CLICommand command)
        {
#if DEBUG
            //_logger.Info($"command = {command}");
#endif

            var libName = command.ProjectName;

#if DEBUG
            //_logger.Info($"libName = {libName}");
#endif

            if (!command.NoLogo)
            {
                ConsoleWrapper.WriteLogChannel("Only installation from a local source is supported!");
                ConsoleWrapper.WriteText($"Installing `{libName}`...");
            }

            var worldSpaceFilesSearcherOptions = new WorldSpaceFilesSearcherOptions()
            {
                InputDir = command.InputDir
            };

            var targetFiles = WorldSpaceFilesSearcher.Run(worldSpaceFilesSearcherOptions);

#if DEBUG
            //_logger.Info($"targetFiles = {targetFiles}");
#endif

            var socAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(p => p.GetName().Name == "soc").Location;

#if DEBUG
            //_logger.Info($"socAssembly = {socAssembly}");
#endif

            var assemblyPath = (new FileInfo(socAssembly)).DirectoryName;

#if DEBUG
            //_logger.Info($"assemblyPath = {assemblyPath}");
#endif

            var libraryBaseSource = Path.Combine(assemblyPath, "LibsForInstall");

#if DEBUG
            //_logger.Info($"libraryBaseSource = {libraryBaseSource}");
#endif

            var librarySource = Path.Combine(libraryBaseSource, libName);

#if DEBUG
            //_logger.Info($"librarySource = {librarySource}");
#endif

            if(!Directory.Exists(librarySource))
            {
                if (!command.NoLogo)
                {
                    ConsoleWrapper.WriteError($"Unknown library `{libName}`!");
                }
                    
                return;
            }

            var libraryDest = Path.Combine(targetFiles.SharedLibsDir, libName);

#if DEBUG
            //_logger.Info($"libraryDest = {libraryDest}");
#endif

            if(Directory.Exists(libraryDest))
            {
                if (!command.NoLogo)
                {
                    ConsoleWrapper.WriteLogChannel($"Library `{libName}` already exists. It will be updated.");
                }

                Directory.Delete(libraryDest, true);
            }

            Directory.CreateDirectory(libraryDest);

            CopyDirectory(librarySource, librarySource, libraryDest);

            if (!command.NoLogo)
            {
                ConsoleWrapper.WriteText($"Library `{libName}` has been installed!");
            }
        }

        private void CopyDirectory(string dir, string baseSourceDir, string baseDestDir)
        {
#if DEBUG
            //_logger.Info($"dir = '{dir}'");
            //_logger.Info($"baseSourceDir = '{baseSourceDir}'");
            //_logger.Info($"baseDestDir = '{baseDestDir}'");
#endif

            var relativeDirName = dir.Replace(baseSourceDir, string.Empty);

#if DEBUG
            //_logger.Info($"relativeDirName = {relativeDirName}");
#endif

            var destDirName = Path.Combine(baseDestDir, relativeDirName);

#if DEBUG
            //_logger.Info($"destDirName = {destDirName}");
#endif

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            var subDirs = Directory.GetDirectories(dir);

            foreach(var subDir in subDirs)
            {
                CopyDirectory(subDir, baseSourceDir, baseDestDir);
            }

            var files = Directory.GetFiles(dir);

            foreach(var file in files)
            {
#if DEBUG
                //_logger.Info($"file = {file}");
#endif

                var relativeFileName = file.Replace(baseSourceDir, string.Empty).Substring(1);

#if DEBUG
                //_logger.Info($"relativeFileName = {relativeFileName}");
#endif

                var newFileName = Path.Combine(baseDestDir, relativeFileName);

#if DEBUG
                //_logger.Info($"newFileName = {newFileName}");
#endif

                File.Copy(file, newFileName, true);
            }
        }
    }
}

/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClayProjectFiles
{
    public static class WorldSpaceCreator
    {
        public static FileInfo CreateWithWSpaceFile(WorldSpaceCreationSettings settings, FileInfo wSpaceFile, Action<string> errorCallBack)
        {
            DirectoryInfo appDir = null;

            var kindOfNewCommand = settings.KindOfNewCommand;

            switch (kindOfNewCommand)
            {
                case KindOfNewCommand.NPC:
                    appDir = wSpaceFile.Directory.GetDirectories().SingleOrDefault(p => p.Name == "Npcs");
                    break;

                case KindOfNewCommand.Thing:
                    appDir = wSpaceFile.Directory.GetDirectories().SingleOrDefault(p => p.Name == "Things");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfNewCommand), kindOfNewCommand, null);
            }

            if (appDir == null)
            {
                throw new NotImplementedException();
            }

            CreateProject(settings, appDir.FullName, errorCallBack);

            return wSpaceFile;
        }

        public static FileInfo CreateWithOutWSpaceFile(WorldSpaceCreationSettings settings, string initialDir, Action<string> errorCallBack)
        {
            var projectName = settings.ProjectName;

            var worldSpaceDirName = Path.Combine(initialDir, projectName);

            if (!Directory.Exists(worldSpaceDirName))
            {
                Directory.CreateDirectory(worldSpaceDirName);
            }

            var wSpaceFileName = Path.Combine(worldSpaceDirName, $"{projectName}.wspace");

            if (!File.Exists(wSpaceFileName))
            {
                var wSpaceJsonFile = new WorldJsonFile() { MainNpc = projectName };

                File.WriteAllText(wSpaceFileName, JsonConvert.SerializeObject(wSpaceJsonFile, Formatting.Indented));
            }

            var worldDirName = Path.Combine(worldSpaceDirName, "World");

            if (!Directory.Exists(worldDirName))
            {
                Directory.CreateDirectory(worldDirName);
            }

            var worldFileName = Path.Combine(worldDirName, $"{projectName}.world");

            if (!File.Exists(worldFileName))
            {
                File.WriteAllText(worldFileName, "{}");
            }

            var thingDirName = Path.Combine(worldSpaceDirName, "Things");

            if (!Directory.Exists(thingDirName))
            {
                Directory.CreateDirectory(thingDirName);
            }

            var playerDirName = Path.Combine(worldSpaceDirName, "Players");

            if (!Directory.Exists(playerDirName))
            {
                Directory.CreateDirectory(playerDirName);
            }

            var modulesDirName = Path.Combine(worldSpaceDirName, "Modules");

            if (!Directory.Exists(modulesDirName))
            {
                Directory.CreateDirectory(modulesDirName);
            }

            var npcsDirName = Path.Combine(worldSpaceDirName, "Npcs");

            if (!Directory.Exists(npcsDirName))
            {
                Directory.CreateDirectory(npcsDirName);
            }

            if(!settings.CreateOnlyWorldspace)
            {
                var targetDirName = string.Empty;

                var kindOfNewCommand = settings.KindOfNewCommand;

                switch (kindOfNewCommand)
                {
                    case KindOfNewCommand.NPC:
                        targetDirName = npcsDirName;
                        break;

                    case KindOfNewCommand.Thing:
                        targetDirName = thingDirName;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfNewCommand), kindOfNewCommand, null);
                }

                CreateProject(settings, targetDirName, errorCallBack);
            }            

            return new FileInfo(wSpaceFileName);
        }

        private static void CreateProject(WorldSpaceCreationSettings settings, string npcsDirName, Action<string> errorCallBack)
        {
            var projectName = settings.ProjectName;

            var projectDirName = Path.Combine(npcsDirName, projectName);

            if (Directory.Exists(projectDirName))
            {
                errorCallBack($"The NPC '{projectName}' already exists!");
                return;
            }
            else
            {
                Directory.CreateDirectory(projectDirName);
            }

            var npcFileName = Path.Combine(projectDirName, $"{projectName}.sobj");

            File.WriteAllText(npcFileName, "{}");

            var appFileName = Path.Combine(projectDirName, $"{projectName}.soc");

            var sb = new StringBuilder();
            sb.AppendLine($"app {projectName}");
            sb.AppendLine("{");
            sb.AppendLine("    on Init =>");
            sb.AppendLine("    {");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(appFileName, sb.ToString());
        }
    }
}

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
            var appDir = wSpaceFile.Directory.GetDirectories().SingleOrDefault(p => p.Name == "Npcs");

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

            var hostsDirName = Path.Combine(worldSpaceDirName, "Things");

            if (!Directory.Exists(hostsDirName))
            {
                Directory.CreateDirectory(hostsDirName);
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

            CreateProject(settings, npcsDirName, errorCallBack);

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

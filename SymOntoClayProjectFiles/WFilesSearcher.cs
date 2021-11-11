using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClayProjectFiles
{
    public static class WFilesSearcher
    {
        public static FileInfo FindWSpaceFile(string dir)
        {
            return FindWSpaceFile(new DirectoryInfo(dir));
        }

        public static FileInfo FindWSpaceFile(DirectoryInfo dir)
        {
            var wSpaceFilesList = dir.EnumerateFiles().Where(p => p.Name.EndsWith(".wspace"));

            var count = wSpaceFilesList.Count();

            switch (count)
            {
                case 0:
                    {
                        var parentDir = dir.Parent;

                        if (parentDir == null)
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

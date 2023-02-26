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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.ProjectFiles
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

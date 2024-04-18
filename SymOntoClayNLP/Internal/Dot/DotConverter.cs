/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymOntoClay.NLP.Internal.Dot
{
    /// <summary>
    /// You can see it by http://magjac.com/graphviz-visual-editor/.
    /// </summary>
    public class DotConverter
    {
        public static string ConvertToString(ICGNode node)
        {
            var tmpContext = new DotContext();

            var tmpMainLeaf = tmpContext.CreateRootLeaf(node);

            tmpMainLeaf.Run();

            return tmpMainLeaf.Text;
        }

        public static void DumpToFile(ICGNode node, string fileName)
        {
            var dotStr = ConvertToString(node);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (var fs = File.OpenWrite(fileName))
            {
                using (var writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.Write(dotStr);
                    fs.Flush();
                }
            }
        }
    }
}

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

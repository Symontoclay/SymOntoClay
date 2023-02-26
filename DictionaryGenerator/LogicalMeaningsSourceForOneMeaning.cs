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
using System.Text;

namespace DictionaryGenerator
{
    public class LogicalMeaningsSourceForOneMeaning
    {
        public LogicalMeaningsSourceForOneMeaning(string path)
        {
#if DEBUG
            NLog.LogManager.GetCurrentClassLogger().Info($"LogicalMeaningsSourceForOneMeaning path = {path}");
#endif

            var fileInfo = new FileInfo(path);
            var fileName = fileInfo.Name;
            var extension = fileInfo.Extension;

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"LogicalMeaningsSourceForOneMeaning fileName = {fileName}");
#endif

            fileName = fileName.Replace(extension, string.Empty);

#if DEBUG
            //NLog.LogManager.GetCurrentClassLogger().Info($"LogicalMeaningsSourceForOneMeaning NEXT fileName = {fileName}");
#endif

            LogicalMeaning = fileName;

            using (var fs = File.OpenRead(path))
            {
                using (var sr = new StreamReader(fs))
                {
                    var currentLine = string.Empty;

                    while ((currentLine = sr.ReadLine()) != null)
                    {
#if DEBUG
                        NLog.LogManager.GetCurrentClassLogger().Info($"LogicalMeaningsSourceForOneMeaning currentLine = {currentLine}");
#endif

                        if(string.IsNullOrWhiteSpace(currentLine))
                        {
                            continue;
                        }

                        mWordsList.Add(currentLine.Trim());
                    }
                }
            }
        }

        public string LogicalMeaning { get; private set; }
        private List<string> mWordsList = new List<string>();

        public bool ContainsWord(string word)
        {
            return mWordsList.Contains(word);
        }
    }
}

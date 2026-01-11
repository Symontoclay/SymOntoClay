/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

namespace DictionaryGenerator
{
    public class LogicalMeaningsSource
    {
#if DEBUG
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
#endif

        public LogicalMeaningsSource(string localPath)
        {
#if DEBUG
            _logger.Info($"LogicalMeaningsSource localPath = {localPath}");
#endif

            var rootPath = AppDomain.CurrentDomain.BaseDirectory;

            var absolutePath = Path.Combine(rootPath, localPath);

#if DEBUG
            _logger.Info($"LogicalMeaningsSource absolutePath = {absolutePath}");
#endif

            if(Directory.Exists(absolutePath))
            {
                var filesList = Directory.GetFiles(absolutePath);

#if DEBUG
                _logger.Info($"LogicalMeaningsSource filesList.Length = {filesList.Length}");
#endif

                foreach(var fileName in filesList)
                {
#if DEBUG
                    _logger.Info($"LogicalMeaningsSource fileName = {fileName}");
#endif

                    var item = new LogicalMeaningsSourceForOneMeaning(fileName);
                    mSourcesList.Add(item);
                }
            }
        }

        private List<LogicalMeaningsSourceForOneMeaning> mSourcesList = new List<LogicalMeaningsSourceForOneMeaning>();

        public List<string> GetLogicalMeanings(string word)
        {
            var result = new List<string>();

            foreach(var item in mSourcesList)
            {
                if(item.ContainsWord(word))
                {
                    result.Add(item.LogicalMeaning);
                }
            }
            return result;
        }
    }
}

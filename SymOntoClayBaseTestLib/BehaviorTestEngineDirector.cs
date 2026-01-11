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

namespace SymOntoClay.BaseTestLib
{
    public static class BehaviorTestEngineDirector
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static IBehaviorTestEngineInstance CreateMinimalInstance(this IBehaviorTestEngineInstanceBuilder builder, string fileContent, Func<int, string, bool> logHandler)
        {
            builder.DontUseTimeoutToEnd();
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }

        public static IBehaviorTestEngineInstance CreateMinimalInstance(this IBehaviorTestEngineInstanceBuilder builder, string fileContent, Action<int, string> logHandler)
        {
            builder.UseDefaultTimeoutToEnd();
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }

        public static IBehaviorTestEngineInstance CreateMinimalInstance(this IBehaviorTestEngineInstanceBuilder builder, string fileContent, Action<string> logHandler)
        {
            builder.UseDefaultTimeoutToEnd();
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }

        public static IBehaviorTestEngineInstance CreateMinimalInstanceWithCategories(this IBehaviorTestEngineInstanceBuilder builder,
            string fileContent, List<string> categories, Func<int, string, bool> logHandler)
        {
            builder.DontUseTimeoutToEnd();
            builder.UseCategories(categories);
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }

        public static IBehaviorTestEngineInstance CreateMinimalInstanceWithImportStandardLibrary(this IBehaviorTestEngineInstanceBuilder builder, string fileContent, Func<int, string, bool> logHandler)
        {
            builder.DontUseTimeoutToEnd();
            builder.SetUsingStandardLibrary(KindOfUsingStandardLibrary.Import);
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }

        public static IBehaviorTestEngineInstance CreateMinimalInstanceWithImportStandardLibrary(this IBehaviorTestEngineInstanceBuilder builder, string fileContent, Action<int, string> logHandler)
        {
            builder.DontUseTimeoutToEnd();
            builder.SetUsingStandardLibrary(KindOfUsingStandardLibrary.Import);
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }

        public static IBehaviorTestEngineInstance CreateMinimalInstanceWithPlatformListener(this IBehaviorTestEngineInstanceBuilder builder,
            string fileContent, Func<int, string, bool> logHandler, object platformListener)
        {
            builder.DontUseTimeoutToEnd();
            builder.UsePlatformListener(platformListener);
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }

        public static IBehaviorTestEngineInstance CreateMinimalInstanceWithPlatformListener(this IBehaviorTestEngineInstanceBuilder builder,
            string fileContent, Action<int, string> logHandler, object platformListener)
        {
            builder.UseDefaultTimeoutToEnd();
            builder.UsePlatformListener(platformListener);
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }

        public static IBehaviorTestEngineInstance CreateMinimalInstanceWithPlatformListenerAndImportStandardLibrary(this IBehaviorTestEngineInstanceBuilder builder,
            string fileContent, Func<int, string, bool> logHandler, object platformListener)
        {
            builder.DontUseTimeoutToEnd();
            builder.SetUsingStandardLibrary(KindOfUsingStandardLibrary.Import);
            builder.UsePlatformListener(platformListener);
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }

        public static IBehaviorTestEngineInstance CreateMinimalInstanceWithOneHtnIteration(this IBehaviorTestEngineInstanceBuilder builder,
            string fileContent, Func<int, string, bool> logHandler)
        {
            builder.DontUseTimeoutToEnd();
            builder.SethHtnIterationsMaxCount(1);
            builder.TestedCode(fileContent);
            builder.LogHandler(logHandler);

            return builder.Build();
        }
    }
}

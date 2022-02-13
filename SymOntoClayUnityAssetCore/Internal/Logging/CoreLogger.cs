/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.Logging
{
    public class CoreLogger: IWorldCoreComponent
    {
        private readonly object _lockObj = new object();
        private readonly IWorldCoreContext _coreContext;
        private readonly CoreLoggerSettings _settings;
        private readonly LoggerContext _loggerContext = new LoggerContext();
        private readonly IEntityLogger _coreLogger;

        public CoreLogger(LoggingSettings settings, IWorldCoreContext coreContext)
        {
            _coreContext = coreContext;
            _settings = new CoreLoggerSettings();
            _settings.LogDir = settings.LogDir;
            _settings.PlatformLoggers = settings.PlatformLoggers?.ToList();

            _loggerContext.Enable = settings.Enable;

            if(!string.IsNullOrWhiteSpace(_settings.LogDir))
            {
                Directory.CreateDirectory(_settings.LogDir);

                var now = DateTime.Now;
                var todaysDir = $"{now.Year}_{now.Month:00}_{now.Day:00}_{now.Hour:00}_{now.Minute:00}_{now.Second:00}";

                _settings.LogDir = Path.Combine(_settings.LogDir, todaysDir);

                Directory.CreateDirectory(_settings.LogDir);
            }

            _coreLogger = new InternalLogger(_loggerContext, "Core", _settings);
        }

        /// <summary>
        /// Gets or sets value of enable logging.
        /// It alows enable or disable logging or remote connection for whole components synchronously.
        /// </summary>
        public bool EnableLogging
        {
            get
            {
                lock(_lockObj)
                {
                    return _loggerContext.Enable;
                }
            }

            set
            {
                lock (_lockObj)
                {
                    _loggerContext.Enable = value;
                }
            }
        }

        public IEntityLogger WordCoreLogger => _coreLogger;

        public IEntityLogger CreateLogger(string name)
        {
            return new InternalLogger(_loggerContext, name, _settings);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        /// <inheritdoc/>
        public bool IsDisposed { get => false; }
    }
}

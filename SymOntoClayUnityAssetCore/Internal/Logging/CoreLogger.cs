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
                var todaysDir = $"{now.Year}_{now.Month}_{now.Day}_{now.Hour}_{now.Minute}_{now.Second}";

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

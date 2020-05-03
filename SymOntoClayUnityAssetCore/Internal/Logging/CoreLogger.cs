using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.Logging
{
    public class CoreLogger: IWordCoreComponent
    {
        private readonly object _lockObj = new object();
        private readonly IWorlCoreContext _coreContext;
        private readonly CoreLoggerSettings _settings;
        private readonly LoggerContext _loggerContext = new LoggerContext();

        public CoreLogger(LoggingSettings settings, IWorlCoreContext coreContext)
        {
            _coreContext = coreContext;
            _settings = new CoreLoggerSettings();
            _settings.LogDir = settings.LogDir;
            _settings.PlatformLoggers = settings.PlatformLoggers?.ToList();

            _loggerContext.Enable = settings.Enable;
        }

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

        /// <inheritdoc/>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsDisposed { get => throw new NotImplementedException(); }
    }
}

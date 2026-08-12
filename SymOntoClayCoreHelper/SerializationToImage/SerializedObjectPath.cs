using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializedObjectPath
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static SerializedObjectPath Parse(string path)
        {
#if DEBUG
            _logger.Info($"path = {path}");
#endif

            var pathChanksList = path.Split('/');

            foreach (var chank in pathChanksList)
            {
#if DEBUG
                _logger.Info($"chank = {chank}");
#endif
            }

            throw new NotImplementedException("C3B4EBD4-3618-4E11-96BC-1041D7440574");
        }
    }
}

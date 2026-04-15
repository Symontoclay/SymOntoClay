using SymOntoClay.CoreHelper.Serialization;

namespace TestSandbox.SerializationToImage
{
    public class SerializationToImageHandler
    {
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            Case1();

            _logger.Info("End");
        }

        private void Case1()
        {
            var worlContext = new TstWorldContext();

            var serializer = new Serializer();
        }
    }
}

using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using NLog;

namespace SymOntoClay.CoreHelper.SerializerAdapters
{
    public sealed class ULongFormatter : IMessagePackFormatter<ulong>
    {
#if DEBUG
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public void Serialize(ref MessagePackWriter writer, ulong value, MessagePackSerializerOptions options)
        {
#if DEBUG
            _logger.Info($"value = {value}");
#endif

            //Always write as UInt64
            writer.WriteUInt64(value);
        }

        public ulong Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
#if DEBUG
            _logger.Info("Hi!");
#endif

            //We check the type and read it as UInt64
            if (reader.TryReadNil())
            {
                return 0ul;
            }

            return reader.ReadUInt64();
        }
    }

    public class MessagePackSerializerAdapter : ISerializerAdapter
    {
        public MessagePackSerializerAdapter()
        {
            var resolver = CompositeResolver.Create(
                new IMessagePackFormatter[]
                {
                    new ULongFormatter() //Our custom formatter
                },
                new IFormatterResolver[]
                {
                    StandardResolver.Instance //Standard resolvers 
                });

            _options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
        }

        private readonly MessagePackSerializerOptions _options;

        /// <inheritdoc/>
        public byte[] Serialize<T>(T obj)
        {
            return MessagePackSerializer.Serialize(obj/*, _options*/);
        }

        /// <inheritdoc/>
        public T Deserialize<T>(byte[] data)
        {
            return MessagePackSerializer.Deserialize<T>(data/*, _options*/);
        }
    }
}

/*
var options = MessagePackSerializerOptions.Standard
    .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

byte[] dump = MessagePackSerializer.Serialize(complexObject, options);
 */

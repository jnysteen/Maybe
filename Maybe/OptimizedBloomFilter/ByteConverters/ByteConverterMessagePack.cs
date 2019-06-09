using System.IO;
using MessagePack;

namespace Maybe.OptimizedBloomFilter.ByteConverters
{
    /// <summary>
    ///  Converts an instance of T into a byte array using MessagePack (https://github.com/neuecc/MessagePack-CSharp)
    ///  T must be annotated with MessagePack attributes to enable serialization - otherwise, exceptions will be thrown!  
    /// </summary>
    public class ByteConverterMessagePack<T> : IByteConverter<T>
    {
        /// <inheritdoc />
        public byte[] ConvertToBytes(T item)
        {
            return MessagePackSerializer.Serialize(item);
        }
    }
}
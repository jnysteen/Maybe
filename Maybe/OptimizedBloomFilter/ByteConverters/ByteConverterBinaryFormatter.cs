using System.IO;

namespace Maybe.OptimizedBloomFilter.ByteConverters
{
    /// <summary>
    ///     Maybe.net's original method for converting objects into byte arrays
    /// </summary>
    public class ByteConverterBinaryFormatter<T> : IByteConverter<T>
    {
        /// <inheritdoc />
        public byte[] ConvertToBytes(T item)
        {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, item);
                return stream.ToArray();
            }
        }
    }
}
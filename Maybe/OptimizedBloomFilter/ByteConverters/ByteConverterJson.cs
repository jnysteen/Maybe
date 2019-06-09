using System.Text;
using Newtonsoft.Json;

namespace Maybe.OptimizedBloomFilter.ByteConverters
{
    /// <summary>
    ///  Converts an instance of T into a byte array using Newtonsoft.Json
    /// </summary>
    public class ByteConverterJson<T> : IByteConverter<T>
    {
        /// <inheritdoc />
        public byte[] ConvertToBytes(T item)
        {
            var jsonSerialized = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(jsonSerialized);
        }
    }
}
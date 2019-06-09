namespace Maybe.OptimizedBloomFilter.ByteConverters
{
    /// <summary>
    ///     Converts an instance of T into a corresponding byte array representation
    /// </summary>
    public interface IByteConverter<T>
    {
        /// <summary>
        ///     Converts an instance of T into a corresponding byte array representation
        /// </summary>
        /// <param name="item">The instance of T</param>
        /// <returns>The instance as a byte array</returns>
        byte[] ConvertToBytes(T item);
    }
}
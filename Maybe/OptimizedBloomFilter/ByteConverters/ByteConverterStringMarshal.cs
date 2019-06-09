using System;
using System.Runtime.InteropServices;

namespace Maybe.OptimizedBloomFilter.ByteConverters
{
    /// <summary>
    ///     Converts a string into a byte array by copying the string's bytes directly from memory
    /// </summary>
    public class ByteConverterStringMarshal: IByteConverter<string>
    {
        /// <inheritdoc />
        public unsafe byte[] ConvertToBytes(string item)
        {
            var tempByte = new byte[item.Length * 2];
            fixed (void* ptr = item)
            {
                Marshal.Copy(new IntPtr(ptr), tempByte, 0, item.Length * 2);
            }

            return tempByte;
        }
    }
}
using Maybe.BloomFilter;
using Maybe.OptimizedBloomFilter.ByteConverters;

namespace Maybe.OptimizedBloomFilter
{
    /// <summary>
    ///     An string-only optimized version of the original <see cref="BloomFilter{T}"/>,
    /// </summary>
    public class StringOptimizedBloomfilter : GenericOptimizedBloomFilter<string>
    {
        /// <summary>
        /// Docs
        /// </summary>
        /// <param name="bitArraySize"></param>
        /// <param name="numHashes"></param>
        /// <param name="byteConverter"></param>
        protected StringOptimizedBloomfilter(int bitArraySize, int numHashes, IByteConverter<string> byteConverter) : base(bitArraySize, numHashes, byteConverter)
        {
        }

        /// <summary>
        /// Docs
        /// </summary>
        /// <param name="expectedItems"></param>
        /// <param name="acceptableErrorRate"></param>
        /// <param name="byteConverter"></param>
        public StringOptimizedBloomfilter(int expectedItems, double acceptableErrorRate, IByteConverter<string> byteConverter) : base(expectedItems, acceptableErrorRate, byteConverter)
        {
        }
    }
}
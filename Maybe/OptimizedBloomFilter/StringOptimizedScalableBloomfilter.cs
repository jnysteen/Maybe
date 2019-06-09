using Maybe.BloomFilter;
using Maybe.OptimizedBloomFilter.ByteConverters;

namespace Maybe.OptimizedBloomFilter
{
    /// <summary>
    ///     An string-only optimized version of the original <see cref="ScalableBloomFilter{T}"/>,
    /// </summary>
    public class StringOptimizedScalableBloomFilter : GenericOptimizedScalableBloomFilter<string>
    {
        /// <inheritdoc />
        public StringOptimizedScalableBloomFilter(double maximumErrorRate, IByteConverter<string> byteConverter) : base(maximumErrorRate, byteConverter)
        {
        }
    }
}
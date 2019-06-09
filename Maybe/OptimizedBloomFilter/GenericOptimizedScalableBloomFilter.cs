using System;
using System.Collections.Generic;
using System.Linq;
using Maybe.BloomFilter;
using Maybe.OptimizedBloomFilter.ByteConverters;

namespace Maybe.OptimizedBloomFilter
{
    /// <summary>
    ///     An optimized version of the original <see cref="ScalableBloomFilter{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of data that will be contained in the bloom filter.</typeparam>
    public class GenericOptimizedScalableBloomFilter<T>
    {
        /// <summary>
        /// The minimum number of items that this scalable bloom filter will handle.
        /// </summary>
        public const int MinimumCapacity = 50;

        private readonly List<GenericOptimizedBloomFilterBase<T>> _filters;
        private readonly double _maxErrorRate;
        private readonly IByteConverter<T> _byteConverter;
        private int _activeItemCount;
        private int _capacity;

        /// <summary>
        /// Creates a new bloom filter with error rate limited to the desired ratio.
        /// </summary>
        /// <param name="maximumErrorRate">Maximum error rate to tolerate -- more memory will be used to reduce error rate.</param>
        /// <param name="byteConverter"></param>
        public GenericOptimizedScalableBloomFilter(double maximumErrorRate, IByteConverter<T> byteConverter)
        {
            _maxErrorRate = maximumErrorRate;
            _byteConverter = byteConverter;
            _filters = new List<GenericOptimizedBloomFilterBase<T>>();
        }

        /// <summary>
        /// Adds a new item to the bloom filter and scales the bloom filter as needed.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (_activeItemCount >= _capacity)
            {
                _capacity = Math.Max(MinimumCapacity, _capacity * 2);
                AddNewFilter(_maxErrorRate, _capacity);
                _activeItemCount = 0;
            }
            _activeItemCount++;
            _filters.Last().Add(item);
        }

        /// <summary>
        /// Checks whether an item may currently exist in the bloom filter.
        /// </summary>
        /// <param name="item">The item to check for membership in this <see cref="GenericOptimizedScalableBloomFilter{T}"/></param>
        /// <returns>True if the item MIGHT be in the collection. False if the item is NOT in the collection.</returns>
        public bool Contains(T item) => _filters.Any(filter => filter.Contains(item));

        /// <summary>
        /// Gets the number of filters that are currently being used internally to hold items without exceeding the error rate.
        /// </summary>
        public int NumberFilters => _filters.Count();

        private void AddNewFilter(double maxError, int capacity)
        {
            _filters.Add(new GenericOptimizedBloomFilter<T>(capacity, maxError, _byteConverter));
        }
        
    }
}
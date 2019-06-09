﻿using System;
using System.Linq;
using Maybe.OptimizedBloomFilter.ByteConverters;

namespace Maybe.BloomFilter
{
    /// <summary>
    /// A bloom filter modified to store counters and allow elements to be removed from the collection.
    /// </summary>
    /// <typeparam name="T">The type of item to be stored in the collection</typeparam>
    [Serializable]
    public class CountingBloomFilter<T> : BloomFilterBase<T>
    {
        private readonly IByteConverter<T> _byteConverter;
        private readonly byte[] _collectionState;

        /// <summary>
        /// Creates a new counting bloom filter -- a bloom filter capable of tracking how many times a bit has been set
        /// </summary>
        /// <param name="arraySize">Size of the internal bit array to track items</param>
        /// <param name="numHashes">Number of times the input should be hashed before working with the bit array.</param>
        /// <param name="byteConverter"></param>
        protected CountingBloomFilter(int arraySize, int numHashes, IByteConverter<T> byteConverter) : base(arraySize, numHashes, byteConverter)
        {
            _byteConverter = byteConverter;
            _collectionState = new byte[arraySize];
            for (var i = 0; i < _collectionState.Length; i++)
            {
                _collectionState[i] = 0;
            }
        }


        /// <summary>
        /// Creates a new counting bloom filter
        /// </summary>
        /// <param name="expectedItems">Expected number of items for the bloom filter to hold</param>
        /// <param name="acceptableErrorRate">The maximum error rate for this counting bloom filter when items are below expected value</param>
        /// <param name="byteConverter"></param>
        public CountingBloomFilter(int expectedItems, double acceptableErrorRate, IByteConverter<T> byteConverter) : base(expectedItems, acceptableErrorRate, byteConverter)
        {
            _byteConverter = byteConverter;
            _collectionState = new byte[CollectionLength];
            for (var i = 0; i < _collectionState.Length; i++)
            {
                _collectionState[i] = 0;
            }
        }


        /// <inheritdoc />
        public override double FillRatio => _collectionState.Count(position => position > 0) / (double)_collectionState.Length;

        /// <summary>
        /// Creates a new counting bloom filter with appropriate bit width and hash functions for your expected size and error rate.
        /// </summary>
        /// <param name="expectedItems">The maximum number of items you expect to be in the counting bloom filter</param>
        /// <param name="acceptableErrorRate">The maximum rate of false positives you can accept. Must be a value between 0.00-1.00</param>
        /// <param name="byteConverter"></param>
        /// <returns>A new bloom filter configured appropriately for number of items and error rate</returns>
        public static CountingBloomFilter<T> Create(int expectedItems, double acceptableErrorRate, IByteConverter<T> byteConverter)
        {
            return new CountingBloomFilter<T>(expectedItems, acceptableErrorRate, byteConverter);
        }

        /// <summary>
        /// Adds an item to the counting bloom filter
        /// </summary>
        /// <param name="item">The item which should be added</param>
        public override void Add(T item) => DoHashAction(item, hash =>
        {
            if (_collectionState[hash] < byte.MaxValue)
            {
                _collectionState[hash]++;
            }
        });

        /// <summary>
        /// Checks if this counting bloom filter currently contains an item
        /// </summary>
        /// <param name="item">The item for which to search in the bloom filter</param>
        /// <returns>False if the item is NOT in the counting bloom filter. True if the item MIGHT be in the counting bloom filter.</returns>
        public override bool Contains(T item)
        {
            var containsItem = true;
            DoHashAction(item, hash => containsItem = containsItem && _collectionState[hash] > 0);
            return containsItem;
        }

        /// <summary>
        /// Adds an item to the bloom filter and returns if it might already be contained before
        /// </summary>
        /// <param name="item">The item which should be added and searched in the bloom filter</param>
        /// <returns>False if the item was NOT in the bloom filter before. True if the item MIGHT have been in the bloom filter.</returns>
        public override bool AddAndCheck(T item)
        {
            var containsItem = true;
            DoHashAction(item, hash =>
            {
                containsItem = containsItem && _collectionState[hash] > 0;
                if (_collectionState[hash] < byte.MaxValue)
                {
                    _collectionState[hash]++;
                }
            });
            return containsItem;
        }

        /// <summary>
        /// Removes an item from the counting bloom filter
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if the counting bloom filter might contain the item and the item was removed. False otherwise.</returns>
        public bool Remove(T item)
        {
            if (!Contains(item)) return false;

            DoHashAction(item, hash => _collectionState[hash]--);
            return true;
        }

        /// <summary>
        /// Returns the counter value at a given index if the index is valid. 0 if the index is invalid.
        /// </summary>
        public byte CounterAt(int index)
        {
            return index < 0 || index >= _collectionState.Length ? (byte)0 : _collectionState[index];
        }
    }
}

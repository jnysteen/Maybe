using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Order;
using Maybe.BloomFilter;
using Maybe.OptimizedBloomFilter.ByteConverters;

namespace Maybe.Benchmark.AddAndCount
{
    /// <summary>
    /// Benchmarks different methods for counting the amount of unique strings in a collection
    /// </summary>
    [CoreJob()]
    [MemoryDiagnoser]
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [EncodingAttribute.Unicode]
    [CsvExporter(CsvSeparator.Semicolon), RPlotExporter]
    
    public class CountingBenchmarker
    {
        [Params(10000)]
        public int ItemsToInsert { get; set; }

        [Params(0.02)]
        public double MaximumErrorRate { get; set; }
        
        private HashSet<string> _hashSet;
        private ScalableBloomFilter<string> _scalableBloomFilter;
        private BloomFilter<string> _bloomFilter;

        [GlobalSetup(Target = nameof(AddItemsThenCount_HashSet))]
        public void Setup_HashSet()
        {
            _hashSet = new HashSet<string>();
        }
        
        /// <summary>
        /// Counts the amount of unique strings using a hashset
        /// </summary>
        [Benchmark(Description = "HashSet")]
        public void AddItemsThenCount_HashSet()
        {
            for (var i = 0; i < ItemsToInsert; i++)
            {
                var s = $"string-{i + 1}";
                _hashSet.Add(s);
            }

            var count = _hashSet.Count;
        }
        
        
        [GlobalSetup(Target = nameof(AddItemsThenCount_ScalableBloomFilter))]
        public void Setup_ScalableBloomFilter()
        {
            var byteConverter = new ByteConverterStringMarshal();
            _scalableBloomFilter = new ScalableBloomFilter<string>(MaximumErrorRate, byteConverter);
        }
        
        /// <summary>
        /// Counts the amount of unique strings using a scalable Bloom filter
        /// </summary>
        [Benchmark(Baseline = true, Description = "ScalableBloomFilter")]
        public void AddItemsThenCount_ScalableBloomFilter()
        {
            var count = 0;
            for (var i = 0; i < ItemsToInsert; i++)
            {
                var s = $"string-{i + 1}";
                if (_scalableBloomFilter.Contains(s)) 
                    continue;
                count++;
                _scalableBloomFilter.Add(s);
            }

            var x = count;
        }
        
        
        [GlobalSetup(Target = nameof(AddItemsThenCount_BloomFilter))]
        public void Setup_BloomFilter()
        {
            var byteConverter = new ByteConverterStringMarshal();
            _bloomFilter = new BloomFilter<string>(ItemsToInsert, MaximumErrorRate, byteConverter);
        }
        
        /// <summary>
        /// Counts the amount of unique strings using a Bloom filter
        /// </summary>
        [Benchmark(Description = "BloomFilter")]
        public void AddItemsThenCount_BloomFilter()
        {
            var count = 0;
            for (var i = 0; i < ItemsToInsert; i++)
            {
                var s = $"string-{i + 1}";
                if (!_bloomFilter.AddAndCheck(s)) 
                    count++;
            }

            var x = count;
        }
    }
}
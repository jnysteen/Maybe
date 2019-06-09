using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Order;
using Maybe.BloomFilter;
using Maybe.OptimizedBloomFilter;
using Maybe.OptimizedBloomFilter.ByteConverters;

namespace Maybe.Benchmark.Contains
{
    /// <summary>
    /// Benchmarks how fast a scalable Bloom filter can perform "contains" compared to other data structures
    /// </summary>
    [CoreJob]
    [BenchmarkCategory("ScalableBloomFilter", "ObjectType:string", "OperationType:contains")]
    [MemoryDiagnoser]
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [EncodingAttribute.Unicode]
    [CsvExporter(CsvSeparator.Semicolon), RPlotExporter]
    public class ScalableBloomFilterContainsBenchmarker
    {
        private ScalableBloomFilter<string> _regularScalableBloomFilter;
        private GenericOptimizedScalableBloomFilter<string> _genericOptimizedScalableBloomFilter;
        private StringOptimizedScalableBloomFilter _stringOptimizedScalableBloomFilter;
        private HashSet<string> _hashSet;
        private static readonly IByteConverter<string> ByteConverter = new ByteConverterBinaryFormatter<string>();

        [Params(1000, 10000, 50000)]
        public int ItemsToInsert { get; set; }

        [Params(.02)]
        public double MaximumErrorRate { get; set; }

        
        [GlobalSetup(Target = nameof(CheckIfContainsItem_ScalableBloomFilter))]
        public void Setup_ScalableBloomFilter()
        {
            _regularScalableBloomFilter = new ScalableBloomFilter<string>(MaximumErrorRate, ByteConverter);

            for (var i = 0; i < ItemsToInsert; i++)
            {
                var s = $"string-{i + 1}";
                _regularScalableBloomFilter.Add(s);
            }
        }
        
        [Benchmark(Description = "ScalableBloomFilter")]
        public void CheckIfContainsItem_ScalableBloomFilter()
        {
            for (int i = 0; i < ItemsToInsert; i++)
            {
                var contains = _regularScalableBloomFilter.Contains($"string-{i+1}");
            }
        }     
        
        [GlobalSetup(Target = nameof(CheckIfContainsItem_GenericOptimizedScalableBloomFilter))]
        public void Setup_GenericOptimizedScalableBloomFilter()
        {
            _genericOptimizedScalableBloomFilter = new GenericOptimizedScalableBloomFilter<string>(MaximumErrorRate, ByteConverter);

            for (var i = 0; i < ItemsToInsert; i++)
            {
                var s = $"string-{i + 1}";
                _genericOptimizedScalableBloomFilter.Add(s);
            }
        }
        
        [Benchmark(Description = "GenericOptimizedScalableBloomFilter")]
        public void CheckIfContainsItem_GenericOptimizedScalableBloomFilter()
        {
            for (int i = 0; i < ItemsToInsert; i++)
            {
                var contains = _genericOptimizedScalableBloomFilter.Contains($"string-{i+1}");
            }
        }  
        
        [GlobalSetup(Target = nameof(CheckIfContainsItem_StringOptimizedScalableBloomFilter))]
        public void Setup_StringOptimizedScalableBloomFilter()
        {
            var byteConverter = new ByteConverterStringMarshal();
            _stringOptimizedScalableBloomFilter = new StringOptimizedScalableBloomFilter(MaximumErrorRate, byteConverter);

            for (var i = 0; i < ItemsToInsert; i++)
            {
                var s = $"string-{i + 1}";
                _stringOptimizedScalableBloomFilter.Add(s);
            }
        }
        
        [Benchmark(Description = "StringOptimizedScalableBloomFilter")]
        public void CheckIfContainsItem_StringOptimizedScalableBloomFilter()
        {
            for (int i = 0; i < ItemsToInsert; i++)
            {
                var contains = _stringOptimizedScalableBloomFilter.Contains($"string-{i+1}");
            }
        }
        
        
        [GlobalSetup(Target = nameof(CheckIfContainsItem_HashSet))]
        public void Setup_HashSet()
        {
            _hashSet = new HashSet<string>();

            for (var i = 0; i < ItemsToInsert; i++)
            {
                var s = $"string-{i + 1}";
                _hashSet.Add(s);
            }
        }
        
        [Benchmark(Baseline = true, Description = "HashSet")]
        public void CheckIfContainsItem_HashSet()
        {
            for (int i = 0; i < ItemsToInsert; i++)
            {
                var contains = _hashSet.Contains($"string-{i+1}");
            }
        }     
    }
}
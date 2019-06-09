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
        private ScalableBloomFilter<string> _regularBloomFilter;
        private GenericOptimizedScalableBloomFilter<string> _genericOptimizedBloomFilter;
        private StringOptimizedScalableBloomFilter _stringOptimizedBloomFilter;
        private StringOptimizedBloomfilter _stringOptimizedNonScalableBloomFilter;
        private HashSet<string> _hashSet;
        private static readonly IByteConverter<string> ByteConverter = new ByteConverterBinaryFormatter<string>();

        [Params(1000)]
        public int ItemsToInsert { get; set; }

        [Params(.02)]
        public double MaximumErrorRate { get; set; }

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
        
        [GlobalSetup(Target = nameof(CheckIfContainsItem_RegularFilter))]
        public void Setup_RegularFilter()
        {
            _regularBloomFilter = new ScalableBloomFilter<string>(MaximumErrorRate, ByteConverter);

            for (var i = 0; i < ItemsToInsert; i++)
            {
                var s = $"string-{i + 1}";
                _regularBloomFilter.Add(s);
            }
        }
        
        [GlobalSetup(Target = nameof(CheckIfContainsItem_OptimizedFilter))]
        public void Setup_OptimizedFilter()
        {
            _genericOptimizedBloomFilter = new GenericOptimizedScalableBloomFilter<string>(MaximumErrorRate, ByteConverter);

            for (var i = 0; i < ItemsToInsert; i++)
            {
                var s = $"string-{i + 1}";
                _genericOptimizedBloomFilter.Add(s);
            }
        }
        
        [GlobalSetup(Target = nameof(CheckIfContainsItem_StringOptimizedFilter))]
        public void Setup_StringOptimizedFilter()
        {
            var byteConverter = new ByteConverterStringMarshal();
            _stringOptimizedBloomFilter = new StringOptimizedScalableBloomFilter(MaximumErrorRate, byteConverter);

            for (var i = 0; i < ItemsToInsert; i++)
            {
                var s = $"string-{i + 1}";
                _stringOptimizedBloomFilter.Add(s);
            }
        }
        
        [GlobalSetup(Target = nameof(CheckIfContainsItem_StringOptimizedNonScalableFilter))]
        public void Setup_StringOptimizedNonScalableFilter()
        {
            var byteConverter = new ByteConverterStringMarshal();
            _stringOptimizedNonScalableBloomFilter = new StringOptimizedBloomfilter(ItemsToInsert, MaximumErrorRate, byteConverter);

            for (var i = 0; i < ItemsToInsert; i++)
            {
                var s = $"string-{i + 1}";
                _stringOptimizedNonScalableBloomFilter.Add(s);
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
        
        [Benchmark(Description = "Regular filter")]
        public void CheckIfContainsItem_RegularFilter()
        {
            for (int i = 0; i < ItemsToInsert; i++)
            {
                var contains = _regularBloomFilter.Contains($"string-{i+1}");
            }
        }     
        
        [Benchmark(Description = "Optimized filter")]
        public void CheckIfContainsItem_OptimizedFilter()
        {
            for (int i = 0; i < ItemsToInsert; i++)
            {
                var contains = _genericOptimizedBloomFilter.Contains($"string-{i+1}");
            }
        }     
        
        [Benchmark(Description = "String optimized filter")]
        public void CheckIfContainsItem_StringOptimizedFilter()
        {
            for (int i = 0; i < ItemsToInsert; i++)
            {
                var contains = _stringOptimizedBloomFilter.Contains($"string-{i+1}");
            }
        }
        
        [Benchmark(Description = "String optimized, non-scalable filter")]
        public void CheckIfContainsItem_StringOptimizedNonScalableFilter()
        {
            for (int i = 0; i < ItemsToInsert; i++)
            {
                var contains = _stringOptimizedNonScalableBloomFilter.Contains($"string-{i+1}");
            }
        }   
    }
}
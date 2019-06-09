using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Order;
using Maybe.BloomFilter;
using Maybe.OptimizedBloomFilter.ByteConverters;

namespace Maybe.Benchmark.ByteSerialization
{
    /// <summary>
    ///     Benchmark the performance differences in using different methods for serializing strings into bytes
    /// </summary>
    [CoreJob]
    [BenchmarkCategory("ByteSerialization", "ObjectType:string")]
    [MemoryDiagnoser]
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [EncodingAttribute.Unicode]
    [CsvExporter(CsvSeparator.Semicolon), RPlotExporter]
    public class StringByteSerializationBenchmarker
    {
        private ScalableBloomFilter<string> _bloomFilter; 
        
        [Params(1000)]
        public int ItemsToInsert { get; set; }

        [Params(.02)]
        public double MaximumErrorRate { get; set; }

        private void GeneralSetup(IByteConverter<string> byteConverter, int itemsToInsert)
        {
            _bloomFilter = new ScalableBloomFilter<string>(MaximumErrorRate, byteConverter);
            for (var i = 0; i < itemsToInsert; i++)
            {
                var s = $"string-{i + 1}";
                _bloomFilter.Add(s);
            }
        }
        
        [GlobalSetup(Target = nameof(BinaryFormatterBenchmark))]
        public void Setup_BinaryFormatter()
        {
            var byteConverter = new ByteConverterBinaryFormatter<string>();
            GeneralSetup(byteConverter, ItemsToInsert);
        }
        
        [Benchmark(Description = "BinaryFormatter", Baseline = true)]
        public void BinaryFormatterBenchmark()
        {
            for (int i = 0; i < ItemsToInsert; i++)
            {
                var contains = _bloomFilter.Contains($"string-{i+1}");
            }
        }
        
        [GlobalSetup(Target = nameof(MessagePackBenchmark))]
        public void Setup_MessagePack()
        {
            var byteConverter = new ByteConverterMessagePack<string>();
            GeneralSetup(byteConverter, ItemsToInsert);
        }
        
        [Benchmark(Description = "MessagePack")]
        public void MessagePackBenchmark()
        {
            for (int i = 0; i < ItemsToInsert; i++)
            {
                var contains = _bloomFilter.Contains($"string-{i+1}");
            }
        }
        
        [GlobalSetup(Target = nameof(MarshalBenchmark))]
        public void Setup_Marshal()
        {
            var byteConverter = new ByteConverterStringMarshal();
            GeneralSetup(byteConverter, ItemsToInsert);
        }
        
        [Benchmark(Description = "Marshal")]
        public void MarshalBenchmark()
        {
            for (int i = 0; i < ItemsToInsert; i++)
            {
                var contains = _bloomFilter.Contains($"string-{i+1}");
            }
        }
        
        [GlobalSetup(Target = nameof(JsonBenchmark))]
        public void Setup_Json()
        {
            var byteConverter = new ByteConverterJson<string>();
            GeneralSetup(byteConverter, ItemsToInsert);
        }
        
        [Benchmark(Description = "JSON")]
        public void JsonBenchmark()
        {
            for (int i = 0; i < ItemsToInsert; i++)
            {
                var contains = _bloomFilter.Contains($"string-{i+1}");
            }
        }
    }
}
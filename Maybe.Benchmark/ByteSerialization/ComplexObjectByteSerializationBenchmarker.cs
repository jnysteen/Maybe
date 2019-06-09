using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Order;
using Maybe.BloomFilter;
using Maybe.OptimizedBloomFilter.ByteConverters;
using MessagePack;

namespace Maybe.Benchmark.ByteSerialization
{
    /// <summary>
    ///     Benchmark the performance differences in using different methods for serializing complex objects into bytes
    /// </summary>
    [CoreJob]
    [BenchmarkCategory("ByteSerialization", "ObjectType:ComplexType")]
    [MemoryDiagnoser]
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [EncodingAttribute.Unicode]
    [CsvExporter(CsvSeparator.Semicolon), RPlotExporter]
    public class ComplexObjectByteSerializationBenchmarker
    {
        private ScalableBloomFilter<ComplexType> _bloomFilter; 
        
        [Params(10000)]
        public int ItemsToInsert { get; set; }

        [Params(.02)]
        public double MaximumErrorRate { get; set; }
        
        public int ChildrenPerInstance = 3;
        public int Depth = 3;
        
        private IEnumerable<ComplexType> CreateComplexTypes()
        {
            return ComplexType.CreateMultiple(ItemsToInsert, ChildrenPerInstance, Depth);
        }

        private void GeneralSetup(IByteConverter<ComplexType> byteConverter)
        {
            _bloomFilter = new ScalableBloomFilter<ComplexType>(0.02, byteConverter);
            foreach (var complexType in CreateComplexTypes())
            {
                _bloomFilter.Add(complexType);
            }
        }
        
        [GlobalSetup(Target = nameof(BinaryFormatterBenchmark))]
        public void Setup_BinaryFormatter()
        {
            var byteConverter = new ByteConverterBinaryFormatter<ComplexType>();
            GeneralSetup(byteConverter);
        }
        
        [Benchmark(Description = "BinaryFormatter", Baseline = true)]
        public void BinaryFormatterBenchmark()
        {
            foreach (var complexType in CreateComplexTypes())
            {
                var contains = _bloomFilter.Contains(complexType);
            }
        }
        
        
        [GlobalSetup(Target = nameof(MessagePackBenchmark))]
        public void Setup_MessagePack()
        {
            var byteConverter = new ByteConverterMessagePack<ComplexType>();
            GeneralSetup(byteConverter);
        }
        
        [Benchmark(Description = "MessagePack")]
        public void MessagePackBenchmark()
        {
            foreach (var complexType in CreateComplexTypes())
            {
                var contains = _bloomFilter.Contains(complexType);
            }
        }
        
        
        [GlobalSetup(Target = nameof(JsonBenchmark))]
        public void Setup_Json()
        {
            var byteConverter = new ByteConverterJson<ComplexType>();
            GeneralSetup(byteConverter);
        }
        
        [Benchmark(Description = "JSON")]
        public void JsonBenchmark()
        {
            foreach (var complexType in CreateComplexTypes())
            {
                var contains = _bloomFilter.Contains(complexType);
            }
        }
    }
    
    [MessagePackObject] // MessagePack requires this
    [Serializable] // The BinaryFormatter requires this
    public class ComplexType
    {
        [Key(0)] // MessagePack requires this
        [DataMember] // The BinaryFormatter *might* require this 
        public int Id { get; set; } // MessagePack requires the getter and setter to be public 

        [Key(1)] // MessagePack requires this
        [DataMember] // The BinaryFormatter *might* require this 
        public IList<ComplexType> Children { get; set; }

        public static ComplexType Create(int childrenPerInstance, int depth)
        {
            var idSupplier = new IdSupplier();
            return Create(idSupplier, childrenPerInstance, depth);
        }
        
        private static ComplexType Create(IdSupplier idSupplier,int childrenPerInstance, int depth)
        {
            var instance = new ComplexType()
            {
                Id = idSupplier.GetId(),
                Children = new List<ComplexType>()
            };

            if (depth == 0)
                return instance;

            for (int i = 0; i < childrenPerInstance; i++)
                instance.Children.Add(Create(idSupplier, childrenPerInstance, depth - 1));
            
            return instance;
        }
        
        public static IEnumerable<ComplexType> CreateMultiple(int instances, int childrenPerInstance, int depth)
        {
            var idSupplier = new IdSupplier();
            for (var i = 0; i < instances; i++)
            {
                yield return Create(idSupplier, childrenPerInstance, depth);
            }
        }

        public override string ToString()
        {
            return $"Id: {Id}";
        }

        private class IdSupplier
        {
            private int _nextId = 1;

            public int GetId()
            {
                return _nextId++;
            }
        }
        
    }
}
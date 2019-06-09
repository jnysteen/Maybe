using System;
using System.Globalization;
using System.Threading;
using BenchmarkDotNet.Running;
using Maybe.Benchmark.AddAndCount;
using Maybe.Benchmark.Contains;

namespace Maybe.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            // Without the line below, parameters with decimals will be outputted with commas in certain cultures
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
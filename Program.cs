using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PerfTest
{
	class Program
	{
		static void Main(string[] args)
		{
			SingleLineTest();
			MultiLineTest();
			SingleLineTest();
			MultiLineTest();
			SingleLineTest();
			MultiLineTest();
		}

		public static void SingleLineTest()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int count = 0;
			for (uint i = 0; i < 1000000000; ++i) {
				//var isMultipleOf16 = i % 16 == 0;
				//count += isMultipleOf16 ? 1 : 0;
				count += i % 16 == 0 ? 1 : 0;
			}
			stopwatch.Stop();
			Console.WriteLine("Single-line test --> Count: {0}, Time: {1}", count, stopwatch.ElapsedMilliseconds);
		}

		public static void MultiLineTest()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int count = 0;
			for (uint i = 0; i < 1000000000; ++i) {
				var isMultipleOf16 = i % 16 == 0;
				count += isMultipleOf16 ? 1 : 0;
				//count += i % 16 == 0 ? 1 : 0;
			}
			stopwatch.Stop();
			Console.WriteLine("Multi-line test  --> Count: {0}, Time: {1}", count, stopwatch.ElapsedMilliseconds);
		}
	}
}

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PerfTest
{
	/// <summary>
	/// Demonstrates that adding a local variable can make .NET code slower, even when optimized.
	/// </summary>
	/// <remarks>
	/// To reproduce the experiment, build this application in both the x64 and x86 configurations, and run both executables from outside VS2010.
	/// The observation has been that for either configuration, SingleLineTest and MultiLineTest show significantly different timings.
	/// Additionally, if you turn off "Suppress JIT optimization on module load" in VS2010, you can put a breakpoint in each "for" block and see that the assembly code varies.
	/// </remarks>
	/// <seealso cref="http://stackoverflow.com/questions/10369421/why-does-adding-local-variables-make-net-code-slower"/>
	class Program
	{
		static void Main()
		{
			// To demonstrate that the JIT order affects performance, uncomment the call to MultiLineTest below.
			// You'll see that SingleLineTest and MultiLineTest now run at the same speed (almost, but not quite, as slow as MultiLineTest before).
			// Next, uncomment the RuntimeHelpers line. You'll see that SingleLineTest speeds back up.
			// Note that this JIT order behavior only applies to x86 (not x64).	
			// See also: http://stackoverflow.com/questions/10406796/why-does-jit-order-affect-performance

			////RuntimeHelpers.PrepareMethod(typeof(Program).GetMethod("SingleLineTest").MethodHandle);
			////MultiLineTest();

			SingleLineTest();
			MultiLineTest();
			SingleLineTest();
			MultiLineTest();
			SingleLineTest();
			MultiLineTest();

			MoreTests.Run();
		}

		public static void SingleLineTest()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int count = 0;
			for (uint i = 0; i < 1000000000; ++i) {
				count += i % 16 == 0 ? 1 : 0;
			}
			stopwatch.Stop();
			Console.WriteLine("Single-line (conditional op) --> Count: {0}, Time: {1}", count, stopwatch.ElapsedMilliseconds);
		}

		public static void MultiLineTest()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int count = 0;
			for (uint i = 0; i < 1000000000; ++i) {
				var isMultipleOf16 = i % 16 == 0;
				count += isMultipleOf16 ? 1 : 0;
			}
			stopwatch.Stop();
			Console.WriteLine("Multi-line  (conditional op) --> Count: {0}, Time: {1}", count, stopwatch.ElapsedMilliseconds);
		}
	}

	/// <summary>
	/// Additional tests to see what happens if the conditional operator (?:) is removed from the loop body.
	/// </summary>
	static class MoreTests
	{
		public static void Run()
		{
			// These tests use an "if" statement.
			IfStatementSingleLineTest();
			IfStatementMultiLineTest();
			IfStatementSingleLineTest();
			IfStatementMultiLineTest();
			IfStatementSingleLineTest();
			IfStatementMultiLineTest();

			// These tests eliminate the condition altogether, and instead use only bit math.
			NoConditionSingleLineTest();
			NoConditionMultiLineTest();
			NoConditionSingleLineTest();
			NoConditionMultiLineTest();
			NoConditionSingleLineTest();
			NoConditionMultiLineTest();
		}

		static void IfStatementSingleLineTest()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			uint count = 0;
			for (uint i = 0; i < 1000000000; ++i) {
				if (i % 16 == 0)
					++count;
			}
			stopwatch.Stop();
			Console.WriteLine("Single-line   (if statement) --> Count: {0}, Time: {1}", count, stopwatch.ElapsedMilliseconds);
		}

		static void IfStatementMultiLineTest()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			uint count = 0;
			for (uint i = 0; i < 1000000000; ++i) {
				var isMultipleOf16 = i % 16 == 0;
				if (isMultipleOf16)
					++count;
			}
			stopwatch.Stop();
			Console.WriteLine("Multi-line    (if statement) --> Count: {0}, Time: {1}", count, stopwatch.ElapsedMilliseconds);
		}

		static void NoConditionSingleLineTest()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			uint count = 0;
			for (uint i = 0; i < 1000000000; ++i) {
				count += ~(i >> 3 | i >> 2 | i >> 1 | i) & 1;
			}
			stopwatch.Stop();
			Console.WriteLine("Single-line   (no condition) --> Count: {0}, Time: {1}", count, stopwatch.ElapsedMilliseconds);
		}

		static void NoConditionMultiLineTest()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			uint count = 0;
			for (uint i = 0; i < 1000000000; ++i) {
				uint isMultipleOf16 = ~(i >> 3 | i >> 2 | i >> 1 | i) & 1;
				count += isMultipleOf16;
			}
			stopwatch.Stop();
			Console.WriteLine("Multi-line    (no condition) --> Count: {0}, Time: {1}", count, stopwatch.ElapsedMilliseconds);
		}
	}
}

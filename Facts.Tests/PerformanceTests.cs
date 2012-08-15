using System;
using System.Diagnostics;
using Facts.Toolbox;
using NUnit.Framework;

namespace Facts.Tests
{
	[TestFixture]
	sealed class PerformanceTests
	{
		sealed class Value
		{
			public int V;
		}

		[Test]
		public static void InPlaceUpdateVersusByHand()
		{
			const int num = 1000000;

			var resUpdates = measure(() => runUpdates(num));
			var resByHand = measure(() => runManual(num));

			var ticksUpdates = (double) resUpdates.Ticks;
			var ticksHand = (double) resByHand.Ticks;

			Debug.WriteLine("update is {0} times slower than manual".format(ticksUpdates / ticksHand));
			Debug.WriteLine("update: {0}, manual: {1}".format(resUpdates, resByHand));
		}

		static void runUpdates(int count)
		{
			var instance = new Value();

			while (count-- != 0)
			{
				instance = instance.update(i => i.V, v => v + 1);
			}
		}

		static void runManual(int count)
		{
			var instance = new Value();
			while (count-- != 0)
			{
				instance = new Value {V = instance.V + 1};
			}
		}


		static TimeSpan measure(Action action)
		{
			var sw = new Stopwatch();
			sw.Start();
			action();
			sw.Stop();
			return sw.Elapsed;
		}
	}
}

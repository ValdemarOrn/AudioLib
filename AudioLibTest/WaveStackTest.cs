using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Drawing;
using AudioLib.Plot;

namespace AudioLib.Test
{
	/*[TestFixture]
	public class WaveStackTest
	{	

		[Test]
		public void TestFindMaxPartials()
		{
			int maxPartials = 64;
			float minFreq = 19000 / 24000.0f;
			int steps = 10000;
			var output = WaveStack.findMaxPartials(maxPartials, minFreq, steps);
		}

		[Test]
		public void TestSetWave()
		{
			var w = new WaveStack();

			var saw = Utils.Saw(2048, 1.0f);
			w.SetWave(saw);

			var list = new List<Line>();
			foreach (var wave in w.Waves)
			{
				Line l = new Line(Utils.Linspace(-1, 1, wave.Length), wave);
				list.Add(l);
			}

			Plot.Plot.ShowPlot(list);
		}

		[Test]
		public void TestGetSample()
		{
			var w = new WaveStack();

			var saw = Utils.Saw(1024, 1.0f);
			w.SetWave(saw);

			w.GetSample(0.1f, 0.499f);
			w.GetSample(0.1f, 0.3f);
		}
	}*/
}

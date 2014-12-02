using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.TF
{
	public sealed class Hp1
	{
		// http://musicdsp.org/archive.php?classid=3#116

		private double fs;
		private double b0, a1;

		public double Output;
		private double lpOut;
		private double cutoffHz;

		public Hp1(float fs)
		{
			this.fs = fs;
		}

		public double Samplerate
		{
			get { return fs; }
			set { fs = value; Update(); }
		}

		public double CutoffHz
		{
			get { return cutoffHz; }
			set { cutoffHz = value; Update(); }
		}

		private void Update()
		{
			// Prevent going over the Nyquist frequency
			if (cutoffHz >= fs * 0.5)
				cutoffHz = fs * 0.499;

			var x = 2 * Math.PI * cutoffHz / fs;
			var nn = (2 - Math.Cos(x));
			var alpha = nn - Math.Sqrt(nn * nn - 1);

			a1 = alpha;
			b0 = 1 - alpha;
		}

		public double Process(double input)
		{
			if (input == 0 && lpOut < 0.000000000001)
			{
				Output = 0;
			}
			else
			{
				lpOut = b0 * input + a1 * lpOut;
				Output = input - lpOut;
			}

			return Output;
		}

		public void Process(double[] input, double[] output, int len)
		{
			for (int i = 0; i < len; i++)
				output[i] = Process(input[i]);
		}
	}
}

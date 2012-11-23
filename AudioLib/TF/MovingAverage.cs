using AudioLib.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.TF
{
	public sealed class MovingAverage : Transfer
	{
		int _samples;
		public int Samples
		{
			get
			{
				return _samples;
			}
			set
			{
				if (value < 1)
					value = 1;
				_samples = value;
				Update();
			}
		}

		public MovingAverage()
		{ }

		public MovingAverage(int samples)
		{
			Samples = samples;
		}

		public override void Update()
		{
			double[] b = new double[Samples];
			double[] a = new double[1];

			a[0] = 1;
			for (int i = 0; i < b.Length; i++)
			{
				b[i] = 1.0 / b.Length;
			}

			this.B = b;
			this.A = a;
		}
	}
}

using AudioLib.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.TF
{
	public sealed class Highpass1 : TransferVariable
	{
		public const int P_FREQ = 0;

		public Highpass1(float fs) : base(fs, 1)
		{ }

		public override void Update()
		{
			double[] b = new double[2];
			double[] a = new double[2];

			// PRevent going over the Nyquist frequency
			if (Parameters[P_FREQ] >= Fs * 0.5)
				Parameters[P_FREQ] = Fs * 0.499;

			// Compensate for frequency in bilinear transform
			float f = (float)(2.0 * Fs * (Math.Tan((Parameters[P_FREQ] * 2 * Math.PI) / (Fs * 2))));
			if (f == 0) f = 0.0001f; // prevent divByZero exception

			b[0] = 2*Fs;
			b[1] = -2*Fs;

			a[0] = f+2*Fs;
			a[1] = f-2*Fs;

			var aInv = 1 / a[0];

			// normalize
			b[0] = b[0] * aInv;
			b[1] = b[1] * aInv;

			a[1] = a[1] * aInv;
			a[0] = 1;

			this.B = b;
			this.A = a;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.TF
{
	/*public sealed class Lp1
	{
		private float fs;
		private float b0, b1, a0, a1;

		private float in0, out0;

		public Lp1(float fs)
		{
			this.fs = fs;
		}

		public void Update(double cutoffHz)
		{
			// Prevent going over the Nyquist frequency
			if (cutoffHz >= fs * 0.5)
				cutoffHz = fs * 0.499;

			// Compensate for frequency in bilinear transform
			var f = (float)(2.0 * fs * (Math.Tan((cutoffHz * 2 * Math.PI) / (fs * 2))));

			// prevent DivByZero exception
			if (f == 0) f = 0.0001f; 

			b0 = f;
			b1 = f;

			a0 = f + 2 * fs;
			a1 = f - 2 * fs;

			var aInv = 1 / a0;

			// normalize
			b0 = b0 * aInv;
			b1 = b1 * aInv;

			a1 = a1 * aInv;
			a0 = 1;
		}

		//public void 
	}*/
}

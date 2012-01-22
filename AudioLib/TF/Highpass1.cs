using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.TF
{
	public class Highpass1 : TransferVariable
	{
		public const int P_FREQ = 0;

		public Highpass1(float fs)
			: base(fs)
		{
			parameters = new float[1];
		}

		public override void Update()
		{
			float[] b = new float[2];
			float[] a = new float[2];

			// PRevent going over the Nyquist frequency
			if (parameters[P_FREQ] >= fs / 2)
				parameters[P_FREQ] = fs / 2 - 100;

			// Compensate for frequency in bilinear transform
			float f = (float)(2.0f*fs*(Math.Tan((parameters[P_FREQ]*2*Math.PI)/(fs*2))));
			if (f == 0) f = 0.0001f; // prevent divByZero exception

			b[0] = -2*fs;
			b[1] = 2*fs;

			a[0] = f-2*fs;
			a[1] = f+2*fs;

			// normalize
			b[0] = b[0] / a[0];
			b[1] = b[1] / a[0];

			a[1] = a[1] / a[0];
			a[0] = 1;

			this.B = b;
			this.A = a;
		}
	}
}

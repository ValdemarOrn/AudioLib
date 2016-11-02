using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioLib.Modules;

namespace AudioLib.TF
{
	public class Butterworth : TransferVariable
	{
		public const int P_CUTOFF_HZ = 0;
		public const int P_ORDER = 1;

		public Butterworth(double fs) : base(fs, 2)
		{ }

		public override void Update()
		{ 
			var cutoffHz = Parameters[P_CUTOFF_HZ];
            var T = 1.0 / Fs;
			var warpedWc = 2 / T * Math.Tan(cutoffHz * 2 * Math.PI * T / 2);
            double[] sb = { 1 };
			double[] sa = null;
			Func<double, double> ScaleFreq = (power) => Math.Pow(1 / warpedWc, power);

			if (Parameters[P_ORDER] == 1)
			{
				sa = new [] { 1.0, ScaleFreq(1) };
			}
			else if (Parameters[P_ORDER] == 2)
			{
				sa = new[] { 1.0, 1.4142 * ScaleFreq(1), 1 * ScaleFreq(2) };
			}
			else if (Parameters[P_ORDER] == 3)
			{
				sa = new[] { 1, 2 * ScaleFreq(1), 2 * ScaleFreq(2), 1 * ScaleFreq(3) };
			}
			else if (Parameters[P_ORDER] == 4)
			{
				sa = new[] { 1, 2.613 * ScaleFreq(1), 3.414 * ScaleFreq(2), 2.613 * ScaleFreq(3), 1 * ScaleFreq(4) };
			}
			else if (Parameters[P_ORDER] == 5)
			{
				sa = new[] { 1, 3.236 * ScaleFreq(1), 5.236 * ScaleFreq(2), 5.236 * ScaleFreq(3), 3.236 * ScaleFreq(4), 1 * ScaleFreq(5) };
			}
			else if (Parameters[P_ORDER] == 6)
			{
				sa = new[] { 1, 3.864 * ScaleFreq(1), 7.464 * ScaleFreq(2), 9.142 * ScaleFreq(3), 7.464 * ScaleFreq(4), 3.864 * ScaleFreq(5), 1 * ScaleFreq(6) };
			}
			else
			{
				throw new Exception("Orders higher than 6 are not supported");
			}

			double[] zb, za;

			Bilinear.Transform(sb, sa, out zb, out za, this.Fs);

			this.B = zb;
			this.A = za;
		}
	}
}

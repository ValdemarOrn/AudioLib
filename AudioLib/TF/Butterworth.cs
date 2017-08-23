using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioLib.Modules;

namespace AudioLib.TF
{
	public class Butterworth : Transfer
    {
	    private double fs;

	    public double CutoffHz { get; set; }
        public new int Order { get; set; }
        public bool Highpass { get; set; }

	    public Butterworth(double fs)
	    {
            this.fs = fs;

	    }

		public override void Update()
		{
		    var zbza = ComputeCoefficients(fs, Highpass, CutoffHz, Order);

            this.B = zbza.Item1;
			this.A = zbza.Item2;
		}

	    public static Tuple<double[], double[]> ComputeCoefficients(double fs, bool isHighpass, double cutoffHz, int order)
	    {
            var T = 1.0 / fs;
            var warpedWc = 2 / T * Math.Tan(cutoffHz * 2 * Math.PI * T / 2);

            // don't go over nyquist, with 10 hz safety buffer
            if (warpedWc >= fs / 2 * 2 * Math.PI)
                warpedWc = (fs - 10) / 2 * 2 * Math.PI;

            double[] sa = null;
            double[] sb = new double[order + 1];
            sb[0] = 1;

            Func<double, double> ScaleFreq = (power) => Math.Pow(1.0 / warpedWc, power);

            if (order == 1)
            {
                sa = new[] { 1.0, ScaleFreq(1) };
            }
            else if (order == 2)
            {
                sa = new[] { 1.0, 1.4142 * ScaleFreq(1), 1 * ScaleFreq(2) };
            }
            else if (order == 3)
            {
                sa = new[] { 1, 2 * ScaleFreq(1), 2 * ScaleFreq(2), 1 * ScaleFreq(3) };
            }
            else if (order == 4)
            {
                sa = new[] { 1, 2.613 * ScaleFreq(1), 3.414 * ScaleFreq(2), 2.613 * ScaleFreq(3), 1 * ScaleFreq(4) };
            }
            else if (order == 5)
            {
                sa = new[] { 1, 3.236 * ScaleFreq(1), 5.236 * ScaleFreq(2), 5.236 * ScaleFreq(3), 3.236 * ScaleFreq(4), 1 * ScaleFreq(5) };
            }
            else if (order == 6)
            {
                sa = new[] { 1, 3.864 * ScaleFreq(1), 7.464 * ScaleFreq(2), 9.142 * ScaleFreq(3), 7.464 * ScaleFreq(4), 3.864 * ScaleFreq(5), 1 * ScaleFreq(6) };
            }
            else
            {
                throw new Exception("Orders higher than 6 are not supported");
            }

            if (isHighpass)
            {
                // When transforming to high pass, we replace all s with 1/s.
                // then we multiply both numerator and denominator with s^order.
                // Since the denom. of a butterworth filter is mirrored, we don't actually need to flip it
                // but when we flip the numerator, we end up with an s^Order, which also needs to be frequency scaled
                //Utils.ReverseInPlace(sa);
                Utils.ReverseInPlace(sb);
                sb[order] = sb[order] * ScaleFreq(order);
            }

            double[] zb, za;

            Bilinear.Transform(sb, sa, out zb, out za, fs);

	        return Tuple.Create(zb, za);
	    }
	}
}

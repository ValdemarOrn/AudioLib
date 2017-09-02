using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AudioLib
{
    public class Freqz
    {
        public class Point
        {
            public double W { get; set; }
            public double Magnitude { get; set; }
            public double Phase { get; set; }
        }

	    public static Point[] Compute(double[] b, double[] a, int points)
	    {
		    var ws = Enumerable.Range(0, points).Select(x => x / (double)points * Math.PI).ToArray();
		    return Compute(b, a, ws);
	    }

	    public static Point[] Compute(double[] b, double[] a, double[] ws)
        {
            var output = new List<Point>();

            for (int i = 0; i < ws.Length; i++)
            {
                var wReal = ws[i];
                var w = Complex.FromPolarCoordinates(1, wReal);
                Complex ww = 1;
                Complex bSum = 0;
                Complex aSum = 0;

                foreach (var bb in b)
                {
                    ww *= w;
                    bSum += bb * ww;
                }

                ww = 1;
                foreach (var aa in a)
                {
                    ww *= w;
                    aSum += aa * ww;
                }

                var div = (bSum / aSum);
                var mag = div.Magnitude;
                var ang = div.Phase;

                output.Add(new Point { W = wReal, Magnitude = mag, Phase = ang });
            }

            return output.ToArray();
        }
    }
}

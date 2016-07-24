using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.SplineLut
{
	public class BezierPoint
	{
		public double X;
		public double Y;

		public double C1x;
		public double C1y;
		public double C2x;
		public double C2y;

		public PointType PointType;
	}

	public enum PointType
	{
		Fixed, // control points mirror around center
		Uneven, // same derivative, different magnitude
		Free // completely independent points
	}

	public class BezierLookup
	{
        public double Bias;

		private double[] xs;
		private double[] ys;
		private double[] ks;
        private double max;
        private double min;

        public BezierLookup()
        {
            var data = JsonConvert.DeserializeObject<double[][]>(jsonData);
            SetData(data[0], data[1], data[2]);
        }

        /// <summary>
        /// Arrays must be sorted from lowest x value to highest
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="ks"></param>
        public SplineInterpolator(double[] xs, double[] ys, double[] ks)
		{
            SetData(xs, ys, ks);
		}

        private void SetData(double[] xs, double[] ys, double[] ks)
        {
            this.xs = xs;
            this.ys = ys;
            this.ks = ks;
            this.max = xs.Max();
            this.min = xs.Min();
        }

        public void ProcessInPlace(double[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                var x = inputs[i] + Bias;
                if (x > max) x = max - 0.00001;
                if (x < min) x = min + 0.00001;
                var y = InterpolateNoBias(x);
                inputs[i] = y;
            }
        }

		public double InterpolateNoBias(double x)
		{
			var i = 1;
			while (xs[i] < x)
				i++;
			
			var t = (x - xs[i - 1]) / (xs[i] - xs[i - 1]);
			
			var a = ks[i - 1] * (xs[i] - xs[i - 1]) - (ys[i] - ys[i - 1]);
			var b = -ks[i] * (xs[i] - xs[i - 1]) + (ys[i] - ys[i - 1]);

			var q = (1 - t) * ys[i - 1] + t * ys[i] + t * (1 - t) * (a * (1 - t) + b * t);
			return q;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public class Interpolate
	{
		public static double Linear(double position, double[] data)
		{
			double pos = position * (data.Length - 1);
			double y1, y2;
			double mu;
			double output;

			int x0 = (int)pos;
			if (x0 == data.Length - 1)
				return data[x0];

			mu = pos - x0;

			y1 = data[x0];
			y2 = data[x0 + 1];

			output = y1 * (1 - mu) + y2 * mu;

			return output;
		}

		public static double Cosine(double position, double[] data)
		{
			double pos = position * (data.Length - 1);
			double y1, y2;
			double mu;
			double output;

			int x0 = (int)pos;
			if (x0 == data.Length - 1)
				return data[x0];

			mu = pos - x0;

			y1 = data[x0];
			y2 = data[x0 + 1];

			double mu2;

			mu2 = (double)((1 - Math.Cos(mu * Math.PI)) / 2);
			output = (y1 * (1 - mu2) + y2 * mu2);

			return output;
		}


		public static double Cubic(double position, double[] data)
		{
			double pos = position * (data.Length - 1);
			double y0, y1, y2, y3;
			double a0, a1, a2, a3, mu;
			double output;

			int x0 = (int)pos - 1;
			mu = pos - x0 - 1;

			// If pos is an integer, or close to it, return exact index
			if (mu == 0.0f)
			{
				output = data[x0 + 1];
				return output;
			}

			// Find x0
			if (x0 < 0)
				y0 = data[x0 + 1];
			else
				y0 = data[x0];

			y1 = data[x0 + 1];
			y2 = data[x0 + 2];

			// Find x3
			if (x0 + 3 > (data.Length - 1))
				y3 = data[x0 + 2];
			else
				y3 = data[x0 + 3];

			double mu2 = mu * mu;
			a0 = y3 - y2 - y0 + y1;
			a1 = y0 - y1 - a0;
			a2 = y2 - y0;
			a3 = y1;

			output = (a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3);
			return output;
		}

		public static double CubicWrap(double position, double[] data)
		{
			double pos = position * (data.Length - 1);
			double y0, y1, y2, y3;
			double a0, a1, a2, a3, mu;
			double output;

			int x0 = (int)pos - 1;
			mu = pos - x0 - 1;

			// If pos is an integer, or close to it, return exact index
			if (mu == 0.0f)
			{
				output = data[x0 + 1];
				return output;
			}

			// Find x0
			if (x0 < 0)
				y0 = data[data.Length - 1];
			else
				y0 = data[x0];

			y1 = data[x0 + 1];
			y2 = data[x0 + 2];

			// Find x3
			if (x0 + 3 > (data.Length - 1))
				y3 = data[0];
			else
				y3 = data[x0 + 3];

			double mu2 = mu * mu;
			a0 = y3 - y2 - y0 + y1;
			a1 = y0 - y1 - a0;
			a2 = y2 - y0;
			a3 = y1;

			output = (a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3);
			return output;
		}

		public static double Spline(double position, double[] data)
		{
			double pos = position * (data.Length - 1);
			double y0, y1, y2, y3;
			double a,b,c, mu;
			double output;

			int x0 = (int)pos - 1;
			mu = pos - x0 - 1;

			// If pos is an integer, or close to it, return exact index
			if (mu == 0.0f)
			{
				output = data[x0 + 1];
				return output;
			}

			// Find x0
			if (x0 < 0)
				y0 = data[x0 + 1];
			else
				y0 = data[x0];

			y1 = data[x0 + 1];
			y2 = data[x0 + 2];

			// Find x3
			if (x0 + 3 > (data.Length - 1))
				y3 = data[x0 + 2];
			else
				y3 = data[x0 + 3];

			// Spline 1
			c = y1;
			a = (y2 + y0) * 0.5f - y1;
			b = (y2 - y0) * 0.5f;
			double v1 = a * mu * mu + b * mu + c;

			// Spline 2
			c = y2;
			a = (y3 + y1) / 2 - y2;
			b = (y3 - y1) / (2);
			double mmu = (mu - 1);
			double v2 = a * mmu * mmu + b * mmu + c;

			output = v1 * (1 - mu) + v2 * mu;
			return output;
		}

		public static double SplineWrap(double position, double[] data)
		{
			double pos = position * (data.Length - 1);
			double y0, y1, y2, y3;
			double a, b, c, mu;
			double output;

			int x0 = (int)pos - 1;
			mu = pos - x0 - 1;

			// If pos is an integer, or close to it, return exact index
			if (mu == 0.0f)
			{
				output = data[x0 + 1];
				return output;
			}

			// Find x0
			if (x0 < 0)
				y0 = data[data.Length - 1];
			else
				y0 = data[x0];

			y1 = data[x0 + 1];
			y2 = data[x0 + 2];

			// Find x3
			if (x0 + 3 > (data.Length - 1))
				y3 = data[0];
			else
				y3 = data[x0 + 3];

			// Spline 1
			c = y1;
			a = (y2 + y0) * 0.5f - y1;
			b = (y2 - y0) * 0.5f;
			double v1 = a * mu * mu + b * mu + c;

			// Spline 2
			c = y2;
			a = (y3 + y1) / 2 - y2;
			b = (y3 - y1) / (2);
			double mmu = (mu - 1);
			double v2 = a * mmu * mmu + b * mmu + c;

			output = v1 * (1 - mu) + v2 * mu;
			return output;
		}
	}
}

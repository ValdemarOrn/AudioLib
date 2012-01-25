using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLib
{
	/**
	 * Takes data on the form
	 * b3*s^3 + b2*s^2 + b1*s + b0
	 * ---------------------------  
	 * a3*s^3 + a2*s^2 + a1*s + a0
	 * Warning: b[0] = b0, b[1] = b1, etc, so make sure array is not reversed
	 * 
	 */
	public class Bilinear
	{
		public static void transform(double[] b, double[] a, out double[] zb, out double[] za, double fs)
		{
			// Make sure the lengths of a and b are equal, otherwise the transform breaks
			// If one array is shorten than the other, we create a new one and pad it with zeros
			if(b.Length > a.Length)
			{
				double[] newA = new double[b.Length];
				for(int i=0; i < b.Length; i++)
				{
					if(i<a.Length)
						newA[i] = a[i];
					else
						newA[i] = 0;
				}
				a = newA;
			}
			else if(a.Length > b.Length)
			{
				double[] newB = new double[a.Length];
				for(int i=0; i<a.Length; i++)
				{
					if(i<b.Length)
						newB[i] = b[i];
					else
						newB[i] = 0;
				}
				b = newB;
			}

			// a.len = b.len!

			za = new double[a.Length];
			zb = new double[a.Length];

			if(b.Length == 2)
				Bilinear.sToZ1(b, a, zb, za, fs);
			else if(b.Length == 3)
				Bilinear.sToZ2(b, a, zb, za, fs);
			else if(b.Length == 4)
				Bilinear.sToZ3(b, a, zb, za, fs);
			else if(b.Length == 5)
				Bilinear.sToZ4(b, a, zb, za, fs);
			else
				Bilinear.supertransform(b, a, zb, za, fs);
		}


		private static void sToZ1(double[] b, double[] a, double[] zb, double[] za, double fs)
		{
			zb[1] = b[0] - 2 * b[1] * fs;	//z^0
			zb[0] = b[0] + 2 * b[1] * fs;	//z^1

			za[1] = a[0] - 2 * a[1] * fs;	//z^0
			za[0] = a[0] + 2 * a[1] * fs;	//z^1
		}

		private static void sToZ2(double[] b, double[] a, double[] zb, double[] za, double fs)
		{
			double fs2 = fs * fs;

			zb[2] = b[0] - 2 * b[1] * fs + 4 * b[2] * fs2;	//z^0
			zb[1] = 2 * b[0] - 8 * b[2] * fs2;	//z^1
			zb[0] = b[0] + 2 * b[1] * fs + 4 * b[2] * fs2;	//z^2

			za[2] = a[0] - 2 * a[1] * fs + 4 * a[2] * fs2;	//z^0
			za[1] = 2 * a[0] - 8 * a[2] * fs2;	//z^1
			za[0] = a[0] + 2 * a[1] * fs + 4 * a[2] * fs2;	//z^2
		}

		private static void sToZ3(double[] b, double[] a, double[] zb, double[] za, double fs)
		{
			double fs2 = fs * fs;
			double fs3 = fs * fs * fs;


			zb[3] = b[0] - 2 * b[1] * fs + 4 * b[2] * fs2 - 8 * b[3] * fs3;	//z^0
			zb[2] = 3 * b[0] - 2 * b[1] * fs - 4 * b[2] * fs2 + 24 * b[3] * fs3;	//z^1
			zb[1] = 3 * b[0] + 2 * b[1] * fs - 4 * b[2] * fs2 - 24 * b[3] * fs3;	//z^2
			zb[0] = b[0] + 2 * b[1] * fs + 4 * b[2] * fs2 + 8 * b[3] * fs3;	//z^3

			za[3] = a[0] - 2 * a[1] * fs + 4 * a[2] * fs2 - 8 * a[3] * fs3;	//z^0
			za[2] = 3 * a[0] - 2 * a[1] * fs - 4 * a[2] * fs2 + 24 * a[3] * fs3;	//z^1
			za[1] = 3 * a[0] + 2 * a[1] * fs - 4 * a[2] * fs2 - 24 * a[3] * fs3;	//z^2
			za[0] = a[0] + 2 * a[1] * fs + 4 * a[2] * fs2 + 8 * a[3] * fs3;	//z^3
		}

		private static void sToZ4(double[] b, double[] a, double[] zb, double[] za, double fs)
		{
			double fs2 = fs * fs;
			double fs3 = fs * fs * fs;
			double fs4 = fs * fs * fs * fs;

			zb[4] = b[0] - 2 * b[1] * fs + 4 * b[2] * fs2 - 8 * b[3] * fs3 + 16 * b[4] * fs4;	//z0
			zb[3] = 4 * b[0] - 4 * b[1] * fs + 16 * b[3] * fs3 - 64 * b[4] * fs4;	//z1
			zb[2] = 6 * b[0] - 8 * b[2] * fs2 + 96 * b[4] * fs4;	//z2
			zb[1] = 4 * b[0] + 4 * b[1] * fs - 16 * b[3] * fs3 - 64 * b[4] * fs4;	//z3
			zb[0] = b[0] + 2 * b[1] * fs + 4 * b[2] * fs2 + 8 * b[3] * fs3 + 16 * b[4] * fs4;	//z4

			za[4] = a[0] - 2 * a[1] * fs + 4 * a[2] * fs2 - 8 * a[3] * fs3 + 16 * a[4] * fs4;	//z0
			za[3] = 4 * a[0] - 4 * a[1] * fs + 16 * a[3] * fs3 - 64 * a[4] * fs4;	//z1
			za[2] = 6 * a[0] - 8 * a[2] * fs2 + 96 * a[4] * fs4;	//z2
			za[1] = 4 * a[0] + 4 * a[1] * fs - 16 * a[3] * fs3 - 64 * a[4] * fs4;	//z3
			za[0] = a[0] + 2 * a[1] * fs + 4 * a[2] * fs2 + 8 * a[3] * fs3 + 16 * a[4] * fs4;	//z4
		}

		private static void supertransform(double[] b, double[] a, double[] zb, double[] za, double fs)
		{
			int numOfCoeffs = b.Length;
			int order = numOfCoeffs - 1;

			double[] zplus1 = { 1.0f, 1.0f };
			double[] zminus1 = { 1.0f, -1.0f };

			double[][] polys = new double[numOfCoeffs][];

			for(int i=0; i<numOfCoeffs; i++)
			{
				polys[i] = Conv.conv(arrayPower(zplus1, order-i) , arrayPower(zminus1, i));
			}

			for(int i=0; i<numOfCoeffs; i++)
			{
				zb[i] = 0;
				za[i] = 0;
				for(int j=0; j<numOfCoeffs; j++)
				{
					zb[i] += (double)(polys[j][i]*b[j]*Math.Pow(2*fs, j));
					za[i] += (double)(polys[j][i]*a[j]*Math.Pow(2*fs, j));
				}

			}
		}

		// 

		/// <summary>
		/// Calculates the power of a polynomial by convoluting its coefficients with it self n-times
		/// </summary>
		/// <param name="a">The coefficients of the polynomial</param>
		/// <param name="n">The power to which to raise the polynomial</param>
		/// <returns>The resulting polynomial coefficients after raising the poly to power-of-n</returns>
		public static double[] arrayPower(double[] a, int n)
		{
			double[] output = { 1f };
			for (int i = 0; i < n; i++)
				output = Conv.conv(output, a);

			return output;
		}
	}
}

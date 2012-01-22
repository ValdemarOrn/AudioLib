using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public class SimpleDFT
	{
		/// <summary>
		/// Provides the Discrete Fourier Transform for a real-valued input signal
		/// </summary>
		/// <param name="input">the signal to transform</param>
		/// <param name="partials">the maximum number of partials to calculate. If not value is given it defaults to input/2</param>
		/// <returns>The Cos and Sin components of the signal, respectively</returns>
		public static List<double[]> DFT(double[] input, int partials = 0)
		{
			int len = input.Length;
			double[] cosDFT = new double[len / 2 + 1];
			double[] sinDFT = new double[len / 2 + 1];

			if (partials == 0)
				partials = len / 2;

			for (int n = 0; n <= partials; n++)
			{
				double cos = 0.0;
				double sin = 0.0;

				for (int i = 0; i < len; i++)
				{
					cos += input[i] * Math.Cos(2 * Math.PI * n / len * i);
					sin += input[i] * Math.Sin(2 * Math.PI * n / len * i);
				}

				cosDFT[n] = (double)cos;
				sinDFT[n] = (double)sin;
			}

			return new List<double[]>() { cosDFT, sinDFT };
		}

		/// <summary>
		/// Takes the real-valued Cos and Sin components of Fourier transformed signal and reconstructs the time-domain signal
		/// </summary>
		/// <param name="cos">Array of cos components, containing frequency components from 0 to pi. sin.Length must match cos.Length</param>
		/// <param name="sin">Array of sin components, containing frequency components from 0 to pi. sin.Length must match cos.Length</param>
		/// <param name="len">
		/// The length of the output signal. 
		/// If len < (partials-1)*2 then frequency data will be lost in the output signal. 
		/// if no len parameter is given it defaults to (partials-1)*2
		/// </param>
		/// <returns>the real-valued time-domain signal</returns>
		public static double[] IDFT(double[] cos, double[] sin, int len = 0)
		{
			if (cos.Length != sin.Length) throw new ArgumentException("cos.Length and sin.Length bust match!");

			if (len == 0)
				len = (cos.Length - 1) * 2;

			double[] output = new double[len];

			int partials = (sin.Length - 1);
			if (partials > len / 2)
				partials = len / 2;

			for (int n = 0; n <= partials; n++)
			{
				var cos1 = cos[n];
				var sin1 = sin[n];

				var cos2 = cos1;
				var sin2 = -sin1;

				if (n == 0 || n == partials)
				{
					cos2 = 0;
					sin2 = 0;
				}

				double tempcos1;
				double tempsin1;
				double tempcos2;
				double tempsin2;

				for (int i = 0; i < len; i++)
				{
					tempcos1 = Math.Cos(2 * Math.PI * n / ((double)len) * i);
					tempsin1 = Math.Sin(2 * Math.PI * n / ((double)len) * i);

					tempcos2 = Math.Cos(2 * Math.PI * (len-n) / ((double)len) * i);
					tempsin2 = Math.Sin(2 * Math.PI * (len-n) / ((double)len) * i);

					output[i] += tempcos1 * cos1 + tempcos2 * cos2 + tempsin1 * sin1 + tempsin2 * sin2;
				}
			}

			double[] output2 = new double[output.Length];
			double normal = 1.0 / output.Length;
			for (int i = 0; i < len; i++)
			{
				output2[i] = (double)output[i] * normal;
			}

			return output2;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public sealed class SimpleDFT
	{
		/// <summary>
		/// Provides the Discrete Fourier Transform for a real-valued input signal
		/// </summary>
		/// <param name="input">the signal to transform</param>
		/// <param name="partials">the maximum number of partials to calculate. If no value is given it defaults to input.length / 2</param>
		/// <returns>The Cos and Sin components of the signal, respectively</returns>
		public static Tuple<double[], double[]> DFT(double[] input, int partials = 0)
		{
			int len = input.Length;

			if (partials == 0)
				partials = len / 2;

			double[] cosDFT = new double[partials + 1];
			double[] sinDFT = new double[partials + 1];

			bool odd = input.Length % 2 != 0;

			double power;
			if (odd && partials == len / 2)
				power = 1.0 / (input.Length / 2 + 0.5);
			else
				power = 1.0 / (input.Length / 2);

			for (int n = 0; n <= partials; n++)
			{
				double cos = 0.0;
				double sin = 0.0;

				for (int i = 0; i < len; i++)
				{
					cos += input[i] * Math.Cos(2 * Math.PI * n / (double)len * i);
					sin += input[i] * Math.Sin(2 * Math.PI * n / (double)len * i);
				}

				if (n == 0 || n == partials && !odd) // halve the power of the last partial
				{
					cosDFT[n] = cos * power * 0.5;
					sinDFT[n] = sin * power * 0.5;
				}
				else
				{
					cosDFT[n] = cos * power;
					sinDFT[n] = sin * power;
				}
			}

			return ToPolar(cosDFT, sinDFT);
		}

		private static Tuple<double[], double[]> ToPolar(double[] cos, double[] sin)
		{
			var length = new double[cos.Length];
			var phase = new double[cos.Length];

			for(int i = 0; i<length.Length; i++)
			{
				length[i] = Math.Sqrt(cos[i] * cos[i] + sin[i] * sin[i]);
				phase[i] = Math.Atan2(sin[i], cos[i]);
			}

			return new Tuple<double[], double[]>(length, phase);
		}

		/// <summary>
		/// Adds up all the waves to create a new waveform
		/// </summary>
		/// <param name="partials">Tuple containing length and phase components</param>
		/// <param name="len">The length of the output signal.</param>
		/// <param name="maxPartials">Limit the reconstructed wave to this many partials</param>
		/// <returns>the real-valued time-domain signal</returns>
		public static double[] IDFT(Tuple<double[], double[]> partials, int len, int maxPartials = 0)
		{
			var length = partials.Item1;
			var phase = partials.Item2;

			if (maxPartials == 0)
				maxPartials = phase.Length - 1;

			if (phase.Length != phase.Length)
				throw new ArgumentException("length and phase arrays must have same length");

			double[] output = new double[len];
			double lenDouble = len;

			for (int k = 0; k < phase.Length; k++)
			{
				if (k > maxPartials)
					break;

				for (int i = 0; i < len; i++)
				{
					output[i] += Math.Cos(i / lenDouble * k * 2 * Math.PI - phase[k]) * length[k];
				}
			}

			return output;
		}
	}
}
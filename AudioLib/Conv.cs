using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLib
{
	public class Conv
	{
		private double[] response;
		private double[] tail;

		public Conv(double[] responseIn)
		{
			this.response = responseIn;

			this.tail = new double[this.response.Length - 1];
		}

		public void setResponse(double[] inResponse)
		{
			this.response = inResponse;
			this.tail = new double[this.response.Length - 1];
		}

		/**
		 * Process an incoming signal with the defined impulse response
		 * @param input Input signal to be processed
		 * @return processed signal <b>without</b> the "tail". output.length = input.length
		 */
		public double[] process(double[] input)
		{
			double[] data = Conv.conv(this.response, input);
			double[] output = new double[input.Length];
			double[] newTail = new double[tail.Length];

			int i = 0;
			while (i < data.Length)
			{
				if (i < output.Length)
				{
					// Ef data liggur á bilinu 0...output length, færa gögn í output
					output[i] = data[i];

					if (i < tail.Length)
						output[i] += tail[i];
				}
				else
				{
					// Ef data liggur út fyrir output þá færa restina í nýtt tail
					newTail[i - output.Length] = data[i];

					if (i < tail.Length) // Ef það er eitthvað eftir af gamla teilinu bæta því við
						newTail[i - output.Length] += tail[i];
				}

				i++;
			}
			this.tail = newTail;

			return output;
		}

		public static double[] conv(double[] h, double[] g)
		{
			double[] output = new double[h.Length + g.Length - 1];

			// To minimize the number of MAC-operations, split the loop in two
			// parts, use different algorithms on each side
			int lg = g.Length;
			int lh = h.Length;
			for (int i = 0; i < output.Length; i++)
			{
				for (int j = 0; j <= i; j++)
				{
					if (j < lh && (i - j) < lg)
						output[i] += h[j] * g[i - j];
				}

				// System.out.println(""+output[i]);
			}

			return output;
		}


		/// <summary>
		/// PErforms a circular convolution returning data.length samples
		/// Useful for brickwall sinc filters
		/// </summary>
		/// <param name="data"></param>
		/// <param name="kernel"></param>
		/// <returns></returns>
		public static double[] ConvSimpleCircular(double[] data, double[] kernel)
		{
			int M = (kernel.Length - 1) / 2;
			// Note: kernel[M] is the midpoint of filter ;)


			// moduloConstant is a constant to add to the modulo operation to prevent negative indexes
			// must be a multiple of data.length and larger than the filter kernel
			int moduloConstant = data.Length;
			while (moduloConstant < kernel.Length)
				moduloConstant += data.Length;

			var output = new double[data.Length];

			for (int i = 0; i < data.Length; i++)
			{
				double sample = data[i] * kernel[M];

				for (int j = 1; j < (kernel.Length - 1) / 2; j++)
				{
					sample += data[(i + j) % data.Length] * kernel[M + j] + data[(i - j + moduloConstant) % data.Length] * kernel[M - j];
				}

				output[i] = sample;
			}

			return output;
		}
	}
}

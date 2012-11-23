using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLib.Modules
{
	/**
	 * A basic class representing a transfer function.
	 * To alter the TF method update() should be overloaded to calculate
	 * the correct response. If variable comonents are used then setParam() can be used
	 * to set the value, and their values then used in update() to calculate the
	 * correct response.
	 * 
	 * b[0]*z^3 + b[1]*z^2 + b[2]*z + b[3]   b[0] + b[1]*z^-1 + b[2]*z^-2 + b[3]*z^-3
	 * ----------------------------------- = -----------------------------------------
	 * a[0]*z^3 + a[1]*z^2 + a[2]*z + a[3]   a[0] + a[1]*z^-1 + a[2]*z^-2 + a[3]*z^-3
	 * 
	 */
	public class Transfer
	{
		public int Order
		{
			get { return (b.Length > a.Length) ? b.Length - 1 : a.Length - 1; }
		}

		public double[] B
		{
			get { return b; }
			set 
			{
				if (value == null) throw new ArgumentNullException();
				b = value;
			}
		}

		public double[] A
		{
			get { return a; }
			set
			{
				if (value == null) throw new ArgumentNullException();

				a = value;
				if (a[0] == 0.0)
					Gain = 0.0;
				else
					Gain = 1 / a[0];
			}
		}

		private double[] b;
		private double[] a;

		private double[] bufIn = new double[50];
		private double[] bufOut = new double[50];

		private double Gain;

		// buffer size
		private int modulo = 50;
		private int index = 0;

		public Transfer()
		{
			b = new double[1];
			a = new double[1];

			B = new double[1];
			A = new double[1];
		}

		/// <summary>
		/// This method should be overloaded, it is used to calculate the
		/// coefficients of the filter
		/// </summary>
		public virtual void Update()
		{

		}

		public double Process(double input)
		{
			index = (index + 1) % modulo;

			bufIn[index] = input;
			bufOut[index] = 0;

			for (int j = 0; j < b.Length; j++)
				bufOut[index] += (b[j] * bufIn[((index - j) + modulo) % modulo]);

			for (int j = 1; j < a.Length; j++)
				bufOut[index] -= (a[j] * bufOut[((index - j) + modulo) % modulo]);

			bufOut[index] = bufOut[index] * Gain;

			var output = bufOut[index];
			return output;
		}

		public double[] Process(double[] input)
		{
			double[] output = new double[input.Length];
			Process(input, output);
			return output;
		}

		public void ProcessInPlace(double[] input)
		{
			Process(input, input);
		}

		public void Process(double[] input, double[] output)
		{
			if(true && a.Length == 2)
			{
				for (int i = 0; i < input.Length; i++)
				{
					index = (index + 1) % modulo;

					bufIn[index] = input[i];
					bufOut[index] = 0;

					bufOut[index] =
						  (b[0] * bufIn[((index) + modulo) % modulo])
						+ (b[1] * bufIn[((index - 1) + modulo) % modulo])
						- (a[1] * bufOut[((index - 1) + modulo) % modulo]);

					bufOut[index] = bufOut[index] * Gain;

					output[i] = bufOut[index];
				}
			}
			else if(a.Length == 3)
			{
				for (int i = 0; i < input.Length; i++)
				{
					index = (index + 1) % modulo;

					bufIn[index] = input[i];
					bufOut[index] = 0;

					bufOut[index] =
						  (b[0] * bufIn[((index) + modulo) % modulo])
						+ (b[1] * bufIn[((index - 1) + modulo) % modulo])
						+ (b[2] * bufIn[((index - 2) + modulo) % modulo])
						- (a[1] * bufOut[((index - 1) + modulo) % modulo])
						- (a[2] * bufOut[((index - 2) + modulo) % modulo]);

					bufOut[index] = bufOut[index] * Gain;

					output[i] = bufOut[index];
				}
			}
			else if (a.Length == 4)
			{
				for (int i = 0; i < input.Length; i++)
				{
					index = (index + 1) % modulo;

					bufIn[index] = input[i];
					bufOut[index] = 0;

					bufOut[index] =
						  (b[0] * bufIn[((index) + modulo) % modulo])
						+ (b[1] * bufIn[((index - 1) + modulo) % modulo])
						+ (b[2] * bufIn[((index - 2) + modulo) % modulo])
						+ (b[3] * bufIn[((index - 3) + modulo) % modulo])
						- (a[1] * bufOut[((index - 1) + modulo) % modulo])
						- (a[2] * bufOut[((index - 2) + modulo) % modulo])
						- (a[3] * bufOut[((index - 3) + modulo) % modulo]);

					bufOut[index] = bufOut[index] * Gain;

					output[i] = bufOut[index];
				}
			}
			else
			{
				var bLen = b.Length;
				var aLen = a.Length;

				for (int i = 0; i < input.Length; i++)
				{
					index = (index + 1) % modulo;

					bufIn[index] = input[i];
					bufOut[index] = 0;

					for (int j = 0; j < bLen; j++)
						bufOut[index] += (b[j] * bufIn[((index - j) + modulo) % modulo]);

					for (int j = 1; j < aLen; j++)
						bufOut[index] -= (a[j] * bufOut[((index - j) + modulo) % modulo]);

					bufOut[index] = bufOut[index] * Gain;

					output[i] = bufOut[index];
				}
			}
		}
	}
}

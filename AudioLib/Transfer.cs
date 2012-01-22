using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLib
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

		public float[] B
		{
			get { return b; }
			set 
			{
				if (value == null) throw new ArgumentNullException();
				b = value;
				bufIn = new float[Order + 2];
				bufOut = new float[Order + 2];
				p = 0;
			}
		}

		public float[] A
		{
			get { return a; }
			set
			{
				if (value == null) throw new ArgumentNullException();
				a = value;
				bufIn = new float[Order + 2];
				bufOut = new float[Order + 2];
				p = 0;
			}
		}
	
		private float[] b;
		private float[] a;

		private float[] bufIn;
		private float[] bufOut;

		private int p = 0;

		public Transfer()
		{
			b = new float[1];
			a = new float[1];

			B = new float[1];
			A = new float[1];
		}

		/// <summary>
		/// This method should be overloaded, it is used to calculate the
		/// coefficients of the filter
		/// </summary>
		public virtual void Update()
		{

		}

		public virtual float[] process(float[] input)
		{
			float[] output = new float[input.Length];
			process(input, output);
			return output;
		}

		public virtual void process(float[] input, float[] output)
		{
			if(input.Length > output.Length)
				return;

			for(int i=0; i < input.Length; i++)
			{
				// buffer size
				var mod = bufIn.Length;

				p = (p+1) % mod;

				bufIn[p] = input[i];
				bufOut[p] = 0;

				for(int j = 0; j < b.Length; j++)
					bufOut[p] += b[j] * bufIn[((p - j) + mod) % mod];

				for(int j = 1; j < a.Length; j++)
					bufOut[p] -= a[j] * bufOut[((p - j) + mod) % mod];

				if (a[0] != 1.0f && a[0] != 0)
					bufOut[p] = bufOut[p] / a[0];

				output[i] = bufOut[p];
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	/// <summary>
	/// Class containing lookup tables for common functions.
	/// Useful where precision is not vital, but speed is
	/// </summary>
	public sealed class ValueTables
	{
		public static double[] Sqrt;
		public static double[] Sqrt3;
		public static double[] Pow1_5;
		public static double[] Pow2;
		public static double[] Pow3;
		public static double[] Pow4;
		public static double[] x2Pow3;

		// octave response. value double every step (2,3,4,5 or 6 steps)
		public static double[] Response2Oct;
		public static double[] Response3Oct;
		public static double[] Response4Oct;
		public static double[] Response5Oct;
		public static double[] Response6Oct;

		// decade response, value multiplies by 10 every step
		public static double[] Response2Dec;
		public static double[] Response3Dec;
		public static double[] Response4Dec;

		static ValueTables()
		{
			Sqrt = new double[4001];
			Sqrt3 = new double[4001];
			Pow1_5 = new double[4001];
			Pow2 = new double[4001];
			Pow3 = new double[4001];
			Pow4 = new double[4001];
			x2Pow3 = new double[4001];

			Response2Oct = new double[4001];
			Response3Oct = new double[4001];
			Response4Oct = new double[4001];
			Response5Oct = new double[4001];
			Response6Oct = new double[4001];

			Response2Dec = new double[4001];
			Response3Dec = new double[4001];
			Response4Dec = new double[4001];

			for(int i = 0; i <= 4000; i++)
			{
				double x = i / 4000.0;

				Sqrt[i] = Math.Sqrt(x);
				Sqrt3[i] = Math.Pow(x, 1.0 / 3.0);
				Pow1_5[i] = Math.Pow(x, 1.5);
				Pow2[i] = Math.Pow(x, 2.0);
				Pow3[i] = Math.Pow(x, 3.0);
				Pow4[i] = Math.Pow(x, 4.0);

				x2Pow3[i] = Math.Pow(2*x, 3.0);
				Response2Oct[i] = (Math.Pow(4, x) - 1.0) / 4.0 + 0.25;
				Response3Oct[i] = (Math.Pow(8, x) - 1.0) / 8.0 + 0.125;
				Response4Oct[i] = (Math.Pow(16, x) - 1.0) / 16.0 + 0.125 / 2.0;
				Response5Oct[i] = (Math.Pow(32, x) - 1.0) / 32.0 + 0.125 / 4.0;
				Response6Oct[i] = (Math.Pow(64, x) - 1.0) / 64.0 + 0.125 / 8.0;

				Response2Dec[i] = Math.Pow(100, x) / 100.0;
				Response3Dec[i] = Math.Pow(1000, x) / 1000.0;
				Response4Dec[i] = Math.Pow(10000, x) / 10000.0;
			}

			for (int i = 1; i <= 4000; i++)
			{
				Response2Oct[i] = (Response2Oct[i] - Response2Oct[0]) / (1 - Response2Oct[0]);
				Response3Oct[i] = (Response3Oct[i] - Response3Oct[0]) / (1 - Response3Oct[0]);
				Response4Oct[i] = (Response4Oct[i] - Response4Oct[0]) / (1 - Response4Oct[0]);
				Response5Oct[i] = (Response5Oct[i] - Response5Oct[0]) / (1 - Response5Oct[0]);
				Response6Oct[i] = (Response6Oct[i] - Response6Oct[0]) / (1 - Response6Oct[0]);
				Response2Dec[i] = (Response2Dec[i] - Response2Dec[0]) / (1 - Response2Dec[0]);
				Response3Dec[i] = (Response3Dec[i] - Response3Dec[0]) / (1 - Response3Dec[0]);
				Response4Dec[i] = (Response4Dec[i] - Response4Dec[0]) / (1 - Response4Dec[0]);
			}

			Response2Oct[0] = 0;
			Response3Oct[0] = 0;
			Response4Oct[0] = 0;
			Response5Oct[0] = 0;
			Response6Oct[0] = 0;
			Response2Dec[0] = 0;
			Response3Dec[0] = 0;
			Response4Dec[0] = 0;
		}

		/// <summary>
		/// Perform a table lookup, returns nearest neighbour. If table is null it returns index
		/// </summary>
		/// <param name="index">range 0..1</param>
		/// <param name="table"></param>
		/// <returns></returns>
		public static double Get(double index, double[] table)
		{
			if (table == null)
				return index;

			int idx = (int)(index * 4000.999);
			return table[idx];
		}

		/// <summary>
		/// Returns the index in the table where the value is closest to the desired value.
		/// Only works for monotonic functions
		/// </summary>
		/// <param name="desiredValue">The value we want to find the index of</param>
		/// <param name="table">the table to search</param>
		/// <param name="iterations">The number of iterations to perform</param>
		/// <returns></returns>
		public static double FindIndex(double desiredValue, double[] table, int iterations = 12)
		{
			double dx = 0.25;
			double idx = 0.5;

			if (table == null)
				return desiredValue;

			bool decreasing = table[0] > table[table.Length - 1];

			double sign = decreasing ? -1.0 : 1.0;

			for (int i = 0; i < iterations; i++)
			{
				double val = Get(idx, table);
				if (val > desiredValue)
					idx = idx - dx * sign;
				else
					idx = idx + dx * sign;

				dx = dx * 0.5;
			}

			return idx;
		}
	}
}

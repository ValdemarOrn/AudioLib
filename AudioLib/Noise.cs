using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public class Noise
	{
		static Noise()
		{
			White = new double[100000];
			Pink = new double[100000];
			Random = new double[100000];

			var r = new Random();

			for (int i = 0; i < White.Length; i++)
			{
				White[i] = (r.NextDouble() > 0.5) ? 0.5 : -0.5;
				
				Random[i] = 2 * r.NextDouble() - 1.0;

				// not really pink. Todo: Make pink
				if (i == 0)
					Pink[i] = 0.0;
				else
					Pink[i] = (Pink[i - 1] + Random[i]) * 0.5;
			}
		}

		public static double[] White;
		public static double[] Pink;
		public static double[] Random;
	}
}

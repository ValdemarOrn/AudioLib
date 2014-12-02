using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public static class Extensions
	{
		public static void MixWith(this double[] dest, int count, params double[][] components)
		{
			var bufCount = components.Length;
			for (int buf = 0; buf < bufCount; buf++)
			{
				for (int i = 0; i < count; i++)
					dest[i] += components[buf][i];
			}
		}

		public static void MixInto(this double[] dest, int count, params double[][] components)
		{
			for (int i = 0; i < count; i++)
				dest[i] = components[0][i];

			var bufCount = components.Length;
			for (int buf = 1; buf < bufCount; buf++)
			{
				for (int i = 0; i < count; i++)
					dest[i] += components[buf][i];
			}
		}

		public static void Gain(this double[] buffer, double gain, int count)
		{
			for (int i = 0; i < count; i++)
				buffer[i] *= gain;
		}
		
		public static void Copy(this double[] source, double[] dest, int count)
		{
			Array.Copy(source, dest, count);
		}

		public static void Zero(this double[] source)
		{
			for (int i = 0; i < source.Length; i++)
				source[i] = 0;
		}
	}
}

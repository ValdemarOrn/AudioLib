using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public class LUT
	{
		public double min;
		public double max;
		public double[] table;

		public void ReadFile(string filename)
		{
			var lines = System.IO.File.ReadAllLines(filename);

			// Read header line: min, max and length
			var line = lines[0];
			var culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
			String[] vars = line.Split('\t');
			min = Single.Parse(vars[0], culture);
			max = Single.Parse(vars[1], culture);

			table = new double[lines.Count()-1];

			for (int i = 0; i < lines.Count() - 1; i++)
			{
				table[i] = Single.Parse(lines[i + 1], culture);
			}
		}

		public void GetValuesInPlace(double[] input)
		{
			GetValues(input, input);
		}

		public double[] GetValues(double[] input)
		{
			double[] output = new double[input.Length];
			GetValues(input, output);
			return output;
		}

		public void GetValues(double[] input, double[] output)
		{
			for (int i = 0; i < input.Length; i++)
				output[i] = GetValue(input[i]);
		}

		public double GetValue(double value)
		{
			if (!(value > min && value < max)) throw new ArgumentException("value is not between min and max. Value: " + value);

			double idx = (double)((value - min) / (max - min));
			double output = Interpolate.Spline(idx, table);
			return output;
		}
	}

	
}

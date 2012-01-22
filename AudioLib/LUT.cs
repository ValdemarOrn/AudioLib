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
		public float[] table;

		public void ReadFile(string filename)
		{
			var lines = System.IO.File.ReadAllLines(filename);

			// Read header line: min, max and length
			var line = lines[0];
			var culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
			String[] vars = line.Split('\t');
			min = Single.Parse(vars[0], culture);
			max = Single.Parse(vars[1], culture);

			table = new float[lines.Count()-1];

			for (int i = 0; i < lines.Count() - 1; i++)
			{
				table[i] = Single.Parse(lines[i + 1], culture);
			}
		}

		public float Read(float value)
		{
			if (value > max || value < min) throw new ArgumentException("value is not between min and max. Value: " + value);

			float idx = (float)((value - min) / (max - min));
			float output = Interpolate.Spline(idx, table);
			return output;
		}
	}

	
}

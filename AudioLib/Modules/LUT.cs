using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Modules
{
	public sealed class LUT
	{
		public double Min;
		public double Max;
		public double[] Table;

		public double Bias;

		public void ReadFile(string filename)
		{
			var lines = System.IO.File.ReadAllLines(filename);
			ReadRecord(lines);
		}

		public void ReadRecord(string[] lines)
		{
			// Read header line: min, max and length
			var line = lines[0];
			var culture = System.Globalization.CultureInfo.InvariantCulture;
			String[] vars = line.Split(new char[]{'\t', ' '});
			Min = Single.Parse(vars[0].Trim(), culture);
			Max = Single.Parse(vars[1].Trim(), culture);

			Table = new double[lines.Count()-1];

			for (int i = 0; i < lines.Count() - 1; i++)
			{
				Table[i] = Single.Parse(lines[i + 1].Trim(), culture);
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
			var len = input.Length;
			var maxMin = 1 / (Max - Min);

			double sig = 0.0;

			for (int i = 0; i < len; i++)
			{
				sig = input[i] + Bias;
				if (sig <= Min)
					output[i] = Table[0];
				else if (sig >= Max)
					output[i] = Table[Table.Length - 1];
				else
				{
					// linear interoplation
					double pos = (sig - Min) * maxMin;
					pos = pos * (Table.Length - 0.0001);
					int index = (int)pos;
					if (index >= Table.Length - 1)
					{
						output[i] = Table[index];
					}
					else
					{
						pos = pos - index;
						output[i] = Table[index] * (1.0 - pos) + Table[index] * pos;
					}
				}
			}
		}

		public double GetValue(double value)
		{			
			value = value + Bias;
			if (value < Min)
				return Table[0];
			if (value > Max)
				return Table[Table.Length - 1];

			var maxMin = 1 / (Max - Min);

			double idx = (value - Min) * maxMin;
			double output = Interpolate.Linear(idx, Table);
			return output;
		}
	}

	
}

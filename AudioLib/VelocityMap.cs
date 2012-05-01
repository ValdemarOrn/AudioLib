using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public class VelocityMap
	{
		public int NumberOfPoints
		{
			get
			{
				return _values.Count;
			}
			set
			{
				if (value < 2)
					throw new Exception("You must have at least two zones");

				while (_values.Count < value)
					_values.Add(_values.Count / (double)(value-1));

				while (_values.Count > value)
					_values.RemoveAt(_values.Count-1);
			}
		}

		List<double> _values;

		public List<double> Values
		{
			get
			{
				return _values;
			}
			set
			{
				_values = value;
				NumberOfPoints = _values.Count;
			}
		}

		public VelocityMap(int numberOfPoints)
		{
			_values = new List<double>();
			NumberOfPoints = numberOfPoints;
		}

		public double Map(double input)
		{
			if (input >= 1.0)
				return Values[Values.Count - 1];
			if (input <= 0.0)
				return Values[0];

			int NumZones = NumberOfPoints - 1;
			double zoneSize = 1.0 / NumZones;
			int zone = (int)(input / zoneSize);

			double dy = (Values[zone + 1] - Values[zone]) / zoneSize;
			double output = Values[zone] + (input - zone * zoneSize) * dy;
			return output;
		}


	}
}

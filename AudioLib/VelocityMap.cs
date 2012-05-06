using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public class VelocityMap
	{
		// The working area boundaries
		double _xmin, _xmax, _ymin, _ymax;

		public double XMin
		{
			get { return _xmin;}
			set
			{ 
				if (value >= XMax) 
					value = XMax - 0.01; 
				_xmin = value;

				for (int i = 0; i < X.Count; i++)
				{
					if (X[i] < _xmin)
						X[i] = _xmin;
				}
			}
		}

		public double XMax
		{
			get { return _xmax; }
			set
			{ 
				if (value <= XMin) 
					value = XMin + 0.01; 
				_xmax = value;

				for (int i = 0; i < X.Count; i++)
				{
					if (X[i] > _xmax)
						X[i] = _xmax;
				}
			}
		}

		public double YMin
		{
			get { return _ymin; }
			set 
			{ 
				if (value >= YMax) 
					value = YMax - 0.01; 
				_ymin = value;

				for (int i = 0; i < Y.Count; i++)
				{
					if (Y[i] < _ymin)
						Y[i] = _ymin;
				}
			}
		}

		public double YMax
		{
			get { return _ymax; }
			set 
			{ 
				if (value <= YMin) 
					value = YMin + 0.01; 
				_ymax = value;

				for (int i = 0; i < Y.Count; i++)
				{
					if (Y[i] > _ymax)
						Y[i] = _ymax;
				}
			}
		}

		List<double> X;
		List<double> Y;

		public VelocityMap(int numberOfPoints)
		{
			X = new List<double>();
			Y = new List<double>();

			XMax = 1.0;
			YMax = 1.0;

			for (int i = 0; i < numberOfPoints; i++)
			{
				X.Add(((double)i)/(numberOfPoints-1));
				Y.Add(((double)i) / (numberOfPoints-1));
			}
		}

		public int Count { get { return X.Count; } }

		public double GetX(int i)
		{
			return X[i];
		}

		public double GetY(int i)
		{
			return Y[i];
		}

		public void SetX(double val, int i)
		{
			if (i > 0 && val < X[i - 1])
				val = X[i - 1] + 0.000001;

			if (i < X.Count - 1 && val > X[i + 1])
				val = X[i + 1] - 0.000001;

			if (val < XMin)
				val = XMin;
			
			if (val > XMax)
				val = XMax;

			X[i] = val;
		}

		public void SetY(double val, int i)
		{
			if (val < YMin)
				val = YMin;

			if (val > YMax)
				val = YMax;

			Y[i] = val;
		}

		public void Add(double x, double y)
		{
			int index = 0;
			while (index < X.Count && X[index] < x)
				index++;

			X.Insert(index, x);
			Y.Insert(index, y);
		}

		public void Remove(int index)
		{
			X.RemoveAt(index);
			Y.RemoveAt(index);
		}

		public double Map(double input)
		{
			if (input >= X[X.Count - 1])
				return Y[Y.Count - 1];
			if (input <= X[0])
				return Y[0];

			int zone = 0;

			while (X[zone + 1] <= input)
				zone++;

			double zoneSize = X[zone + 1] - X[zone];

			double dy = (Y[zone + 1] - Y[zone]) / zoneSize;
			double output = Y[zone] + ((input - X[zone])) * dy;
			return output;
		}


	}
}

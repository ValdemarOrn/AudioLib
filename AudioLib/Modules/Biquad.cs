using System;

namespace AudioLib.Modules
{
	/// <summary>
	/// Biquadratic filter with multiple modes of operation
	/// For reference, see: 
	/// http://www.ti.com/lit/an/slaa447/slaa447.pdf
	/// http://www.musicdsp.org/files/Audio-EQ-Cookbook.txt
	/// http://www.musicdsp.org/files/biquad.c
	/// </summary>
	public class Biquad
	{
		public enum FilterType
		{
			LowPass = 0,
			HighPass, 
			BandPass, 
			Notch, 
			Peak, 
			LowShelf, 
			HighShelf
		};

		private double samplerate;
		private double _q;
		private double a0, a1, a2, b0, b1, b2;
		private double x1, x2, y, y1, y2;
		private double gain;

		public FilterType Type;
		public double Output;
		
		/// <summary>
		/// Cutoff / Center Frequency
		/// </summary>
		public double Frequency;

		public double Samplerate
		{
			get { return samplerate; }
			set { samplerate = value; Update(); }
		}

		public double Slope;

		/// <summary>
		/// Gain for shelfing and peak filter
		/// </summary>
		public double GainDB
		{
			get
			{
				return Math.Log10(gain) * 20;
			}
			set
			{
				gain = Math.Pow(10, value / 20);
			}
		}

		public double Gain
		{
			get { return gain; }
			set
			{
				if (value == 0)
					value = 0.001; // -60dB
				gain = value;
			}
		}

		/// <summary>
		/// Q-factor
		/// </summary>
		public double Q
		{
			get { return _q; }
			set
			{
				if (value == 0)
					value = 1e-12;
				_q = value;
			}
		}

		public double[] A { get { return new double[] { 1, a1, a2 }; } }

		public double[] B { get { return new double[] { b0, b1, b2 }; } }

		public Biquad()
		{

		}

		public Biquad(FilterType filterType, double samplerate)
		{
			Type = filterType;
			Samplerate = samplerate;

			GainDB = 0.0;
			Frequency = samplerate / 4;
			Q = 0.5;
		}


		public void Update()
		{
			double omega = 2 * Math.PI * Frequency / Samplerate;
			double sinOmega = Math.Sin(omega);
			double cosOmega = Math.Cos(omega);

			double sqrtA = 0.0;
			double alpha = 0.0;

			// The gain is here is a bit fishy! It's now correct for peaking
			var A = Math.Pow(10, GainDB / 40);

			if (Type == FilterType.LowShelf || Type == FilterType.HighShelf)
			{
				alpha = sinOmega / 2 * Math.Sqrt((A + 1 / A) * (1 / Slope - 1) + 2);
				sqrtA = Math.Sqrt(A);
			}
			else
			{
				alpha = sinOmega / (2 * Q);
			}

			switch (Type)
			{
				case FilterType.LowPass:
					b0 = (1 - cosOmega) / 2;
					b1 = 1 - cosOmega;
					b2 = (1 - cosOmega) / 2;
					a0 = 1 + alpha;
					a1 = -2 * cosOmega;
					a2 = 1 - alpha;
					break;
				case FilterType.HighPass:
					b0 = (1 + cosOmega) / 2;
					b1 = -(1 + cosOmega);
					b2 = (1 + cosOmega) / 2;
					a0 = 1 + alpha;
					a1 = -2 * cosOmega;
					a2 = 1 - alpha;
					break;
				case FilterType.BandPass:
					b0 = alpha;
					b1 = 0;
					b2 = -alpha;
					a0 = 1 + alpha;
					a1 = -2 * cosOmega;
					a2 = 1 - alpha;
					break;
				case FilterType.Notch:
					b0 = 1;
					b1 = -2 * cosOmega;
					b2 = 1;
					a0 = 1 + alpha;
					a1 = -2 * cosOmega;
					a2 = 1 - alpha;
					break;
				case FilterType.Peak:
					b0 = 1 + (alpha * A);
					b1 = -2 * cosOmega;
					b2 = 1 - (alpha * A);
					a0 = 1 + (alpha / A);
					a1 = -2 * cosOmega;
					a2 = 1 - (alpha / A);
					break;
				case FilterType.LowShelf:
					b0 = A * ((A + 1) - (A - 1) * cosOmega + 2 * sqrtA * alpha);
					b1 = 2 * A * ((A - 1) - (A + 1) * cosOmega);
					b2 = A * ((A + 1) - (A - 1) * cosOmega - 2 * sqrtA * alpha);
					a0 = (A + 1) + (A - 1) * cosOmega + 2 * sqrtA * alpha;
					a1 = -2 * ((A - 1) + (A + 1) * cosOmega);
					a2 = (A + 1) + (A - 1) * cosOmega - 2 * sqrtA * alpha;
					break;
				case FilterType.HighShelf:
					b0 = A * ((A + 1) + (A - 1) * cosOmega + 2 * sqrtA * alpha);
					b1 = -2 * A * ((A - 1) + (A + 1) * cosOmega);
					b2 = A * ((A + 1) + (A - 1) * cosOmega - 2 * sqrtA * alpha);
					a0 = (A + 1) - (A - 1) * cosOmega + 2 * sqrtA * alpha;
					a1 = 2 * ((A - 1) - (A + 1) * cosOmega);
					a2 = (A + 1) - (A - 1) * cosOmega - 2 * sqrtA * alpha;
					break;
			}

			double g = 1 / a0;

			b0 = b0 * g;
			b1 = b1 * g;
			b2 = b2 * g;
			a1 = a1 * g;
			a2 = a2 * g;
		}

		/// <summary>
		/// Calculate the frequency response for a given frequency
		/// </summary>
		/// <param name="freq"></param>
		/// <returns></returns>
		public double GetResponse(double freq)
		{
			// This looks buggered, the gain is 2x too big
			double phi = Math.Pow((Math.Sin(2 * Math.PI * freq / (2.0 * Samplerate))), 2);
			return (Math.Pow(b0 + b1 + b2, 2.0) - 4.0 * (b0 * b1 + 4.0 * b0 * b2 + b1 * b2) * phi + 16.0 * b0 * b2 * phi * phi) / (Math.Pow(1.0 + a1 + a2, 2.0) - 4.0 * (a1 + 4.0 * a2 + a1 * a2) * phi + 16.0 * a2 * phi * phi);
		}

		public double Process(double x)
		{
			y = b0 * x + b1 * x1 + b2 * x2 - a1 * y1 - a2 * y2;
			x2 = x1;
			y2 = y1;
			x1 = x;
			y1 = y;

			Output = y;
			return Output;
		}

		public void Process(double[] input, double[] output, int len)
		{
			for (int i = 0; i < len; i++)
				output[i] = Process(input[i]);
		}

		public void ClearBuffers()
		{
			y = 0;
			x2 = 0;
			y2 = 0;
			x1 = 0;
			y1 = 0;
		}
	}
}

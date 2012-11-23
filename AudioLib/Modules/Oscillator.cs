using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLib.Modules
{
	public sealed class Oscillator
	{
		/// <summary>
		/// Radians per sample, from 0...0.5
		/// </summary>
		double FreqRad;

		public double[] Wave;
		public double Phase;

		private double _fs;
		private double _fsInv;
		private double _nyquist;
		private double _nyquistInv;

		public double Samplerate
		{
			get { return _fs; }
			set
			{ 
				_fs = value; 
				_fsInv = 1.0 / _fs;
				_nyquist = value / 2.0;
				_nyquistInv = 1 / _nyquist;
			}
		}

		public Oscillator(double samplerate)
		{
			Samplerate = samplerate;
		}

		public void SetFrequencyHz(double hz)
		{
			this.FreqRad = hz * _fsInv;  // 500hz / 48000
		}

		/// <summary>
		/// Set the frequency by pitch
		/// </summary>
		/// <param name="pitch">
		/// Pitch of the note. 69.0f = A4 = 440Hz = Midi note 69. 
		/// Pitch increases 1 octave for +12.0. 
		/// Note: Negative values are allowed
		/// </param>
		/// <param name="Fs">The sampling frequency used as reference</param>
		public void SetFrequencyPitch(double pitch)
		{
			double p = (double)Math.Pow(2, (pitch - 69f)/12.0f) * 440.0f;
			SetFrequencyHz(p);
		}

		/// <summary>
		/// Set frequency by control voltage
		/// </summary>
		/// <param name="CV">control voltage. 4V = 440Hz = A4. 1 volt per octave</param>
		/// <param name="Fs">The sampling frequency used as reference</param>
		public void SetFrequencyCV(double CV)
		{
			double p = (double)Math.Pow(2,(CV-4.0f))*440.0f;
			SetFrequencyHz(p);
		}

		public double getSample()
		{
			Phase = (Phase + FreqRad) % 1.0;

			int idx = (int)(Phase * Wave.Length);
			double sample = Wave[idx];
			return sample;
		}

		/// <summary>
		/// returns the optimal distribution of partials per wave, make sure no partials go below
		/// the specified minimum of above the sampling frequency (causing aliasing)
		/// </summary>
		/// <param name="minimumFrequency"></param>
		/// <param name="samplerate"></param>
		/// <returns></returns>
		public static List<int> GetMaxPartials(double minimumFrequency, double samplerate)
		{
			double min = minimumFrequency;
			double max = samplerate;

			var partials = new List<int>();

			partials.Add(128);
			int current = 128;

			for (int i = 1; i < max; i++)
			{
				var cMax = current * i;

				if (cMax >= max)
				{
					current = (int)((min - 1) / (double)i);
					if ((current + 1) * i < max)
						current++;

					partials.Add(current);
				}
			}

			return partials;
		}
	}
}

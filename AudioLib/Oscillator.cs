using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLib
{
	public class Oscillator
	{
		// 0.0 ... 1.0, 1.0 = 2*Pi/sec
		float Frequency;
		public WaveStack Waves;
		public double Phase;

		public Oscillator()
		{
			this.Waves = new WaveStack();
		}

		/// <summary>
		/// Set the frequency of the oscillator in Hz
		/// </summary>
		/// <param name="Freq">The desired frequency in rad/sec. 1.0 = 2*Pi rad/sec = Fs. 0.5 = Fs/2 (Max)</param>
		public void SetFrequencyRadSec(float Freq)
		{
			this.Frequency = Freq;
		}

		public void SetFrequencyHz(float hz, float Fs)
		{
			this.Frequency = hz / Fs;
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
		public void SetFrequencyPitch(float pitch, float Fs)
		{
			float p = (float)Math.Pow(2, (pitch - 69f)/12.0f) * 440.0f;
			this.Frequency = p / Fs;
		}

		/// <summary>
		/// Set frequency by control voltage
		/// </summary>
		/// <param name="CV">control voltage. 4V = 440Hz = A4. 1 volt per octave</param>
		/// <param name="Fs">The sampling frequency used as reference</param>
		public void SetFrequencyCV(float CV, float Fs)
		{
			float pitch = (float)Math.Pow(2,(CV-4.0f))*440.0f;
			this.Frequency = pitch / Fs;
		}

		public float getSample()
		{
			var sample = Waves.GetSample((float)Phase, Frequency);
			Phase = (Phase + Frequency) % 1.0;

			return sample;
		}
	}
}

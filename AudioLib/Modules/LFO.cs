using AudioLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Modules
{
	public sealed class LFO
	{
		public enum Wave
		{
			None = 0,
			Sine = 1,
			Triangle,
			Saw,
			Ramp,
			Pulse,
			SampleAndHold
		}

		public static Dictionary<Wave, string> WaveNames = new Dictionary<Wave, string>
		{
			{ Wave.None, "Constant" },
			{ Wave.Sine, "Sine" },
			{ Wave.Triangle, "Triangle" },
			{ Wave.Saw, "Sawtooth" },
			{ Wave.Ramp, "Ramp" },
			{ Wave.Pulse, "Pulse" },
			{ Wave.SampleAndHold, "Sample & hold" }
		};

		public static double GetSample(Wave selectedWave, double accumulator, double stepOrPwm)
		{
			switch (selectedWave)
			{
				case Wave.Ramp:
					return 2 * accumulator - 1.0;
				case Wave.Saw:
					return 1.0 - 2 * accumulator;
				case Wave.Sine:
					return Math.Sin(accumulator * 2 * Math.PI);
				case Wave.Pulse:
					return (accumulator < stepOrPwm) ? 1.0 : -1.0;
				case Wave.Triangle:
					return Utils.Triangle(accumulator);
				case Wave.SampleAndHold:
					return Noise.Random[(int)stepOrPwm % Noise.Random.Length];
				default:
					return 0;
			}
		}

		private double fs;
		private double fsInv;
		private double accumulator;
		private double stepsize;
		private double step; // S&H Step

		public double Output;
		public Wave SelectedWave;
		public double FreqHz;
		public double StartPhase; // 0...1
		public double Shape; // PWM
		public bool TempoSync;

		public LFO(double samplerate)
		{
			Samplerate = samplerate;
			FreqHz = 0.5;
			UpdateStepsize();
		}

		public double Samplerate
		{
			get { return fs; }
			set
			{
				fs = value;
				fsInv = 1.0 / fs;
			}
		}

		public void Reset()
		{
			accumulator = StartPhase;
		}

		public void UpdateStepsize()
		{
			double increment = FreqHz * fsInv;
			stepsize = increment;
		}

		public double Process(int sampleCount)
		{
			Output = GetSample(SelectedWave, accumulator, (SelectedWave == Wave.SampleAndHold) ? step : Shape);

			accumulator += stepsize * sampleCount;
			if (accumulator > 1)
			{
				accumulator -= 1;
				step++;
			}

			return Output;
		}

		
	}
}

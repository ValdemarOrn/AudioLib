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

		public static double GetSample(Wave selectedWave, double accumulator, double stepOrPWM)
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
					return (accumulator < stepOrPWM) ? 1.0 : -1.0;
				case Wave.Triangle:
					return Utils.Triangle(accumulator);
				case Wave.SampleAndHold:
					return Noise.Random[(int)stepOrPWM % Noise.Random.Length];
				default:
					return 0;
			}
		}

		double _fs;
		double _fsInv;
		public double Samplerate
		{
			get { return _fs; }
			set
			{
				_fs = value;
				_fsInv = 1.0 / _fs;
			}
		}

		public double Output;

		public Wave SelectedWave;
		public double FreqHz;
		public double StartPhase; // 0...1
		public double Shape; // PWM
		public bool TempoSync;

		private double Accumulator;
		private double Stepsize;
		private double Step; // S&H Step

		public LFO(double samplerate)
		{
			Samplerate = samplerate;
			FreqHz = 0.5;
			UpdateStepsize();
		}

		public void Reset()
		{
			Accumulator = StartPhase;
		}

		public void UpdateStepsize()
		{
			double increment = FreqHz * _fsInv;
			Stepsize = increment;
		}

		public double Process(int sampleCount)
		{
			Output = GetSample(SelectedWave, Accumulator, (SelectedWave == Wave.SampleAndHold) ? Step : Shape);

			Accumulator += Stepsize * sampleCount;
			if (Accumulator > 1)
			{
				Accumulator -= 1;
				Step++;
			}

			return Output;
		}

		
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Modules
{
	public class RandWalk
	{
		private int i;
		private int iRandom;
		private double increment;
		private double value;
		private double filtered;
		private int updateModulo;
		private double fa, fb;
		private double samplerate;
		private double samplerateInv;
		private double smoothness;
		private int updateFrequency;

		public RandWalk(double samplerate, int updateFreq, long seed)
		{
			iRandom = (int)(seed % Noise.Random.Length);
			updateFrequency = updateFreq;
			Samplerate = samplerate;
		}

		public double Output { get; private set; }

		public int UpdateFrequency
		{
			get { return updateFrequency; }
			set
			{
				updateFrequency = value;
				Update();
			}
		}

		public double Samplerate
		{
			get { return samplerate; }
			set
			{
				samplerate = value;
				samplerateInv = 1 / samplerate;
				Update();
			}
		}

		public double Smoothness
		{
			get { return smoothness; }
			set
			{
				if (value > 1)
					smoothness = 1;
				else if (value < 0.000001)
					smoothness = 0.000001;
				else
					smoothness = value;

				Update();
			}
		}

		public double Process()
		{
			if (i == 0)
			{
				increment = Noise.Random[iRandom] * samplerateInv;
				iRandom++;
				if (iRandom >= Noise.Random.Length)
					iRandom = 0;

				var distance = 1 + Math.Abs(value);

				if ((increment >= 0 && value >= 0) || (increment <= 0 && value <= 0))
					increment = increment / distance;
			}

			i++;
			if (i >= updateModulo)
				i = 0;

			value = value + increment;
			filtered = filtered * fa + value * fb;
			Output = filtered;
			return filtered;
		}

		private void Update()
		{
			updateModulo = (int)(samplerate / updateFrequency);
			fb = 0.5 * (1 - smoothness) / updateModulo;
			fa = 1.0 - fb;
		}
	}
}

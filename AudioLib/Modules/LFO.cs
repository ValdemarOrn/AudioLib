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
			Sine,
			Triangle,
			Saw,
			Ramp,
			Pulse
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
				FreqHz = FreqHz;
			}
		}

		public Wave SelectedWave;

		double _freqHz;
		public double FreqHz
		{
			get { return _freqHz; }
			set
			{
				_freqHz = value;
				FreqRad = FreqHz * _fsInv;
			}
		}

		//NoteLength FreqSync;

		/*public void SetFreqSync(Sync value)
		{

		}*/

		double FreqRad;
		public double Phase;
		public double StartPhase;
		public double PulseWidth;
		public bool GateReset;
		public bool TempoSync;

		public LFO()
		{ 
		
		}

		public void Reset()
		{
			Phase = StartPhase;
		}

		public double Process(int samples)
		{
			var value = 0.0;

			switch(SelectedWave)
			{
				case Wave.Ramp:
					value = 2 * Phase - 1.0;
					break;
				case Wave.Saw:
					value = 1.0 - 2 * Phase;
					break;
				case Wave.Sine:
					value = Math.Sin(Phase * 2 * Math.PI);
					break;
				case Wave.Pulse:
					value = (Phase < PulseWidth) ? 1.0 : -1.0;
					break;
				case Wave.Triangle:
					value = Utils.Triangle(Phase);
					break;
			}

			Phase += FreqRad;

			Output = value;
			return Output;
		}

		public double Output;
	}
}

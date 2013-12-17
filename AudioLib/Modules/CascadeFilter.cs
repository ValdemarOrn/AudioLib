using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Modules
{
	public sealed class CascadeFilter
	{
		public double Output;
		public double[] OutputBuffer;

		public double CutoffKnob;
		public double CutoffModulation;
		public double CutoffModulationEnv;

		public double Resonance;
		public double ResonanceModulation;

		public double VX;
		public double VA;
		public double VB;
		public double VC;
		public double VD;

		const double oversample = 4;
		double _fsinv;
		double _samplerate;

		double P;

		double x = 0;
		double a = 0;
		double b = 0;
		double c = 0;
		double d = 0;
		double Feedback = 0;

		public double Samplerate
		{
			get { return _samplerate; }
			set
			{
				_samplerate = value;
				_fsinv = 1 / (oversample * value);
			}
		}

		public CascadeFilter(double samplerate, int bufferSize)
		{
			this.Samplerate = samplerate;
			OutputBuffer = new double[bufferSize];
			CutoffKnob = 1;
			VD = 1;
		}

		public void UpdateCoefficients()
		{
			var value = CutoffKnob + CutoffModulation + CutoffModulationEnv;
			if (value > 1)
				value = 1;
			else if (value < 0)
				value = 0;

			var cutoff = 10 + ValueTables.Get(value, ValueTables.Response3Dec) * 21000;
			P = (1 - 2 * cutoff * _fsinv) * (1 - 2 * cutoff * _fsinv);
		}

		public double Process(double input)
		{
			var reso = Resonance + ResonanceModulation;
			if (reso > 1)
				reso = 1;
			else if (reso < 0)
				reso = 0;

			for (int i = 0; i < oversample; i++)
			{
				double fb = reso * 5 * (Feedback - 0.5 * input);
				double val = input - fb;
				x = val;

				// 4 cascaded low pass stages
				a = (1 - P) * val + P * a;
				val = a;
				b = (1 - P) * val + P * b;
				val = b;
				c = (1 - P) * val + P * c;
				val = c;
				d = (1 - P) * val + P * d;
				val = d;

				Feedback = Utils.TanhApprox(val);
			}

			Output = (VX * x + VA * a + VB * b + VC * c + VD * d) * (1 - reso * 0.5);
			return Output;
		}

		public void Process(double[] input, double[] cutoffModulationEnv)
		{
			for(int i = 0; i < input.Length; i++)
			{
				CutoffModulationEnv = cutoffModulationEnv[i];
				UpdateCoefficients();
				OutputBuffer[i] = Process(input[i]);
			}
		}

		
	}
}

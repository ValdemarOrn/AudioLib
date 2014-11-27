using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AudioLib.Modules
{
	public class TwoPoleFilter
	{
		/*
			sys = tf(this.ToString(), 1/Samplerate)

			opt = bodeoptions;
			opt.FreqUnits = 'Hz';
			opt.MagLowerLimMode = 'manual';
			opt.MagLowerLim = -60;

			bode(sys,opt)		 
		*/

		public enum FilterMode
		{
			None = 0,
			LowPass = 1,
			HighPass = 2,
			BandPass = 3,
			Notch = 4
		};

		private double[] sa, sb;
		private double[] za, zb;

		private double fs;
		private double ts;

		public FilterMode Mode;
		public double Fc;
		public double Q;

		private int index;
		private int bufSize;
		private double[] bufIn;
		private double[] bufOut;

		public double Samplerate
		{
			get { return fs; }
			set { fs = value; ts = 1 / fs; }
		}

		public TwoPoleFilter()
		{
			sa = new double[3];
			sb = new double[3];
			za = new double[3];
			zb = new double[3];

			bufSize = 1024;
			bufIn = new double[bufSize];
			bufOut = new double[bufSize];
		}

		public void Update()
		{
			var fRad = Fc * 2 * Math.PI;
			// prewarp
			// fRad = 2 / Ts * Math.Tan(fRad * Ts / 2);
			var w = 2 * Samplerate * Math.Tan(fRad * ts * 0.5);

			switch (Mode)
			{
				case FilterMode.LowPass:
					sb[0] = w * w;
					sb[1] = 0.0;
					sb[2] = 0.0;
					break;
				case FilterMode.HighPass:
					sb[0] = 0.0;
					sb[1] = 0.0;
					sb[2] = 1.0;
					break;
				case FilterMode.BandPass:
					sb[0] = 0.0;
					sb[1] = w / Q;
					sb[2] = 0.0;
					break;
				case FilterMode.Notch:
					sb[0] = w * w; // Can also choose another zero for interesting effects
					sb[1] = 0.0;
					sb[2] = 1.0;
					break;
			}

			sa[0] = w * w;
			sa[1] = w / Q;
			sa[2] = 1.0;
			Bilinear.StoZ2(sb, sa, zb, za, Samplerate);

			var gInv = 1.0 / za[0];
			zb[0] *= gInv;
			zb[1] *= gInv;
			zb[2] *= gInv;

			za[0] = 1.0;
			za[1] *= gInv;
			za[2] *= gInv;
		}

		public void Process(double[] input, double[] output)
		{
			var len = input.Length;
			for (var i = 0; i < len; i++)
			{
				index = (index + 1);
				if (index >= bufSize)
					index -= bufSize;

				bufIn[index] = input[i];
				bufOut[index] = 0;

				if (index < 2)
				{
					bufOut[index] =
						  zb[0] *  bufIn[((index)     + bufSize) % bufSize]
						+ zb[1] *  bufIn[((index - 1) + bufSize) % bufSize]
						+ zb[2] *  bufIn[((index - 2) + bufSize) % bufSize]
						- za[1] * bufOut[((index - 1) + bufSize) % bufSize]
						- za[2] * bufOut[((index - 2) + bufSize) % bufSize];
				}
				else
				{
					bufOut[index] =
						  zb[0] * bufIn[index]
						+ zb[1] * bufIn[index - 1]
						+ zb[2] * bufIn[index - 2]
						- za[1] * bufOut[index - 1]
						- za[2] * bufOut[index - 2];
				}

				bufOut[index] = bufOut[index];
				output[i] = bufOut[index];
			}
		}

		public double Process(double input)
		{
			index = (index + 1);
			if (index >= bufSize)
				index -= bufSize;

			bufIn[index] = input;
			bufOut[index] = 0;

			if (index < 2)
			{
				bufOut[index] =
						zb[0] * bufIn[((index) + bufSize) % bufSize]
					+ zb[1] * bufIn[((index - 1) + bufSize) % bufSize]
					+ zb[2] * bufIn[((index - 2) + bufSize) % bufSize]
					- za[1] * bufOut[((index - 1) + bufSize) % bufSize]
					- za[2] * bufOut[((index - 2) + bufSize) % bufSize];
			}
			else
			{
				bufOut[index] =
						zb[0] * bufIn[index]
					+ zb[1] * bufIn[index - 1]
					+ zb[2] * bufIn[index - 2]
					- za[1] * bufOut[index - 1]
					- za[2] * bufOut[index - 2];
			}

			bufOut[index] = bufOut[index];
			var output = bufOut[index];
			return output;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0} {1} {2}], [{3} {4} {5}]", zb[0], zb[1], zb[2], za[0], za[1], za[2]);
		}

	}
}

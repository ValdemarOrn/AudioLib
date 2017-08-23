using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public sealed class Utils
	{
		static Utils()
		{
			note2HzTable = new double[12801];
			for(int i= 0; i<note2HzTable.Length; i++)
				note2HzTable[i] = Note2Hz(i * 0.01);

			TanhTable = new double[200001];
			for (int i = 0; i < TanhTable.Length; i++)
				TanhTable[i] = Math.Tanh(i * 0.0001 - 10.0);
		}

		public static double ExpResponse(double input)
		{
			return (double)(Math.Pow(20, input)-1)/19;
		}

		public static double LogResponse(double input)
		{
			return Math.Log(20 * input + 1) / Math.Log(21);
		}

		public static double DB2gain(double input)
		{
			return (double)Math.Pow(10, input/20);
		}

		public static double Gain2DB(double input)
		{
			return (double)(20*Math.Log10(input));
		}

		public static double Note2Hz(double note)
		{
			return Math.Pow(2, (note - 69.0) / 12.0) * 440.0;
		}

		static double[] note2HzTable;
		public static double Note2HzLookup(double note)
		{
			if (note < 0)
				note = 0.0;
			else if(note >= 128.0)
				note = 127.999;

			int i = (int)(note * 100);
			double between = note * 100 - i;

			double output = note2HzTable[i] * (1.0 - between) + note2HzTable[i + 1] * between;
			return output;
		}

		static double[] TanhTable;
		public static double TanhLookup(double value)
		{
			value += 10.0;

			if (value < 0.0)
				return -1.0;
			else if (value > 20.0)
				return 1.0;

			// floor
			//int i = (int)(value * 10000);
			//double output = TanhTable[i];
			//return output;

			// linear
			double v = value * 10000;
			int i = (int)v;
			double between = v - i;

			double output = TanhTable[i] * (1.0 - between) + TanhTable[i + 1] * between;
			return output;
		}

		public static double TanhApprox(double x)
		{
			double invert = x < 0.0 ? -1 : 1;
			x = x * invert;

			if (x > 2)
				return invert;

			double val = x - 0.25 * x * x;
			return val * invert;
		}

		public static double[] Linspace(double min, double max, int num)
		{
			double space = (max-min)/(num-1);
			double[] output = new double[num];
			output[0] = min;
			double runningVal = min;
			for(int i=1; i<num; i++)
			{
				runningVal = runningVal + space;
				output[i] =  (double)runningVal;
			}

			return output;
		}

        public static double[] Octavespace(double start, double max, int pointsPerOctave)
        {
            var output = new List<double>();
            var pto = (double)pointsPerOctave;

            int i = 0;
            while (true)
            {
                double n = i / pto;
                var val = start * Math.Pow(2, (1 + n));
                if (val > max)
                    break;

                output.Add(val);
                i++;
            }

            return output.ToArray();
        }

        public static double[] Decadespace(double start, double max, int pointsPerDecade)
        {
            var output = new List<double>();
            var ptd = (double)pointsPerDecade;

            int i = 0;
            while (true)
            {
                double n = i / ptd;
                var val = start * Math.Pow(10, (1 + n));
                if (val > max)
                    break;

                output.Add(val);
                i++;
            }

            return output.ToArray();
        }

        public static double Min(double[] input)
		{
			double min = input[0];
			for(int i=1; i < input.Length; i++)
				if(input[i] < min)
					min = input[i];
		
			return min;
		}

		public static double Max(double[] input)
		{
			double max = input[0];
			for(int i=1; i < input.Length; i++)
				if(input[i] > max)
					max = input[i];

			return max;
		}

		public static double Average(double[] input)
		{
			double ave = 0;
			for(int i=0; i < input.Length; i++)
				ave += input[i];

			return ave / (double)input.Length;
		}
	
		public static double RMS(double[] input)
		{
			double rms = 0;
			for(int i=0; i < input.Length; i++)
				rms += input[i] * input[i];

			return (double)Math.Sqrt(rms /(double)input.Length);
		}

		public static double[] Gain(double[] input, double gain)
		{
			double[] output = new double[input.Length];

			for (int i = 0; i < input.Length; i++)
				output[i] = input[i] * gain;

			return output;
		}

		public static void GainInPlace(double[] input, double gain)
		{
			for(int i=0; i < input.Length; i++)
				input[i] = input[i] * gain;
		}

		public static void Add(double[] input, double value)
		{
			for (int i = 0; i < input.Length; i++)
				input[i] += value;
		}

		public static void Add(double[] input, double[] addedSignal)
		{
			for (int i = 0; i < input.Length; i++)
			{
				input[i] += addedSignal[i];
			}
		}

		public static void AddInPlace(double[] input, double value)
		{
			for (int i = 0; i < input.Length; i++)
				input[i] = input[i] + value;
		}

		public static double[] Saturate(double[] input, double max)
		{
			return Utils.Saturate(input, -max, max);
		}

		public static double[] Saturate(double[] input, double min, double max)
		{
			double[] output = new double[input.Length];

			for(int i=0; i<input.Length; i++)
			{
				if(input[i] < min)
					output[i] = min;
				else if (input[i] > max)
					output[i] = max;
				else
					output[i] = input[i];
			}

			return output;
		}

		public static void SaturateInPlace(double[] input, double max)
		{
			Utils.SaturateInPlace(input, -max, max);
		}

		public static void SaturateInPlace(double[] input, double min, double max)
		{
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] < min)
					input[i] = min;
				else if (input[i] > max)
					input[i] = max;
			}
		}

		public static double[] Downsample(double[] input, int factor)
		{
			var output = input.Where((x, i) => i % factor == 0).ToArray();
			return output;
		}

		public static double[] MovingAve(double[] input, int windowSize)
		{
			var output = new double[input.Length];
			MovingAve(input, output, windowSize);
			return output;
		}

		public static void MovingAve(double[] input, double[] output, int windowSize)
		{
			// if the windowSize is an even number, we add half of each end-value
			bool even = (windowSize % 2) == 0;

			if(even)
				windowSize++;

			int offset = (windowSize - 1) / 2;

			for(int i=0; i < input.Length; i++)
			{
				output[i] = 0;

				for(int k = 0; k < windowSize; k++)
				{
					int index = i + k - offset;
					if(index < 0)
						index = 0;
					else if(index >= input.Length)
						index = input.Length - 1;

					if(even && (k == 0 || k == windowSize - 1))
						output[i] += input[index] * 0.5;
					else
						output[i] += input[index];
				}

				if (even)
					output[i] = output[i] / (windowSize - 1);
				else
					output[i] = output[i] / windowSize;
				
			}
		}

		public static double[] Normalize(double[] wave)
		{
			var min = wave.Min();
			var max = wave.Max();
			var amplitude = max - min;
			if(amplitude == 0)
				return wave.Select(x => x).ToArray();

			var ampInv = 1 / amplitude;
			return wave.Select(x => (x - min) * ampInv * 2 - 1).ToArray();
		}

		/// <summary>
		/// Create a Sinc wave
		/// </summary>
		/// <param name="omega">Range between 0...1 (0.5 = Nyquist, fs/2)</param>
		/// <param name="M">size of output is 2M+1</param>
		/// <returns></returns>
		public static double[] Sinc(double omega, int M)
		{
			double[] x = Linspace(-M, M, 2 * M + 1);
			double[] output = new double[2 * M + 1];

			for (int i = 0; i < M; i++)
			{
				output[i] = (double)(Math.Sin(Math.PI * 2.0 * x[i] * omega) / (Math.PI * 2.0 * x[i] * omega));
				output[2 * M - i] = output[i];
			}

			output[M] = 1;

			return output;
		}

		/// <summary>
		/// Create a window
		/// </summary>
		/// <param name="M">M length of output window N = 2M+1</param>
		/// <param name="type">type can be Bartlett, Hann, Hamming or Blackman</param>
		/// <returns></returns>
		public static double[] Window(int M, String type)
		{
			double[] n = Linspace(-M, M, 2 * M + 1);
			double[] output = new double[2 * M + 1];

			if (type.ToLower().Equals("bartlett"))
			{
				for (int i = 0; i < output.Length; i++)
					output[i] = (double)((n[i] > 0) ? (1 - n[i] / (M + 1)) : (1 + n[i] / (M + 1)));
			}
			else if (type.ToLower().Equals("hann"))
			{
				for (int i = 0; i < output.Length; i++)
					output[i] = (double)(0.5 * (1 + Math.Cos(2 * Math.PI * n[i] / (2 * M + 1))));
			}
			else if (type.ToLower().Equals("hamming"))
			{
				for (int i = 0; i < output.Length; i++)
					output[i] = (double)(0.54 + 0.46 * Math.Cos(2 * Math.PI * n[i] / (2 * M + 1)));
			}
			else if (type.ToLower().Equals("blackman"))
			{
				for (int i = 0; i < output.Length; i++)
					output[i] = (double)(0.42 + 0.5 * Math.Cos(2 * Math.PI * n[i] / (2 * M + 1)) + 0.08 * Math.Cos(4 * Math.PI * n[i] / (2 * M + 1)));
			}
			else // Rect window
			{
				for (int i = 0; i < output.Length; i++)
					output[i] = 1;
			}

			return output;
		}

		public static double[] SincFilter(double wc, int M, String type)
		{
			double[] a = Sinc(wc, M);
			double[] b = Window(M, type);

			for (int i = 0; i < b.Length; i++)
				b[i] = a[i] * b[i];

			var sum = b.Sum();

			for (int i = 0; i < b.Length; i++)
				b[i] = b[i] / sum;

			return b;
		}

		

		/// <summary>
		/// Creates an array containing a sine wave
		/// </summary>
		/// <param name="len">number of samples in the output array</param>
		/// <param name="phase">The starting phase of the signal</param>
		/// <param name="rad">The normalized frequency, radians per sample. Values 0...pi (other values are valid but are aliased)</param>
		/// <param name="mag">Magnitude, V_peak</param>
		/// <returns></returns>
		public static double[] Sinewave(int len, double phase, double rad, double mag)
		{
			double[] output = new double[len];

			for(int i=0; i<output.Length; i++)
			{
				output[i] = (double)Math.Sin(phase + i*rad) * mag;
			}

			return output;
		}

		public static double[] Saw(int len, double mag)
		{
			double unit = 2 * mag / (len-1);
			double[] output = new double[len];
			for (int i = 0; i < len; i++)
				output[i] = (mag - i * unit);

			return output;
		}

		public static double[] SawAdditive(int len, int partials, double mag)
		{
			mag = (double)(2 / Math.PI * mag);
			double[] output = new double[len];

			for (int i = 1; i < partials; i++)
			{
				for (int j = 0; j < len; j++)
				{
					output[j] = output[j] + (double)Math.Sin(i / (double)len * 2.0 * Math.PI * j) / i;
				}
			}

			// Set magnitude
			for (int j = 0; j < len; j++)
			{
				output[j] = output[j] * mag;
			}

			return output;
		}

		public static double[] Square(int len, double mag, double width)
		{
			int highSample = (int)(width * len + 0.5f);
			int startSample = (len - highSample)/2;

			double[] output = new double[len];
			for (int i = 0; i < len; i++)
			{
				if (i >= startSample && i < (startSample + highSample))
					output[i] = mag;
				else
					output[i] = -mag;
			}

			return output;
		}

		public static double[] SquareAdditive(int len, int partials, double mag)
		{
			mag = (double)(4 / Math.PI * mag);
			double[] output = new double[len];

			for (int i = 1; i < partials; i=i+2)
			{
				for (int j = 0; j < len; j++)
				{
					output[j] = output[j] + (double)Math.Sin(i / (double)len * 2.0 * Math.PI * j) / i;
				}
			}

			// Set magnitude
			for (int j = 0; j < len; j++)
			{
				output[j] = output[j] * mag;
			}

			return output;
		}

		public static double Triangle(double phase)
		{
			phase = (phase + 0.25) % 1.0;
			if (phase < 0.0)
				phase += 1.0;

			if (phase < 0.5)
				return (1.0 - 2 * phase) * -2.0 - 1.0;
			else
				return 2 * (phase - 0.5) * -2.0 - 1.0;
		}

		public static double[] Triangle(int len, double mag)
		{
			int half = len / 2 + 1;

			double[] output = new double[len];

			for (int i = 0; i < half; i++)
				output[i] = mag - 2 * mag * i / (half-1);

			for (int i = half; i < len; i++)
				output[i] = output[2*(half-1) - i];

			return output;
		}

		public static double[] TriangleAdditive(int len, int partials, double mag)
		{
			mag = (double)(4 / Math.PI * mag);
			double[] output = new double[len];

			double p = 1.0f;

			for (int i = 1; i < partials; i = i + 2)
			{
				for (int j = 0; j < len; j++)
				{
					output[j] = output[j] + (double)Math.Sin(i / (double)len * 2.0 * Math.PI * j) * (double)(8.0 / (i*i*Math.PI*Math.PI) * p);
				}

				p = -1.0f * p; // invert signal between coefficients
			}

			return output;
		}


	    public static void ReverseInPlace(double[] sa)
	    {
	        var len = sa.Length;
	        for (int i = 0; i < len / 2; i++)
	        {
	            var temp = sa[i];
	            sa[i] = sa[len - 1 - i];
                sa[len - 1 - i] = temp;
            }
	    }

		public static double[] UnrollPhase(double[] phaseData)
		{
			var output = new double[phaseData.Length];
			var prev = double.NaN;
			var threshold = 1.7 * Math.PI;
			var offset = 0.0;

			for (int i = 0; i < phaseData.Length; i++)
			{
				if (i == 54)
				{
					
				}
				var d = phaseData[i];
				if (double.IsNaN(prev))
				{
					output[i] = d;
					prev = d;
				}
				else
				{
					if (d - prev > threshold)
						offset -= 2 * Math.PI;
					if (prev - d > threshold)
						offset += 2 * Math.PI;

					output[i] = d + offset;
					prev = d;
				}
			}

			return output;

		}
	}
}

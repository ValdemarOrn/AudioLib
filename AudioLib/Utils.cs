using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public class Utils
	{
		public static float ExpResponse(float input)
		{
			return (float)(Math.Pow(20, input)-1)/19;
		}

		public static float LogResponse(float input)
		{
			return 2f*input-(float)(Math.Pow(20, input)-1)/19;

		}

		public static float DB2gain(float input)
		{
			return (float)Math.Pow(10, input/20);
		}

		public static float Gain2DB(float input)
		{
			return (float)(20*Math.Log10(input));
		}

		public static float[] Linspace(float min, float max, int num)
		{
			double space = (max-min)/(num-1);
			float[] output = new float[num];
			output[0] = min;
			double runningVal = min;
			for(int i=1; i<num; i++)
			{
				runningVal = runningVal + space;
				output[i] =  (float)runningVal;
			}

			return output;
		}

		public static float Min(float[] input)
		{
			float min = input[0];
			for(int i=1; i < input.Length; i++)
				if(input[i] < min)
					min = input[i];
		
			return min;
		}

		public static float Max(float[] input)
		{
			float max = input[0];
			for(int i=1; i < input.Length; i++)
				if(input[i] > max)
					max = input[i];

			return max;
		}

		public static float Average(float[] input)
		{
			float ave = 0;
			for(int i=0; i < input.Length; i++)
				ave += input[i];

			return ave / (float)input.Length;
		}
	
		public static float RMS(float[] input)
		{
			float rms = 0;
			for(int i=0; i < input.Length; i++)
				rms += input[i] * input[i];

			return (float)Math.Sqrt(rms /(float)input.Length);
		}

		public static float[] Gain(float[] input, float gain)
		{
			float[] output = new float[input.Length];

			for(int i=0; i < input.Length; i++)
				output[i] = input[i] * gain;

			return output;
		}

		public static float[] Saturate(float[] input, float max)
		{
			return Utils.Saturate(input, -max, max);
		}

		public static float[] Saturate(float[] input, float min, float max)
		{
			float[] output = new float[input.Length];

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

		/// <summary>
		/// Create a Sinc wave
		/// </summary>
		/// <param name="omega">Range between 0...1 (0.5 = Nyquist, fs/2)</param>
		/// <param name="M">size of output is 2M+1</param>
		/// <returns></returns>
		public static float[] Sinc(float omega, int M)
		{
			float[] x = Linspace(-M, M, 2 * M + 1);
			float[] output = new float[2 * M + 1];

			for (int i = 0; i < M; i++)
			{
				output[i] = (float)(Math.Sin(Math.PI * 2.0 * x[i] * omega) / (Math.PI * 2.0 * x[i] * omega));
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
		public static float[] Window(int M, String type)
		{
			float[] n = Linspace(-M, M, 2 * M + 1);
			float[] output = new float[2 * M + 1];

			if (type.ToLower().Equals("bartlett"))
			{
				for (int i = 0; i < output.Length; i++)
					output[i] = (float)((n[i] > 0) ? (1 - n[i] / (M + 1)) : (1 + n[i] / (M + 1)));
			}
			else if (type.ToLower().Equals("hann"))
			{
				for (int i = 0; i < output.Length; i++)
					output[i] = (float)(0.5 * (1 + Math.Cos(2 * Math.PI * n[i] / (2 * M + 1))));
			}
			else if (type.ToLower().Equals("hamming"))
			{
				for (int i = 0; i < output.Length; i++)
					output[i] = (float)(0.54 + 0.46 * Math.Cos(2 * Math.PI * n[i] / (2 * M + 1)));
			}
			else if (type.ToLower().Equals("blackman"))
			{
				for (int i = 0; i < output.Length; i++)
					output[i] = (float)(0.42 + 0.5 * Math.Cos(2 * Math.PI * n[i] / (2 * M + 1)) + 0.08 * Math.Cos(4 * Math.PI * n[i] / (2 * M + 1)));
			}
			else // Rect window
			{
				for (int i = 0; i < output.Length; i++)
					output[i] = 1;
			}

			return output;
		}

		public static float[] SincFilter(float wc, int M, String type)
		{
			float[] a = Sinc(wc, M);
			float[] b = Window(M, type);

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
		public static float[] Sinewave(int len, float phase, float rad, float mag)
		{
			float[] output = new float[len];

			for(int i=0; i<output.Length; i++)
			{
				output[i] = (float)Math.Sin(phase + i*rad) * mag;
			}

			return output;
		}

		public static float[] Saw(int len, float mag)
		{
			float unit = 2 * mag / (len-1);
			float[] output = new float[len];
			for (int i = 0; i < len; i++)
				output[i] = (mag - i * unit);

			return output;
		}

		public static float[] SawAdditive(int len, int partials, float mag)
		{
			mag = (float)(2 / Math.PI * mag);
			float[] output = new float[len];

			for (int i = 1; i < partials; i++)
			{
				for (int j = 0; j < len; j++)
				{
					output[j] = output[j] + (float)Math.Sin(i / (double)len * 2.0 * Math.PI * j) / i;
				}
			}

			// Set magnitude
			for (int j = 0; j < len; j++)
			{
				output[j] = output[j] * mag;
			}

			return output;
		}

		public static float[] Square(int len, float mag, float width)
		{
			int highSample = (int)(width * len + 0.5f);
			int startSample = (len - highSample)/2;

			float[] output = new float[len];
			for (int i = 0; i < len; i++)
			{
				if (i >= startSample && i < (startSample + highSample))
					output[i] = mag;
				else
					output[i] = -mag;
			}

			return output;
		}

		public static float[] SquareAdditive(int len, int partials, float mag)
		{
			mag = (float)(4 / Math.PI * mag);
			float[] output = new float[len];

			for (int i = 1; i < partials; i=i+2)
			{
				for (int j = 0; j < len; j++)
				{
					output[j] = output[j] + (float)Math.Sin(i / (double)len * 2.0 * Math.PI * j) / i;
				}
			}

			// Set magnitude
			for (int j = 0; j < len; j++)
			{
				output[j] = output[j] * mag;
			}

			return output;
		}

		public static float[] Triangle(int len, float mag)
		{
			int half = len / 2 + 1;

			float[] output = new float[len];

			for (int i = 0; i < half; i++)
				output[i] = mag - 2 * mag * i / (half-1);

			for (int i = half; i < len; i++)
				output[i] = output[2*(half-1) - i];

			return output;
		}

		public static float[] TriangleAdditive(int len, int partials, float mag)
		{
			mag = (float)(4 / Math.PI * mag);
			float[] output = new float[len];

			float p = 1.0f;

			for (int i = 1; i < partials; i = i + 2)
			{
				for (int j = 0; j < len; j++)
				{
					output[j] = output[j] + (float)Math.Sin(i / (double)len * 2.0 * Math.PI * j) * (float)(8.0 / (i*i*Math.PI*Math.PI) * p);
				}

				p = -1.0f * p; // invert signal between coefficients
			}

			return output;
		}

		
		
	}
}

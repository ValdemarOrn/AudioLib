using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;

namespace AudioLib
{
	public class WaveStack
	{
		public int NumPartials { get { return Waves.Count; } }
		public int[] PartialsInWave;

		public float[] MasterWave;

		public List<float[]> Waves;

		public WaveStack(float[] wave = null)
		{
			if (wave == null)
				wave = Utils.Sinewave(2048, 0.0f, (float)(1.0 / 2048 * 2 * Math.PI), 1.0f);
			SetWave(wave);
		}

		/// <summary>
		/// set a new wave as the master wave, calculate band-limited 
		/// versions of it and put them in the wave stack.
		/// </summary>
		/// <param name="wave">
		/// The high resolution wave that will be filtered and downsampled. 
		/// Should be 1024 samples or larger.
		/// </param>
		public void SetWave(float[] wave)
		{
			this.MasterWave = wave;
			this.PartialsInWave = findMaxPartials(256, (float)(17.5/21.0), 50000);
			this.Waves = new List<float[]>();

			var s = new Stopwatch();
			s.Reset();
			s.Start();
			List<double[]> dft = SimpleDFT.DFT(wave.Select(x => (double)x).ToArray());
			s.Stop();
			MessageBox.Show("millisec: " + s.ElapsedMilliseconds);
			var cos = dft[0];
			var sin = dft[1];

			float normalizer = 1.0f / cos.Length * 2.0f;
			for (int i = 0; i < cos.Length; i++)
			{
				cos[i] = cos[i] * normalizer;
				sin[i] = sin[i] * normalizer;
			}
			s.Reset();
			s.Start();
			for (int k = 0; k < PartialsInWave.Length; k++)
			{
				int partials = PartialsInWave[k];
				
				int size = 512;

				if (partials > 256)
					partials = 256;
				if (partials <= 64)
					size = partials*8;
				if (size < 64)
					size = 64;

				double[] output = new double[size];

				// copy of code from IDFT
				for (int n = 0; n <= partials; n++)
				{
					for (int i = 0; i < size; i++)
					{
						output[i] += Math.Cos(2 * Math.PI * n / size * i) * cos[n];
						output[i] += Math.Sin(2 * Math.PI * n / size * i) * sin[n];
					}
				}

				// double -> float. I used to for extra precision to minimize errors
				float[] output2 = new float[size];
				for (int i = 0; i < size; i++)
					output2[i] += (float)output[i];
				
				this.Waves.Add(output2);
			}
			s.Stop();
			MessageBox.Show("msec: " + s.ElapsedMilliseconds);
		}

		/// <summary>
		/// Calculate the best distribution of number of partials for the wavetable.
		/// Using less and less partials as the frequency goes up prevents aliasing
		/// </summary>
		/// <param name="maxPartials">the number of partials to start with (usually 64 or 128)</param>
		/// <param name="minFreq">the minimum frequency, relative to Fs/2 that is acceptable</param>
		/// <param name="steps">number of steps to run. Higher is more accurate</param>
		/// <returns></returns>
		public static int[] findMaxPartials(int maxPartials, float minFreq, int steps)
		{
			List<int> output = new List<int>();

			int currentPartials = maxPartials;
			float f = 1.0f / steps * 1;

			// Add initial value, if its below the threshold
			if (f * currentPartials < 1.0f)
				output.Add(currentPartials);

			for (int i = 1; i < steps; i++)
			{

				f = 1.0f / steps * i;

				if (f * currentPartials > 1.0f)
				{
					currentPartials--;

					while (f * currentPartials > minFreq)
						currentPartials--;

					// We have now gone below minFreq. so we must add one, give that we are allowed to do so.
					// otherwise, live with the large step (happens at very high frequencies, not we can do about it)
					if (f * (currentPartials + 1) < 1.0f)
						currentPartials++;

					output.Add(currentPartials);
				}
			}

			return output.ToArray();
		}

		/// <summary>
		/// Gets a sample from the wavetable. the methods selects a table from the stack that is
		/// appropriate for the pitch (prevents antialiasing)
		/// </summary>
		/// <param name="pos">the position within the table. 0..1</param>
		/// <param name="frequency">the frequency in rad/sex, 1.0 corresponding to 2*Pi rad/sec = Fs. Max 0.5 = Fs/2</param>
		/// <returns></returns>
		public float GetSample(float pos, float frequency)
		{
			int maxPartials = (int)(0.5 / frequency + 0.000001);
			float[] wave;

			// --------------------------------------
			// Find the correct wave in the stack

			// tighten the search with binary-search, make the wave-lookup loop below faster
			int q = PartialsInWave.Length/2;
			if (PartialsInWave[q] < maxPartials)
			{
				q = q / 2;
				if (PartialsInWave[q] < maxPartials)
					q = 0;
			}
			else
			{
				q = q + q / 2;
				if (PartialsInWave[q] < maxPartials)
					q = PartialsInWave.Length / 2;
			}


			while (true)
			{
				// unrolled loop
				if (PartialsInWave[q] <= maxPartials)
				{
					wave = Waves[q];
					break;
				}
				if (PartialsInWave[q+1] <= maxPartials)
				{
					wave = Waves[q+1];
					break;
				}
				if (PartialsInWave[q+2] <= maxPartials)
				{
					wave = Waves[q+2];
					break;
				}
				if (PartialsInWave[q+3] <= maxPartials)
				{
					wave = Waves[q+3];
					break;
				}
				if (PartialsInWave[q+4] <= maxPartials)
				{
					wave = Waves[q+4];
					break;
				}
				if (PartialsInWave[q+5] <= maxPartials)
				{
					wave = Waves[q+5];
					break;
				}
				if (PartialsInWave[q+6] <= maxPartials)
				{
					wave = Waves[q+6];
					break;
				}
				if (PartialsInWave[q+7] <= maxPartials)
				{
					wave = Waves[q+7];
					break;
				}

				q += 8;
				if (q >= PartialsInWave.Length)
					throw new Exception("no matching partials! q is " + q + ", maxPartials is " + maxPartials
						+ ", smallest available partial is " + PartialsInWave[PartialsInWave.Length - 1]);
			}

			// Find the correct sample in the wave (using spline interpolation)
			float output = Interpolate.SplineWrap(pos, wave);
			return output;
		}
	}
}

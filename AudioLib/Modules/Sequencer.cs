using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Modules
{
	public sealed class Sequencer
	{
		private double SamplePosition;

		public int StepIterator { get; private set; }
		public List<SequencerTrack> Tracks;

		public double BPM;
		public double SampleRate;
		public int StepType;

		public Sequencer()
		{
			StepType = 16;
			Tracks = new List<SequencerTrack>();
		}

		public Sequencer(double bpm, double samplerate) : this()
		{
			BPM = bpm;
			SampleRate = samplerate;
		}

		public void SetSamplePosition(double newPosition)
		{
			SamplePosition = newPosition;
		}

		/// <summary>
		/// returns true is the samplePosition is advanced to the next step, triggering a sequence step
		/// </summary>
		/// <param name="sampleCount"></param>
		/// <returns></returns>
		public bool AddSamples(int sampleCount)
		{
			if (SamplePosition == 0)
			{
				SamplePosition += (double)sampleCount;
				return true;
			}

			SamplePosition += (double)sampleCount;
		   
			var oldIterator = StepIterator;
			var newPos = SamplePosition / SampleRate * BPM / 60.0 * StepType / 4.0;

			StepIterator = (int)newPos;

			if (StepIterator != oldIterator)
				return true;
			else
				return false;
		}

		public List<Step> GetStepData(int offset = 0)
		{
			var output = new List<Step>();
			foreach (var track in Tracks)
				output.Add(track.GetStep(offset));

			return output;
		}
	}
}

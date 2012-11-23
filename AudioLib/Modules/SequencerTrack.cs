using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioLib.Modules
{
	public sealed class SequencerTrack
	{
		public Sequencer Parent { get; private set; }

		int StepCount;

		public int CurrentStep
		{
			get
			{
				return Parent.StepIterator % StepCount;
			}
		}

		public int TrackNumber
		{
			get
			{
				return Parent.Tracks.IndexOf(this);
			}
		}

		private List<Step> Steps;

		public SequencerTrack()
		{
			Steps = new List<Step>();
			for(int i=0; i<64; i++)
				Steps.Add(new Step());
		}

		public SequencerTrack(Sequencer parent, int stepCount) : this()
		{
			Parent = parent;
			StepCount = stepCount;
		}

		public void SetStepCount(int count)
		{
			if (count <= 0 || count > 64)
				throw new ArgumentException("Step count must be between 1 and 64");

			StepCount = count;
		}

		public Step GetStep(int offset = 0)
		{
			int idx = (CurrentStep + offset + 100 * StepCount) % StepCount;

			var step = Steps[idx];
			step.StepNumber = idx;
			return step;
		}
	}
}

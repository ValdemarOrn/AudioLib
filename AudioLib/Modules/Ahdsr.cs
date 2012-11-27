using AudioLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Modules
{
	public sealed class Ahdsr
	{
		public Ahdsr(double samplerate)
		{
			Samplerate = samplerate;
			// no sound at beginning
			ReleasePosition = 100000;
		}

		public double Samplerate;

		#region AHDSR properties 

		double _attack;
		double _attackSamples;
		/// <summary>
		/// Value in milliseconds
		/// </summary>
		public double Attack
		{
			get { return _attack; }
			set
			{
				if (value == 0)
					value = value + 1;
				_attack = value;
				_attackSamples = _attack / 1000.0 * Samplerate;
			}
		}

		double _decay;
		double _decaySamples;
		/// <summary>
		/// Value in milliseconds
		/// </summary>
		public double Decay
		{
			get { return _decay; }
			set
			{
				if (value == 0)
					value = value + 1;
				_decay = value;
				_decaySamples = _decay / 1000.0 * Samplerate;
			}
		}

		double _hold;
		double _holdSamples;
		/// <summary>
		/// Value in milliseconds
		/// </summary>
		public double Hold
		{
			get { return _hold; }
			set
			{
				if (value == 0)
					value = value + 1;
				_hold = value;
				_holdSamples = _hold / 1000.0 * Samplerate;
			}
		}

		/// <summary>
		/// Sustain amount, 0...1
		/// </summary>
		public double Sustain { get; set; }

		double _release;
		double _releaseSamples;
		/// <summary>
		/// Value in milliseconds
		/// </summary>
		public double Release
		{
			get { return _release; }
			set
			{
				if (value == 0)
					value = value + 1;
				_release = value;
				_releaseSamples = _release / 1000.0 * Samplerate;
			}
		}

		#endregion

		bool _gate;
		public bool Gate
		{
			get { return _gate; }
			set
			{
				if (_gate == value && !Retrigger)
					return;

				// get current value before we change the gate
				var currentVal = GetValue();

				_gate = value;
				if(_gate) // turning on, it was off
				{
					Node = 0;
					if (SmoothTransition)
						NodePosition = ValueTables.FindIndex(currentVal, attackTable); // smooth transition from release phase to attack phase. No jump in value
					else
						NodePosition = 0;

					ReleasePosition = 0.0;
				}
			}
		}

		/// <summary>
		/// If set to true there is no jump when transitioning from release phase to attack phase
		/// (when envelope is reset before release phase is complete). Otherwise the attack starts at zero.
		/// </summary>
		public bool SmoothTransition;

		/// <summary>
		/// Set to true for an exponential decay mode
		/// </summary>
		public bool ExponentialDecay;

		/// <summary>
		/// Set to true if the envelope should start from the beginning each time the Gate is set
		/// </summary>
		public bool Retrigger;

		double _atkShape;
		/// <summary>
		/// set between 0..1 to change the curvature of the attack, 0.5 = straight line
		/// </summary>
		public double AttackShape
		{
			get { return _atkShape; }
			set
			{
				_atkShape = value;

				if (value < 0.2)
					attackTable = ValueTables.Sqrt3;
				else if (value < 0.4)
					attackTable = ValueTables.Sqrt;
				else if (value < 0.6)
					attackTable = null;
				else if (value < 0.8)
					attackTable = ValueTables.Pow2;
				else
					attackTable = ValueTables.Pow3;
			}
		}

		double _decShape;
		/// <summary>
		/// set between 0..1 to change the curvature of the attack, 0.5 = straight line
		/// </summary>
		public double DecayShape
		{
			get { return _decShape; }
			set
			{
				_decShape = value;

				if (value < 0.2)
					decayTable = ValueTables.Sqrt3;
				else if (value < 0.4)
					decayTable = ValueTables.Sqrt;
				else if (value < 0.6)
					decayTable = null;
				else if (value < 0.8)
					decayTable = ValueTables.Pow2;
				else
					decayTable = ValueTables.Pow3;
			}
		}

		double _relShape;
		/// <summary>
		/// set between 0..1 to change the curvature of the attack, 0.5 = straight line
		/// </summary>
		public double ReleaseShape
		{
			get { return _relShape; }
			set
			{
				_relShape = value;

				if (value < 0.2)
					releaseTable = ValueTables.Sqrt3;
				else if (value < 0.4)
					releaseTable = ValueTables.Sqrt;
				else if (value < 0.6)
					releaseTable = null;
				else if (value < 0.8)
					releaseTable = ValueTables.Pow2;
				else
					releaseTable = ValueTables.Pow3;
			}
		}

		/// <summary>
		/// Node says which part of the envelope we are currently in.
		/// 0 = Attack phase
		/// 1 = Hold phase
		/// 2 = Decay phase
		/// 3 = Sustain phase
		/// Release phase is calculated separate
		/// </summary>
		int Node;

		/// <summary>
		/// Says how far we have progressed from the previous node to the next.
		/// Value in range 0...1
		/// </summary>
		double NodePosition;

		/// <summary>
		/// Says how far the release phase has progressed
		/// Value in range 0...n (exponential decay)
		/// </summary>
		double ReleasePosition;

		public double Process(double progressSamples)
		{
			if (Gate)
			{
				if (Node == 0)
					NodePosition += progressSamples / _attackSamples;
				else if (Node == 1)
					NodePosition += progressSamples / _holdSamples;
				else if (Node == 2)
					NodePosition += progressSamples / _decaySamples;
				else if (Node == 3)
					NodePosition = 0;

				// transition into next node
				if (NodePosition >= 1.0)
				{
					NodePosition = 0.0;
					Node++;
				}
			}
			else // release phase
			{
				ReleasePosition += progressSamples / _releaseSamples;
			}

			Output = GetValue();
			return Output;
		}

		public double Output;

		// shape lookup tables for attack, decay and release curves
		double[] attackTable = null;
		double[] decayTable = null;
		double[] releaseTable = null;

		double GetValue()
		{
			double val = 0.0;

			if (Node == 0)
				val = ValueTables.Get(NodePosition, attackTable);
			else if (Node == 1)
				val = 1.0;
			else if (Node == 2)
				val = Sustain + ValueTables.Get(1.0 - NodePosition, decayTable) * (1 - Sustain);
			else if (Node == 3)
				val = Sustain;

			if (!Gate)
			{
				if (ExponentialDecay)
					val = val * 1 / (1 + ReleasePosition);
				else if (ReleasePosition < 1.0)
					val = val * ValueTables.Get(1.0 - ReleasePosition, releaseTable);
				else
					val = 0.0;
			}

			return val;
		}

	}
}

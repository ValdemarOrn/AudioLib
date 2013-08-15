using AudioLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Modules
{
	public sealed class Ahdsr
	{
		public const int StageAttack = 1;
		public const int StageHold = 2;
		public const int StageDecay = 3;
		public const int StageSustain = 4;
		public const int StageRelease = 5;

		public double Output;

		public double Samplerate;
		public bool Gate;

		double Attack;
		double Hold;
		double Decay;
		double Sustain;
		double Release;

		double AttackIncr;
		double HoldIncr;
		double DecayIncr;
		double ReleaseIncr;
		double SustainAccLevel;
		
		double Accumulator;
		int Stage;

		/// <summary>
		/// If set to true there is no jump when transitioning from release phase to attack phase
		/// (when envelope is reset before release phase is complete). Otherwise the attack starts at zero.
		/// </summary>
		public bool SmoothTransition;

		public Ahdsr(double samplerate)
		{
			Samplerate = samplerate;

			Stage = StageRelease;
			SetParameter(StageAttack, 1);
			SetParameter(StageHold, 1);
			SetParameter(StageDecay, 64);
			SetParameter(StageSustain, 64);
			SetParameter(StageRelease, 64);
		}
		/*
		#region Shapes

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
		/// Says how far we have progressed from the previous node to the next.
		/// Value in range 0...1
		/// </summary>
		double NodePosition;

		/// <summary>
		/// Says how far the release phase has progressed
		/// Value in range 0...n (exponential decay)
		/// </summary>
		double ReleasePosition;

		#endregion
		*/
		void SetParameter(int stage, double valueMillisOrLevel)
		{
			var value = valueMillisOrLevel;

			double samples = value * Samplerate + 1; // add one to prevent /0
			double inc = (1.0 / samples);

			switch (stage)
			{
				case StageAttack:
					Attack = value;
					AttackIncr = inc;
					break;
				case StageDecay:
					Decay = value;
					DecayIncr = samples;
					// adjust decay rate to sustain level
					DecayIncr = DecayIncr * (1 - Sustain);
					break;
				case StageSustain:
					Sustain = value;
					SustainAccLevel = value;
					SetParameter(StageDecay, Decay); // recalulate decay increment
					break;
				case StageRelease:
					Release = value;
					ReleaseIncr = inc;
					break;
			}
		}

		public double Process(double sampleCount)
		{
			if (!Gate)
				Stage = StageRelease;

			double previous = Accumulator;

			if (Stage == StageAttack)
			{
				Accumulator += AttackIncr * sampleCount;
				if (Accumulator > 1)
				{
					Stage = StageDecay;
					Accumulator = 1 - (Accumulator - 1);
				}
			}
			else if (Stage == StageDecay)
			{
				Accumulator -= (DecayIncr * sampleCount);
				if (Accumulator < 0 || Accumulator <= SustainAccLevel)
				{
					Stage = StageSustain;
					Accumulator = SustainAccLevel;
				}
			}
			else if (Stage == StageSustain)
			{
				Accumulator = SustainAccLevel;
			}
			else if (Stage == StageRelease)
			{
				Accumulator -= (ReleaseIncr * sampleCount);
				if (Accumulator < 0)
				{
					Accumulator = 0;
				}

				if (Gate)
					Stage = StageAttack;
			}

			Output = Accumulator;
			return Output;
		}

		// shape lookup tables for attack, decay and release curves
		/*double[] attackTable = null;
		double[] decayTable = null;
		double[] releaseTable = null;*/

	}
}

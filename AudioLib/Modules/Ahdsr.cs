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

		bool LastGate; // gate state last time we called process()
		double ReleaseLevel;
		
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
			SetParameter(StageAttack, 2);
			SetParameter(StageHold, 2);
			SetParameter(StageDecay, 200);
			SetParameter(StageSustain, 0.5);
			SetParameter(StageRelease, 30);
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
		*/

		/// <summary>
		/// Sets AHDSR parameters, in milliseconds (except for sustain, which is 0...1)
		/// </summary>
		public void SetParameter(int stage, double valueMillisOrLevel)
		{
			var value = valueMillisOrLevel;

			double samples = (value * 0.001) * Samplerate + 1; // add one to prevent /0
			double inc = (1.0 / samples);

			switch (stage)
			{
				case StageAttack:
					Attack = value;
					AttackIncr = inc;
					break;
				case StageDecay:
					Decay = value;
					DecayIncr = inc;
					break;
				case StageHold:
					Hold = value;
					HoldIncr = inc;
					break;
				case StageSustain:
					Sustain = value;
					break;
				case StageRelease:
					Release = value;
					ReleaseIncr = inc;
					break;
			}
		}

		public double Process(double sampleCount)
		{
			if (LastGate && !Gate) // turning gate off
			{
				ReleaseLevel = Output;
				Stage = StageRelease;
				Accumulator = 0;
			}

			if (Stage == StageAttack)
			{
				Accumulator += AttackIncr * sampleCount;
				if (Accumulator > 1)
				{
					Stage = StageHold;
					Accumulator = (Accumulator - 1);
				}
			}
			else if (Stage == StageHold)
			{
				Accumulator += HoldIncr * sampleCount;
				if (Accumulator > 1)
				{
					Stage = StageDecay;
					Accumulator = (Accumulator - 1);
				}
			}
			else if (Stage == StageDecay)
			{
				Accumulator += DecayIncr * sampleCount;
				if (Accumulator > 1)
				{
					Stage = StageSustain;
					Accumulator = 0;
				}
			}
			else if (Stage == StageSustain)
			{
				// do nothing
			}
			else if (Stage == StageRelease)
			{
				Accumulator += ReleaseIncr * sampleCount;
				if (Accumulator > 1)
					Accumulator = 1;

				if (!LastGate && Gate) // turning on
				{
					Stage = StageAttack;
					Accumulator = Output;
				}
			}


			if (Stage == StageAttack)
				Output = Accumulator;
			else if (Stage == StageHold)
				Output = 1;
			else if (Stage == StageDecay)
				Output = (1 - Accumulator) * (1 - Sustain) + Sustain;
			else if (Stage == StageSustain)
				Output = Sustain;
			else if (Stage == StageRelease)
				Output = ReleaseLevel * (1 - Accumulator);

			LastGate = Gate;
			return Output;
		}

		public void Reset()
		{
			Stage = StageAttack;
			Accumulator = 0;
		}

		// shape lookup tables for attack, decay and release curves
		/*double[] attackTable = null;
		double[] decayTable = null;
		double[] releaseTable = null;*/

	}
}

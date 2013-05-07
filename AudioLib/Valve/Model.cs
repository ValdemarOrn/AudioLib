using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Valve
{
	public class ModelParameters
	{
		public double Mu;
		public double Ex;
		public double Kg1;
		public double Kg2;
		public double Kp;
		public double Kvb;
		public double Ccg;
		public double Cpg;
		public double Ccp;
		public double Rgi;

		/// <summary>
		/// Artificially lower the saturation point. Used in the SaturateEg method
		/// </summary>
		public double SaturationPoint;

		/// <summary>
		/// The radius of the knee when using an artificial saturation point
		/// </summary>
		public double Knee;
	}

	public class Models
	{
		public static Triode _12AX7
		{
			get
			{
				var tube = new Triode();
				tube.Parameters.Mu		= 100;
				tube.Parameters.Ex		= 1.4;
				tube.Parameters.Kg1		= 1060;
				tube.Parameters.Kp		= 600;
				tube.Parameters.Kvb		= 300;
				
				tube.Parameters.Ccg		= 2.3e-12;
				tube.Parameters.Cpg		= 2.4e-12;
				tube.Parameters.Ccp		= 0.9e-12;
				tube.Parameters.Rgi = 2000;

				tube.Parameters.SaturationPoint = 999;
				tube.Parameters.Knee = 3;
				return tube;
			}
		}

		public static Triode _12AU7
		{
			get
			{
				var tube = new Triode();
				tube.Parameters.Mu = 21.5;
				tube.Parameters.Ex = 1.3;
				tube.Parameters.Kg1 = 1180;
				tube.Parameters.Kp = 84;
				tube.Parameters.Kvb = 300;
				
				tube.Parameters.Ccg = 2.3e-12;
				tube.Parameters.Cpg = 2.4e-12;
				tube.Parameters.Ccp = 1.0e-12;
				tube.Parameters.Rgi = 2000;

				tube.Parameters.SaturationPoint = 999;
				tube.Parameters.Knee = 3;
				return tube;
			}
		}

		public static Triode _6550
		{
			get
			{
				var tube = new Triode();
				tube.Parameters.Mu = 7.9;
				tube.Parameters.Ex = 1.35;
				tube.Parameters.Kg1 = 890;
				tube.Parameters.Kg2 = 4800;
				tube.Parameters.Kp = 60;
				tube.Parameters.Kvb = 24;

				tube.Parameters.Ccg = 14e-12;
				tube.Parameters.Cpg = 0.85e-12;
				tube.Parameters.Ccp = 14e-12;
				tube.Parameters.Rgi = 1000;

				tube.Parameters.SaturationPoint = 999;
				tube.Parameters.Knee = 3;
				return tube;
			}
		}

		public static Triode _KT88
		{
			get
			{
				var tube = new Triode();
				tube.Parameters.Mu = 8.8;
				tube.Parameters.Ex = 1.35;
				tube.Parameters.Kg1 = 730;
				tube.Parameters.Kg2 = 4200;
				tube.Parameters.Kp = 32;
				tube.Parameters.Kvb = 16;

				tube.Parameters.Ccg = 14e-12;
				tube.Parameters.Cpg = 0.85e-12;
				tube.Parameters.Ccp = 14e-12;
				tube.Parameters.Rgi = 1000;

				tube.Parameters.SaturationPoint = 999;
				tube.Parameters.Knee = 3;
				return tube;
			}
		}

		public static Triode _6L6CG
		{
			get
			{
				var tube = new Triode();
				tube.Parameters.Mu = 9.88;
				tube.Parameters.Ex = 1.442;
				tube.Parameters.Kg1 = 1686;
				tube.Parameters.Kg2 = 4500;
				tube.Parameters.Kp = 30.98;
				tube.Parameters.Kvb = 19.4;

				tube.Parameters.Ccg = 10e-12;
				tube.Parameters.Cpg = 0.6e-12;
				tube.Parameters.Ccp = 6.5e-12;
				tube.Parameters.Rgi = 1000;

				tube.Parameters.SaturationPoint = 999;
				tube.Parameters.Knee = 3;
				return tube;
			}
		}

		public static Triode _EL84
		{
			get
			{
				var tube = new Triode();
				tube.Parameters.Mu = 21.29;
				tube.Parameters.Ex = 1.240;
				tube.Parameters.Kg1 = 401.7;
				tube.Parameters.Kg2 = 4500;
				tube.Parameters.Kp = 111.04;
				tube.Parameters.Kvb = 17.9;

				tube.Parameters.Ccg = 10e-12;
				tube.Parameters.Cpg = 0.6e-12;
				tube.Parameters.Ccp = 5.1e-12;
				tube.Parameters.Rgi = 1000;

				tube.Parameters.SaturationPoint = 999;
				tube.Parameters.Knee = 3;
				return tube;
			}
		}

		public static Triode _EL34
		{
			get
			{
				var tube = new Triode();
				tube.Parameters.Mu = 11.52;
				tube.Parameters.Ex = 1.350;
				tube.Parameters.Kg1 = 608.9;
				tube.Parameters.Kg2 = 4500;
				tube.Parameters.Kp = 41.16;
				tube.Parameters.Kvb = 30.1;

				tube.Parameters.Ccg = 15e-12;
				tube.Parameters.Cpg = 1.0e-12;
				tube.Parameters.Ccp = 8.0e-12;
				tube.Parameters.Rgi = 1000;

				tube.Parameters.SaturationPoint = 999;
				tube.Parameters.Knee = 3;
				return tube;
			}
		}
	}
}

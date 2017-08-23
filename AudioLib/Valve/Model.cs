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
				tube.Model.Mu		= 100;
				tube.Model.Ex		= 1.4;
				tube.Model.Kg1		= 1060;
				tube.Model.Kp		= 600;
				tube.Model.Kvb		= 300;
				
				tube.Model.Ccg		= 2.3e-12;
				tube.Model.Cpg		= 2.4e-12;
				tube.Model.Ccp		= 0.9e-12;
				tube.Model.Rgi = 2000;

				tube.Model.SaturationPoint = 999;
				tube.Model.Knee = 3;
				return tube;
			}
		}

		public static Triode _12AU7
		{
			get
			{
				var tube = new Triode();
				tube.Model.Mu = 21.5;
				tube.Model.Ex = 1.3;
				tube.Model.Kg1 = 1180;
				tube.Model.Kp = 84;
				tube.Model.Kvb = 300;
				
				tube.Model.Ccg = 2.3e-12;
				tube.Model.Cpg = 2.4e-12;
				tube.Model.Ccp = 1.0e-12;
				tube.Model.Rgi = 2000;

				tube.Model.SaturationPoint = 999;
				tube.Model.Knee = 3;
				return tube;
			}
		}

		public static Triode _6550
		{
			get
			{
				var tube = new Triode();
				tube.Model.Mu = 7.9;
				tube.Model.Ex = 1.35;
				tube.Model.Kg1 = 890;
				tube.Model.Kg2 = 4800;
				tube.Model.Kp = 60;
				tube.Model.Kvb = 24;

				tube.Model.Ccg = 14e-12;
				tube.Model.Cpg = 0.85e-12;
				tube.Model.Ccp = 14e-12;
				tube.Model.Rgi = 1000;

				tube.Model.SaturationPoint = 999;
				tube.Model.Knee = 3;
				return tube;
			}
		}

		public static Triode _KT88
		{
			get
			{
				var tube = new Triode();
				tube.Model.Mu = 8.8;
				tube.Model.Ex = 1.35;
				tube.Model.Kg1 = 730;
				tube.Model.Kg2 = 4200;
				tube.Model.Kp = 32;
				tube.Model.Kvb = 16;

				tube.Model.Ccg = 14e-12;
				tube.Model.Cpg = 0.85e-12;
				tube.Model.Ccp = 14e-12;
				tube.Model.Rgi = 1000;

				tube.Model.SaturationPoint = 999;
				tube.Model.Knee = 3;
				return tube;
			}
		}

		public static Triode _6L6CG
		{
			get
			{
				var tube = new Triode();
				tube.Model.Mu = 9.88;
				tube.Model.Ex = 1.442;
				tube.Model.Kg1 = 1686;
				tube.Model.Kg2 = 4500;
				tube.Model.Kp = 30.98;
				tube.Model.Kvb = 19.4;

				tube.Model.Ccg = 10e-12;
				tube.Model.Cpg = 0.6e-12;
				tube.Model.Ccp = 6.5e-12;
				tube.Model.Rgi = 1000;

				tube.Model.SaturationPoint = 999;
				tube.Model.Knee = 3;
				return tube;
			}
		}

		public static Triode _EL84
		{
			get
			{
				var tube = new Triode();
				tube.Model.Mu = 21.29;
				tube.Model.Ex = 1.240;
				tube.Model.Kg1 = 401.7;
				tube.Model.Kg2 = 4500;
				tube.Model.Kp = 111.04;
				tube.Model.Kvb = 17.9;

				tube.Model.Ccg = 10e-12;
				tube.Model.Cpg = 0.6e-12;
				tube.Model.Ccp = 5.1e-12;
				tube.Model.Rgi = 1000;

				tube.Model.SaturationPoint = 999;
				tube.Model.Knee = 3;
				return tube;
			}
		}

		public static Triode _EL34
		{
			get
			{
				var tube = new Triode();
				tube.Model.Mu = 11.52;
				tube.Model.Ex = 1.350;
				tube.Model.Kg1 = 608.9;
				tube.Model.Kg2 = 4500;
				tube.Model.Kp = 41.16;
				tube.Model.Kvb = 30.1;

				tube.Model.Ccg = 15e-12;
				tube.Model.Cpg = 1.0e-12;
				tube.Model.Ccp = 8.0e-12;
				tube.Model.Rgi = 1000;

				tube.Model.SaturationPoint = 999;
				tube.Model.Knee = 3;
				return tube;
			}
		}
	}
}

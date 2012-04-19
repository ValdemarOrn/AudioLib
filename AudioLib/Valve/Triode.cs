using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Valve
{
	public class Triode
	{
		public ModelParameters Parameters;
		public double V;
		public double Rc;

		public Triode()
		{
			Parameters = new ModelParameters();
			V = 400;
			Rc = 100e3;
		}

		public double GetCurrent(double Ep, double Eg)
		{
			double Mu = Parameters.Mu;
			double Ex = Parameters.Ex;
			double Kg1 = Parameters.Kg1;
			double Kg2 = Parameters.Kg2;
			double Kp = Parameters.Kp;
			double Kvb = Parameters.Kvb;
			double Ccg = Parameters.Ccg;
			double Cpg = Parameters.Cpg;
			double Ccp = Parameters.Ccp;
			double Rgi = Parameters.Rgi;

			double E1 = (Ep/Kp)*Math.Log(1 + Math.Exp(Kp*(1/Mu + Eg / Math.Sqrt(Kvb + Ep*Ep))));
			double Ip = (Math.Pow(E1, Ex)/Kg1)*2;

			return Ip;
		}

		/// <summary>
		/// Solve the current in the valve provided a specific grid voltage Eg, V and Rc
		/// </summary>
		/// <param name="Eg"></param>
		/// <returns></returns>
		public double SolveCurrent(double Eg)
		{
			Eg = SaturateEg(Eg);

			double Emin = 0;
			double Emax = V;

			while (true)
			{
				double Ep = 0.5 * (Emin + Emax);

				double Ip = GetCurrent(Ep , Eg);
				double Ip2 = (V - Ep) / Rc;

				if (Math.Abs(Ip - Ip2) < 0.000000001)
					return Ip;

				if (Ip > Ip2)
					Emax = Ep;
				else
					Emin = Ep;
			}
		}

		/// <summary>
		/// Simulates the effects of the diode in the Koren Model.
		/// Saturates the ingput voltage when it enters saturation (Eg > 0)
		/// </summary>
		/// <param name="Eg"></param>
		/// <returns></returns>
		public double SaturateEg(double Eg)
		{
			double factor = Parameters.Knee;
			double start = 1/factor;
			double cutoff = Parameters.SaturationPoint;

			if (Eg < cutoff)
				return Eg;
			else
				return Math.Log(start + (Eg - cutoff)) / factor - Math.Log(start) / factor + cutoff;
		}
	}
}

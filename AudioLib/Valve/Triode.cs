using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Valve
{
	public class Triode
	{
		public ModelParameters Model;
		public double V;
		public double Rc;

		public Triode()
		{
			Model = new ModelParameters();
			V = 400;
			Rc = 100e3;
		}

		public double GetCurrent(double Ep, double Eg)
		{
			double mu = Model.Mu;
			double ex = Model.Ex;
			double kg1 = Model.Kg1;
			double kp = Model.Kp;
			double kvb = Model.Kvb;

			double e1 = (Ep/kp)*Math.Log(1 + Math.Exp(kp*(1/mu + Eg / Math.Sqrt(kvb + Ep*Ep))));
			double ip = (Math.Pow(e1, ex)/kg1)*2;

			return ip;
		}

		/// <summary>
		/// Solve the current in the valve provided a specific grid voltage Eg, V and Rc
		/// </summary>
		/// <param name="Eg"></param>
		/// <returns></returns>
		public double SolveCurrent(double Eg)
		{
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
	}
}

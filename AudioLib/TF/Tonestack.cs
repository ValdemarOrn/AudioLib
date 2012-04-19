using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.TF
{
	public class Tonestack : TransferVariable
	{
		public const int P_BASS = 0;
		public const int P_MID = 1;
		public const int P_TREBLE = 2;

		public double[] component;

		// A bit of a hack, don't want to make things complicated
		public bool FenderMode = false;

		public Tonestack(float fs) : base(fs, 3)
		{
			component = new double[9];
		}

		/**
		 * Set the component values
		 * @param C1
		 * @param C2
		 * @param C3
		 * @param Ri	Input resistance
		 * @param Ro	Output resistance
		 * @param R2
		 * @param T		Treble pot Value
		 * @param M		Mid pot Value
		 * @param B		Bass pot Value
		 */
		public void setComponents(double C1, double C2, double C3, double Ri, double Ro, double R2, double T, double M, double B)
		{
			component[0] = C1;
			component[1] = C2;
			component[2] = C3;
			component[3] = Ri;
			component[4] = Ro;
			component[5] = R2;
			component[6] = T;
			component[7] = M;
			component[8] = B;
		}

		public void setComponents(double[] vector)
		{
			if(vector.Length == 9)
				Array.Copy(vector, component, 9);
			else
				Console.WriteLine("Wrong component vector length");
		}

		public override void Update()
		{
			double[] sb = new double[4];
			double[] sa = new double[4];

			// Capacitors
			double c1 = component[0];
			double c2 = component[1];
			double c3 = component[2];
			// Resistors
			double Ri = component[3]; // R-in
			double Ro = component[4]; // R-out
			double R2 = component[5]; // resistor in stack

			// Pot values, each side
			double T1 = component[6]*parameters[P_TREBLE];
			double T2 = component[6]*(1-parameters[P_TREBLE]);
			double B0 = component[8]*parameters[P_BASS];
			double M1 = component[7] * parameters[P_MID];
			
			// If fendermode = true then = 0
			double M2 = (FenderMode) ? 0 : component[7] * (1 - parameters[P_MID]);

			// numerator

			// s^0
			sb[0] = Ro*0;
			// s^1
			sb[1] = Ro*(B0*c1 + B0*c2 + c1*M1 + c2*M1 + c3*M1 + c1*M2 + c2*M2 + c1*T1);
			// s^2
			sb[2] = Ro*(B0*c1*c3*M1 + B0*c2*c3*M1 + c1*c3*M1*M2 + c2*c3*M1*M2 + B0*c1*c3*R2 + c1*c3*M1*R2 + c1*c2*M2*R2 + c1*c3*M2*R2 + c1*c3*M1*T1 + c1*c2*M2*T1 + c1*(c2 + c3)*R2*T1 + c1*c3*M1*T2 + c1*c2*M2*T2 + B0*c1*c2*(R2 + T1 + T2) + c1*c2*M1*(R2 + T1 + T2));
			// s^3
			sb[3] = Ro*(B0*c1*c2*c3*R2*T1 + c1*c2*c3*M2*R2*T1 + B0*c1*c2*c3*M1*(R2 + T1 + T2) + c1*c2*c3*M1*M2*(R2 + T1 + T2));

			// denominator

			// s^0
			sa[0] = B0 + M1 + M2 + Ro + T1;
			// s^1
			sa[1] = (B0*c3*M1 + c3*M1*M2 + B0*c3*Ri + c3*M1*Ri + c1*M2*Ri + c2*M2*Ri +	B0*c3*R2 + c3*M1*R2 + c2*M2*R2 + c3*M1*Ro + c1*M2*Ro + c2*M2*Ro + c1*Ri*Ro + B0*c1*(Ri + Ro) + c1*M1*(Ri + Ro) + c3*M1*T1 +	c2*M2*T1 + c1*Ri*T1 + c2*(Ri + R2)*(Ro + T1) + c3*(Ri + R2)*(M2 + Ro + T1) + B0*c2*(Ri + R2 + Ro + T1) + c2*M1*(Ri + R2 + Ro + T1) + B0*c1*T2 + c1*M1*T2 + c1*M2*T2 + c1*T1*T2 + c1*Ro*(T1 + T2));
			// s^2
			sa[2] = (B0*c1*c3*M1*Ri + c1*c3*M1*M2*Ri + B0*c1*c3*Ri*R2 + c1*c2*M2*Ri*R2 + B0*c1*c3*M1*Ro + c1*c3*M1*M2*Ro + B0*c1*c3*Ri*Ro + B0*c1*c3*R2*Ro + c1*c2*M2*R2*Ro + c1*c2*M2*Ri*T1 + B0*c1*c2*(Ri + Ro)*(R2 + T1) + c1*c2*M1*(Ri + Ro)*(R2 + T1) + c2*c3*M2*Ri*(Ro + T1) + c2*c3*M2*R2*(Ro + T1) + B0*c2*c3*(Ri + R2)*(Ro + T1) + B0*c2*c3*M1*(Ri + R2 + Ro + T1) + c2*c3*M1*M2*(Ri + R2 + Ro + T1) + B0*c1*c3*M1*T2 + c1*c3*M1*M2*T2 + B0*c1*c3*Ri*T2 + c1*c2*M2*Ri*T2 + B0*c1*c3*R2*T2 + c1*c2*M2*R2*T2 + c1*c2*M2*T1*T2 + B0*c1*c2*(Ri + R2 + Ro + T1)*T2 + c1*c2*M1*(Ri + R2 + Ro + T1)*T2 + c1*(c2 + c3)*Ri*T1*(R2 + T2) + c1*c3*M2*R2*(Ro + T2) + c1*c3*M2*Ri*(R2 + Ro + T2) + c1*c2*M2*Ro*(T1 + T2) + c1*(c2 + c3)*Ri*Ro*(R2 + T1 + T2) + c1*c3*M1*((Ri + Ro)*(R2 + T1) + (Ri + R2 + Ro + T1)*T2) + c1*(c2 + c3)*R2*(T1*T2 + Ro*(T1 + T2)));
			// s^3
			sa[3] = (B0*c1*c2*c3*Ri*Ro*T1 + c1*c2*c3*M2*Ri*Ro*T1 + B0*c1*c2*c3*R2*Ro*T1 + c1*c2*c3*M2*R2*Ro*T1 + B0*c1*c2*c3*M1*(Ri + Ro)*(R2 + T1) + c1*c2*c3*M1*M2*(Ri + Ro)*(R2 + T1) + B0*c1*c2*c3*Ri*R2*(Ro + T1) + c1*c2*c3*M2*Ri*R2*(Ro + T1) + B0*c1*c2*c3*Ri*(Ro + T1)*T2 + c1*c2*c3*M2*Ri*(Ro + T1)*T2 + B0*c1*c2*c3*R2*(Ro + T1)*T2 + c1*c2*c3*M2*R2*(Ro + T1)*T2 + B0*c1*c2*c3*M1*(Ri + R2 + Ro + T1)*T2 + c1*c2*c3*M1*M2*(Ri + R2 + Ro + T1)*T2);

			//System.out.println("b: "+sb[0]+" + "+sb[1]+"s + "+sb[2]+"s^2 + "+sb[3]+"s^3");
			//System.out.println("a: "+sa[0]+" + "+sa[1]+"s + "+sa[2]+"s^2 + "+sa[3]+"s^3");

			double[] zb = new double[4];
			double[] za = new double[4];
			Bilinear.transform(sb, sa, out zb, out za, fs);
			this.B = zb;
			this.A = za;
		}
	}
}

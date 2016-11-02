using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Modules
{
	public class TransferVariable : Transfer
	{
		public readonly double[] Parameters;

		public double Fs;

		public TransferVariable(double fs, int numberOfParameters)
		{
			this.Fs = fs;
			Parameters = new double[numberOfParameters];
		}

		public virtual void SetParam(int paramNumber, double param)
		{
			if (paramNumber < Parameters.Length && paramNumber >= 0)
				Parameters[paramNumber] = param;

			Update();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Modules
{
	public class TransferVariable : Transfer
	{
		public double[] parameters;
		public double fs;

		public TransferVariable(double fs, int numberOfParameters)
		{
			this.fs = fs;
			parameters = new double[numberOfParameters];
		}

		public virtual void SetParam(int paramNumber, double param)
		{
			if (paramNumber < parameters.Length && paramNumber >= 0)
				parameters[paramNumber] = param;

			Update();
		}
	}
}

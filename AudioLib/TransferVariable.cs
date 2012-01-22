using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public class TransferVariable : Transfer
	{
		public float[] parameters;
		public float fs;

		public TransferVariable(float fs)
		{
			this.fs = fs;
		}

		public virtual void SetParam(int paramNumber, float param)
		{
			if ( paramNumber < parameters.Length && paramNumber >= 0 )
				parameters[paramNumber] = param;

			Update();
		}
	}
}

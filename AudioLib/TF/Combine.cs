using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.TF
{
	public class Combine : Transfer
	{
		public Combine()
		{
			TransferFunctions = new List<Transfer>();
		}

		public List<Transfer> TransferFunctions;

		public override void Update()
		{
			float[] newL = { 1 };
			float[] b = newL;
			float[] a = newL;

			for (int i = 0; i < TransferFunctions.Count(); i++)
			{
				b = Conv.conv(b, TransferFunctions[i].B);
				a = Conv.conv(a, TransferFunctions[i].A);
			}

			this.B = b;
			this.A = a;
		}
	}
}

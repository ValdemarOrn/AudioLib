using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AudioLib
{
	public class ShaRandom
	{
		private SHA256Managed sha;
		public ShaRandom()
		{
			sha = new SHA256Managed();
		}

		public IList<double> Generate(long seed, int count)
		{
			var byteList = new List<byte>();
			var iterations = count * sizeof(uint) / (256 / 8) + 1;
			var bytes = BitConverter.GetBytes(seed);

			for (int i = 0; i < iterations; i++)
			{
				bytes = sha.ComputeHash(bytes);
				byteList.AddRange(bytes);
			}

			var byteArray = byteList.ToArray();
			var output = new List<double>();
			for (int i = 0; i < count; i++)
			{
				var val = BitConverter.ToUInt32(byteArray, i * 4);
				var doubleVal = val / (double)uint.MaxValue;
				output.Add(doubleVal);
			}

			return output;
		}
	}
}

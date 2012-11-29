using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	class SampleFormats
	{
		public static byte Get8Bit(double val)
		{
			val = val * 128;
			if (val < -128)
				val = -128;
			if (val > 127)
				val = 127;

			int data = (int)val;
			data = data + 0x80;

			byte output = (byte)data;
			return output;
		}

		public static short Get16Bit(double val)
		{
			val = val * 32768;
			if (val < -32768)
				val = -32768;
			if (val > 32767)
				val = 32767;

			int data = (int)val;
			short output = (short)data;
			return output;
		}

		public static int Get24Bit(double val)
		{
			val = val * 8388608;
			if (val < -8388608)
				val = -8388608;
			if (val > 8388607)
				val = 8388607;

			int data = (int)val;
			return data;
		}

		public static int Get32Bit(double val)
		{
			val = val * 2147483648;
			if (val < -2147483648)
				val = -2147483648;
			if (val > 2147483647)
				val = 2147483647;

			int data = (int)val;
			return data;
		}
	}
}

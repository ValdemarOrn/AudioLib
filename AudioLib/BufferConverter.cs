using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioLib
{
    public static class BufferConverter
    {
		public static sbyte[] ToSbyte(byte[] data)
		{
			var output = new List<sbyte>();

			for (int i = 0; i < data.Length / 4; i++)
			{
				var sample = (sbyte)(-128 + data[i]);
				output.Add(sample);
			}

			return output.ToArray();
		}

		public static float[] ToFloat(byte[] data)
        {
            var output = new List<float>();

            for (int i = 0; i < data.Length / 4; i++)
            {
                var sample = BitConverter.ToSingle(data, i * 4);
                output.Add(sample);
            }

            return output.ToArray();
        }

        public static int[] ToInts(byte[] data)
        {
            var output = new List<int>();

            for (int i = 0; i < data.Length / 4; i++)
            {
                var sample = BitConverter.ToInt32(data, i * 4);
                output.Add(sample);
            }

            return output.ToArray();
        }

        public static uint[] ToUints(byte[] data)
        {
            var output = new List<uint>();

            for (int i = 0; i < data.Length / 4; i++)
            {
                var sample = BitConverter.ToUInt32(data, i * 4);
                output.Add(sample);
            }

            return output.ToArray();
        }

        public static short[] ToShort(byte[] data)
        {
            var output = new List<short>();

            for (int i = 0; i < data.Length / 2; i++)
            {
                var sample = BitConverter.ToInt16(data, i * 2);
                output.Add(sample);
            }

            return output.ToArray();
        }
    }
}

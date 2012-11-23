using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public sealed class CircularBuffer
	{
		int BufferSize;
		double[] Buffer;
		int index = 0;

		public CircularBuffer(int bufferSize)
		{
			BufferSize = bufferSize;
			Buffer = new double[BufferSize];
		}

		public void Write(double value)
		{
			index = (index + 1) % BufferSize;
			Buffer[index] = value;
		}

		public double Read(int offset = 0)
		{
			// prevent/limit negative indexes by adding 1000*bufferSize before modulo
			int readIndex = (index + offset + 1000 * BufferSize) % BufferSize;

			double value = Buffer[readIndex];
			return value;
		}
	}
}

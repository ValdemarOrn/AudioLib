using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	/// <summary>
	/// Function to read and write WAVE file format
	/// </summary>
	public class WaveFiles
	{
		#region Read Operations

		public static FormatChunkData ReadWaveFormat(string filename)
		{
			var data = System.IO.File.ReadAllBytes(filename);
			var clm = new List<byte[]>();
			FormatChunkData format;
			ReadWaveFile(data, ref clm, out format);
			return format;
		}

		/// <summary>
		/// Read a WAVE file. Supports multiple channels, any bitrate.
		/// Supported formats are IEEE 32bit floating point and uncompressed PCM 8/16/24/32 bit
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static double[][] ReadWaveFile(string filename)
		{
			var data = System.IO.File.ReadAllBytes(filename);
			return ReadWaveFile(data);
		}

		public static double[][] ReadWaveFile(string filename, ref List<byte[]> clmData)
		{
			var data = System.IO.File.ReadAllBytes(filename);
			FormatChunkData format;
			return ReadWaveFile(data, ref clmData, out format);
		}

		public static double[][] ReadWaveFile(byte[] data)
		{
			var clm = new List<byte[]>();
			FormatChunkData format;
			return ReadWaveFile(data, ref clm, out format);
		}

		/// <summary>
		/// Read a WAVE file. Supports multiple channels, any bitrate.
		/// Supported formats are IEEE 32bit floating point and uncompressed PCM 8/16/24/32 bit
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="clmData">a list to be filled with metadata from the clm chunk</param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static double[][] ReadWaveFile(byte[] data, ref List<byte[]> clmData, out FormatChunkData format)
		{
			format = null;
			var waveFormat = new byte[] { data[8], data[9], data[10], data[11] };
			string fmt = Encoding.ASCII.GetString(waveFormat);
			if (fmt != "WAVE")
				return null;

			int idx = 12;
			double[][] output = null;

			while (idx < data.Length)
			{

				string chunkId = Encoding.ASCII.GetString(data, idx, 4);
				int chunkSize = BitConverter.ToInt32(data, idx + 4);

				if (chunkId == "fmt ")
				{
					format = ParseFmtChunk(data, idx);
				}
				else if (chunkId == "data")
				{
					if (format == null) // format must preced data.
						return null;

					output = ParseDataChunk(data, idx, chunkSize, format);
					break;
				}
				else
				{
					var dataChunk = data.Skip(idx).Take(8 + chunkSize).ToArray();
					clmData?.Add(dataChunk);
				}

				idx = idx + 8 + chunkSize;
			}

			return output;
		}

		/// <summary>
		/// Parses the data chunk, containing the audio data
		/// </summary>
		/// <param name="data">data array to work on</param>
		/// <param name="idx">starting index of the data chunk</param>
		/// <param name="chunkSize">size of the data chunk</param>
		/// <param name="format">the format description, read from the 'fmt ' chunk</param>
		/// <returns></returns>
		private static double[][] ParseDataChunk(byte[] data, int idx, int chunkSize, FormatChunkData format)
		{
			var channels = new List<List<double>>();

			for (int i = 0; i < format.NumChannels; i++)
				channels.Add(new List<double>());

			idx += 8;
			int channel = 0;

			for (int i = idx; i < chunkSize + idx; i += format.BytesPerSample)
			{
				int value = 0;
				if (format.AudioFormat == 3) // audioFormat 3 indicated IEEE 32bit floating point data
				{
					float val = BitConverter.ToSingle(data, i);
					channels[channel].Add(val);
				}
				else if (format.BytesPerSample == 1) // 8 bit PCM data
				{
					value = data[i] - 0x80;
					channels[channel].Add(value / 128.0);
				}
				else if (format.BytesPerSample == 2) // 16 bit PCM data
				{
					value = BitConverter.ToInt16(data, i);
					channels[channel].Add(value / 32768.0);
				}
				else if (format.BytesPerSample == 3) // 24 bit PCM data
				{
					value = BitConverter.ToInt32(new byte[] { 0, data[i], data[i + 1], data[i + 2] }, 0);
					channels[channel].Add(value / 2147483648.0);
				}
				else if (format.BytesPerSample == 4) // 32 bit PCM data
				{
					value = BitConverter.ToInt32(data, i);
					channels[channel].Add(value / 2147483648.0);
				}

				channel = (channel + 1) % format.NumChannels;
			}

			// convert lists to arrays
			double[][] output = new double[format.NumChannels][];
			for (int i = 0; i < output.Length; i++)
				output[i] = channels[i].ToArray();

			return output;
		}

		/// <summary>
		/// Reads the fmt chunk
		/// </summary>
		/// <param name="data"></param>
		/// <param name="idx"></param>
		/// <returns></returns>
		private static FormatChunkData ParseFmtChunk(byte[] data, int idx)
		{
			var id = Encoding.ASCII.GetString(data, idx, 4);
			var size = BitConverter.ToInt32(data, idx + 4);

			idx = idx + 8;
			var fmt = new FormatChunkData();
			fmt.AudioFormat = data[idx];
			fmt.NumChannels = data[idx + 2];
			fmt.SampleRate = BitConverter.ToInt32(data, idx + 4);
			fmt.ByteRate = BitConverter.ToInt32(data, idx + 8);
			fmt.BlockAlign = BitConverter.ToInt16(data, idx + 12);
			fmt.BitsPerSample = BitConverter.ToInt16(data, idx + 14);
			fmt.BytesPerSample = fmt.BitsPerSample / 8;

			return fmt;
		}

		public class FormatChunkData
		{
			public int AudioFormat;
			public int NumChannels;
			public int SampleRate;
			public int ByteRate;
			public int BlockAlign;
			public int BitsPerSample;
			public int BytesPerSample;
		}

		#endregion

		#region Write Operations

		/// <summary>
		/// Write arrays of double as WAVE files
		/// </summary>
		/// <param name="data">data to write. All channels must have the same length</param>
		/// <param name="waveFormat">desired format of output file</param>
		/// <param name="samplerate">desired output samplerate</param>
		/// <param name="filename">file to write data in</param>
		public static void WriteWaveFile(double[][] data, WaveFormat waveFormat, int samplerate, string filename)
		{
			var bytes = WriteWaveFile(data, waveFormat, samplerate);
			System.IO.File.WriteAllBytes(filename, bytes);
		}

		/// <summary>
		/// Write arrays of double as WAVE files
		/// </summary>
		/// <param name="data">data to write. All channels must have the same length</param>
		/// <param name="waveFormat">desired format of output file</param>
		/// <param name="samplerate">desired output samplerate</param>
		public static byte[] WriteWaveFile(double[][] data, WaveFormat waveFormat, int samplerate)
		{
			if (data == null || data.Any(x => x == null))
				throw new Exception("Data and its content cannot be null");

			int maxlen = data.Max(x => x.Length);

			if(data.Any(x => x.Length != maxlen))
				throw new Exception("All channels must have the same length");

			byte audioFormat = (waveFormat == WaveFormat.IEEEFloat32) ? (byte)3 : (byte)1;
			byte numChannels = (byte)data.Length;
			byte bitsPerSample = (byte)32;
			if (waveFormat == WaveFormat.PCM8Bit)
				bitsPerSample = (byte)8;
			else if (waveFormat == WaveFormat.PCM16Bit)
				bitsPerSample = (byte)16;
			else if (waveFormat == WaveFormat.PCM24Bit)
				bitsPerSample = (byte)24;
			byte blockAlign = (byte)(bitsPerSample / 8 * numChannels);
			int byteRate = samplerate * numChannels * bitsPerSample / 8;

			// Create the fmt chunk
			byte[] fmtChunk = new byte[24];
			fmtChunk[0] = (byte)'f';
			fmtChunk[1] = (byte)'m';
			fmtChunk[2] = (byte)'t';
			fmtChunk[3] = (byte)' ';
			fmtChunk[4] = 16; // chunk size
			fmtChunk[8] = audioFormat;
			fmtChunk[10] = numChannels;
			var samplerateBytes = BitConverter.GetBytes(samplerate);
			fmtChunk[12] = samplerateBytes[0];
			fmtChunk[13] = samplerateBytes[1];
			fmtChunk[14] = samplerateBytes[2];
			fmtChunk[15] = samplerateBytes[3];
			var byterateBytes = BitConverter.GetBytes(byteRate);
			fmtChunk[16] = byterateBytes[0];
			fmtChunk[17] = byterateBytes[1];
			fmtChunk[18] = byterateBytes[2];
			fmtChunk[19] = byterateBytes[3];
			fmtChunk[20] = blockAlign;
			fmtChunk[22] = bitsPerSample;

			// create data chunk
			byte[] dataChunk = new byte[8 + bitsPerSample / 8 * maxlen * numChannels];
			dataChunk[0] = (byte)'d';
			dataChunk[1] = (byte)'a';
			dataChunk[2] = (byte)'t';
			dataChunk[3] = (byte)'a';
			var dataChunkSizeBits = BitConverter.GetBytes(dataChunk.Length - 8);
			dataChunk[4] = dataChunkSizeBits[0];
			dataChunk[5] = dataChunkSizeBits[1];
			dataChunk[6] = dataChunkSizeBits[2];
			dataChunk[7] = dataChunkSizeBits[3];
			int index = 8;

			for (int i = 0; i < maxlen; i++)
			{
				for (int ch = 0; ch < numChannels; ch++)
				{
					byte[] value = null;

					if (waveFormat == WaveFormat.IEEEFloat32)
						value = BitConverter.GetBytes((float)data[ch][i]);
					else if (waveFormat == WaveFormat.PCM8Bit)
						value = new byte[1] { SampleFormats.Get8Bit(data[ch][i]) };
					else if (waveFormat == WaveFormat.PCM16Bit)
						value = BitConverter.GetBytes(SampleFormats.Get16Bit(data[ch][i]));
					else if (waveFormat == WaveFormat.PCM24Bit)
						value = BitConverter.GetBytes(SampleFormats.Get24Bit(data[ch][i]));
					else if (waveFormat == WaveFormat.PCM32Bit)
						value = BitConverter.GetBytes(SampleFormats.Get32Bit(data[ch][i]));

					dataChunk[index] = value[0];
					if (bitsPerSample > 8)
						dataChunk[index + 1] = value[1];
					if (bitsPerSample > 16)
						dataChunk[index + 2] = value[2];
					if (bitsPerSample > 24)
						dataChunk[index + 3] = value[3];

					index += bitsPerSample / 8;
				}
			}

			// combine chunk and form output
			byte[] output = new byte[12 + fmtChunk.Length + dataChunk.Length];
			output[0] = (byte)'R';
			output[1] = (byte)'I';
			output[2] = (byte)'F';
			output[3] = (byte)'F';
			var totalSizeBytes = BitConverter.GetBytes(4 + fmtChunk.Length + dataChunk.Length);
			output[4] = totalSizeBytes[0];
			output[5] = totalSizeBytes[1];
			output[6] = totalSizeBytes[2];
			output[7] = totalSizeBytes[3];
			output[8] = (byte)'W';
			output[9] = (byte)'A';
			output[10] = (byte)'V';
			output[11] = (byte)'E';

			Array.Copy(fmtChunk, 0, output, 12, fmtChunk.Length);
			Array.Copy(dataChunk, 0, output, 12 + fmtChunk.Length, dataChunk.Length);

			return output;
		}

		public enum WaveFormat
		{
			PCM8Bit,
			PCM16Bit,
			PCM24Bit,
			PCM32Bit,
			IEEEFloat32
		}

		#endregion
	}
}

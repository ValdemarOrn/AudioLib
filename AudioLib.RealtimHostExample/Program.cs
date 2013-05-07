using AudioLib.PortAudioInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.RealtimHostExample
{
	/// <summary>
	/// This is a simple example showing how to use RealtimeHost to create a realtime audio application.
	/// It outputs a sinewave to all the output channels. The user can enter a new frequency to change the
	/// frequency of the wave
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			// The host lives in a singleton withing. It can not be created directly 
			// and only one host can exists within an application context
			var host = RealtimeHost.Host;

			// host.Process is the callback method that processes audio data. 
			// Assign the static process method in this class to be the callback
			host.Process = process;

			// Use the graphical editor to create a new config
			var config = RealtimeHostConfig.CreateConfig();

			// assign the config to the host
			host.SetConfig(config);

			// Open the stream and start processing
			host.OpenStream();
			host.StartStream();

			Console.WriteLine("\n\n\n\n");

			// allow the user to enter a new frequency or stop processing audio
			while (true)
			{
				Console.WriteLine("Enter frequency. type exit to processing audio");
				string input = Console.ReadLine();
				if (input.ToLower() == "exit")
				{
					// break the loop and exit the application
					break;
				}
				else
				{
					try
					{
						freq = Convert.ToDouble(input);
					}
					catch (Exception)
					{
						Console.WriteLine("Unable to parse '" + input + "' as a number");
					}
				}
			}

			// close the stream and stop processing
			host.StopStream();
			host.CloseStream();
		}

		static double freq = 400;
		static long n = 0;

		static void process(float[][] inp, float[][] outp)
		{
			for (int i=0; i < outp[0].Length; i++)
			{
				// calculate the frequency of the wave, in radians per sample
				double fwave = freq / RealtimeHost.Host.Samplerate * 2.0 * Math.PI;

				float val = (float)Math.Sin(n * fwave) * 0.5f;

				for (int ch = 0; ch < outp.Length; ch++)
					outp[ch][i] = val;

				n++;
			}
		}
	}
}

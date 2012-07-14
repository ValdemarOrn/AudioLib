using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public class RealtimeHostConfig
	{
		public RealtimeHostConfig()
		{
			int defaultInput = PortAudio.Pa_GetDefaultInputDevice();
			var inputInfo = PortAudio.Pa_GetDeviceInfo(defaultInput);

			inputParameters = new PortAudio.PaStreamParameters();
			inputParameters.device = defaultInput;
			inputParameters.sampleFormat = PortAudio.PaSampleFormat.paFloat32 | PortAudio.PaSampleFormat.paNonInterleaved; /* 32 bit floating point output */
			inputParameters.suggestedLatency = inputInfo.defaultLowInputLatency;
			inputParameters.hostApiSpecificStreamInfo = (IntPtr)0;

			int defaultOutput = PortAudio.Pa_GetDefaultOutputDevice();
			var outputInfo = PortAudio.Pa_GetDeviceInfo(defaultInput);

			outputParameters = new PortAudio.PaStreamParameters();
			outputParameters.device = defaultOutput;
			outputParameters.sampleFormat = PortAudio.PaSampleFormat.paFloat32 | PortAudio.PaSampleFormat.paNonInterleaved; /* 32 bit floating point output */
			outputParameters.suggestedLatency = outputInfo.defaultLowOutputLatency;
			outputParameters.hostApiSpecificStreamInfo = (IntPtr)0;
		}

		//PortAudio.PaStreamCallbackDelegate _callback;
		//PortAudio.PaStreamCallbackDelegate Callback { set { _callback = value; } }

		internal PortAudio.PaStreamParameters inputParameters;
		internal PortAudio.PaStreamParameters outputParameters;

        internal IntPtr Stream;

        /// <summary>
        /// Gets or sets the samplerate that is used for the stream.
        /// </summary>
        public int Samplerate { get; set; }

		public int NumberOfInputs  { get { return inputParameters.channelCount;  } set { inputParameters.channelCount  = value; } }
		public int NumberOfOutputs { get { return outputParameters.channelCount; } set { outputParameters.channelCount = value; } }

		/// <summary>
		/// The PortAudio ID for the currently selected input device
		/// </summary>
		public int InputDeviceID  { get { return inputParameters.device;  } set { inputParameters.device  = value; } }

		/// <summary>
		/// The PortAudio ID for the currently selected output device
		/// </summary>
		public int OutputDeviceID { get { return outputParameters.device; } set { outputParameters.device = value; } }

		public string InputDeviceName  { get { return PortAudio.Pa_GetDeviceInfo(InputDeviceID).name;  } }

		public string OutputDeviceName { get { return PortAudio.Pa_GetDeviceInfo(OutputDeviceID).name; } }

		/// <summary>
		/// The input latency in milliseconds
		/// </summary>
		public double InputLatencyMs
		{
			get { return inputParameters.suggestedLatency; }
			set { inputParameters.suggestedLatency = value; }
		}

		/// <summary>
		/// The output latency in milliseconds
		/// </summary>
		public double OutputLatencyMs
		{
			get { return outputParameters.suggestedLatency; }
			set { outputParameters.suggestedLatency = value; }
		}

		/// <summary>
		/// The size of the buffer. This determines the latency
		/// </summary>
		public UInt32 BufferSize
		{
			get
			{
				var minimumLatency = (InputLatencyMs < OutputLatencyMs) ? InputLatencyMs : OutputLatencyMs;
				UInt32 bufferSize = (UInt32)(Samplerate * minimumLatency);
				return bufferSize;
			}
			set
			{
                if (Samplerate < 0)
                    throw new Exception("Samplerate has not been set");

				InputLatencyMs = value / Samplerate;
				OutputLatencyMs = value / Samplerate;
			}
		}

		public string APIName
		{
			get { int hostApiID = PortAudio.Pa_GetDeviceInfo(OutputDeviceID).hostApi; return PortAudio.Pa_GetHostApiInfo(hostApiID).name; }
		}

		public static RealtimeHostConfig CreateConfig(RealtimeHostConfig config = null)
		{
			var editor = new RealtimeHostConfigEditor();

			if (config != null)
			{
				editor.InputDeviceID = config.InputDeviceID;
				editor.OutputDeviceID = config.OutputDeviceID;
				editor.Samplerate = config.Samplerate;
				editor.Latency = (int)(config.InputLatencyMs * config.Samplerate);
				editor.InputChannels = config.NumberOfInputs;
				editor.OutputChannels = config.NumberOfOutputs;
			}

			editor.ShowDialog();

			if (editor.OK == false)
				return null;
			else
			{
				var conf = new RealtimeHostConfig();
				conf.InputDeviceID = editor.InputDeviceID;
				conf.OutputDeviceID = editor.OutputDeviceID;
				conf.NumberOfInputs = editor.InputChannels;
				conf.NumberOfOutputs = editor.OutputChannels;
				conf.Samplerate = editor.Samplerate;
				conf.InputLatencyMs = editor.Latency / (double)editor.Samplerate;
				conf.OutputLatencyMs = editor.Latency / (double)editor.Samplerate;
				return conf;
			}
		}
	}
}

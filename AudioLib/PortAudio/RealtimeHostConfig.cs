using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AudioLib.PortAudio;

namespace AudioLib.PortAudioInterop
{
	[Serializable]
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

				InputLatencyMs = value / (double)Samplerate;
				OutputLatencyMs = value / (double)Samplerate;
			}
		}

		public string APIName
		{
			get { int hostApiID = PortAudio.Pa_GetDeviceInfo(OutputDeviceID).hostApi; return PortAudio.Pa_GetHostApiInfo(hostApiID).name; }
		}

		public string Serialize()
		{
			var sb = new StringBuilder();
			sb.AppendLine("APIName=" + APIName);
			sb.AppendLine("InputDeviceName=" + InputDeviceName);
			sb.AppendLine("OutputDeviceName=" + OutputDeviceName);
			sb.AppendLine("InputDeviceID=" + InputDeviceID);
			sb.AppendLine("OutputDeviceID=" + OutputDeviceID);
			sb.AppendLine("NumberOfInputs=" + NumberOfInputs);
			sb.AppendLine("NumberOfOutputs=" + NumberOfOutputs);
			sb.AppendLine("Samplerate=" + Samplerate);
			sb.AppendLine("BufferSize=" + BufferSize);
			return sb.ToString();
		}

		public static RealtimeHostConfig Deserialize(string serializedString)
		{
			try
			{
				var dict = serializedString
					.Split('\n')
					.Where(x => x.Contains('='))
					.Select(x => x.Trim())
					.ToDictionary(x => x.Split('=')[0].Trim(), x => x.Split('=')[1].Trim());

				var apiName = dict["APIName"];
				var inDeviceId = Convert.ToInt32(dict["InputDeviceID"]);
				var outDeviceId = Convert.ToInt32(dict["OutputDeviceID"]);
				var inputDeviceName = dict["InputDeviceName"];
				var outputDeviceName = dict["OutputDeviceName"];
				var numInputs = Convert.ToInt32(dict["NumberOfInputs"]);
				var numOutputs = Convert.ToInt32(dict["NumberOfOutputs"]);

				// fuzzy matching of devices as the deviceIds can change
				// prefers a device that matches on apiName, deviceId and deviceName, but falls back to match only apiName and deviceName if not found

				var devices = GetSelectedDevices(apiName, inDeviceId, outDeviceId, inputDeviceName, outputDeviceName);
				var realInputDeviceId = devices.Item1;
				var realOutputDeviceId = devices.Item2;

				var inDevice = PortAudio.Pa_GetDeviceInfo(realInputDeviceId);
				var outDevice = PortAudio.Pa_GetDeviceInfo(realOutputDeviceId);

				if (inDevice.maxInputChannels < numInputs)
					numInputs = inDevice.maxInputChannels;

				if (outDevice.maxOutputChannels < numOutputs)
					numOutputs = outDevice.maxOutputChannels;

				var conf = new RealtimeHostConfig();
				conf.InputDeviceID = realInputDeviceId;
				conf.OutputDeviceID = realOutputDeviceId;
				conf.NumberOfInputs = numInputs;
				conf.NumberOfOutputs = numOutputs;
				conf.Samplerate = Convert.ToInt32(dict["Samplerate"]);
				conf.BufferSize = Convert.ToUInt32(dict["BufferSize"]);
				return conf;
			}
			catch
			{
				return null;
			}
		}

		private static Tuple<int, int> GetSelectedDevices(string apiName, int inDeviceId, int outDeviceId, string inputDeviceName, string outputDeviceName)
		{
			var devices = Enumerable.Range(0, PortAudio.Pa_GetDeviceCount())
				.ToDictionary(x => x, x => PortAudio.Pa_GetDeviceInfo(x))
				.Where(x => PortAudio.Pa_GetHostApiInfo(x.Value.hostApi).name == apiName)
				.ToDictionary(x => x.Key, x => x.Value);

			int realInputDeviceId;
			int realOutputDeviceId;

			var matchedInputArr = devices.Where(x => x.Key == inDeviceId && x.Value.name == inputDeviceName).ToArray();
			if (matchedInputArr.Length == 1)
				realInputDeviceId = matchedInputArr[0].Key;
			else
				realInputDeviceId = devices.Where(x => x.Value.name == inputDeviceName).First().Key;

			var matchedOutputArr = devices.Where(x => x.Key == outDeviceId && x.Value.name == outputDeviceName).ToArray();
			if (matchedOutputArr.Length == 1)
				realOutputDeviceId = matchedOutputArr[0].Key;
			else
				realOutputDeviceId = devices.Where(x => x.Value.name == outputDeviceName).First().Key;

			return Tuple.Create(realInputDeviceId, realOutputDeviceId);
		}


		public static RealtimeHostConfig CreateConfig(RealtimeHostConfig config = null)
		{
			if (!PortAudio.Pa_IsInitialized)
				PortAudio.Pa_Initialize();

			Application.EnableVisualStyles();
			var editor = new RealtimeHostConfigEditor();

			if (config != null)
			{
				var apiName = config.APIName;

				var devices = GetSelectedDevices(apiName, config.InputDeviceID, config.OutputDeviceID, config.InputDeviceName, config.OutputDeviceName);
				var realInputDeviceId = devices.Item1;
				var realOutputDeviceId = devices.Item2;

				var inDevice = PortAudio.Pa_GetDeviceInfo(realInputDeviceId);
				var outDevice = PortAudio.Pa_GetDeviceInfo(realOutputDeviceId);

				var numInputs = (config.NumberOfInputs < inDevice.maxInputChannels)
					? config.NumberOfInputs
					: inDevice.maxInputChannels;

				var numOutputs = (config.NumberOfOutputs < inDevice.maxOutputChannels)
					? config.NumberOfOutputs
					: outDevice.maxOutputChannels;

				editor.InputDeviceID = realInputDeviceId;
				editor.OutputDeviceID = realOutputDeviceId;
				editor.Samplerate = config.Samplerate;
				editor.Latency = (int)(config.InputLatencyMs * config.Samplerate);
				editor.InputChannels = numInputs;
				editor.OutputChannels = numOutputs;
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

				var supported = PortAudio.Pa_IsFormatSupported(ref conf.inputParameters, ref conf.outputParameters, conf.Samplerate);
				if (supported == PortAudio.PaError.paInvalidSampleRate)
					throw new InvalidFormatException("The samplerate you have selected is not supported by the device");
				if (supported == PortAudio.PaError.paInvalidChannelCount)
					throw new InvalidFormatException("The number of inputs or outputs you have selected is not supported by the device");
				if (supported != PortAudio.PaError.paNoError)
					throw new InvalidFormatException("The configuration you have selected is not supported by the device");

				return conf;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AudioLib.PortMidiInterop
{
	public class MidiIn
	{
		private const int BufferSize = 128;

		private readonly int deviceId;
		private readonly PortMidi.PmEvent[] buffer = new PortMidi.PmEvent[BufferSize];
		private readonly List<byte> sysexMessage;
		private PortMidi.PmTimeProcPtr timeCallback;

		private IntPtr stream;
		private volatile bool running;
		private Thread thread;
		private bool insideSysexMessage;
		
		public event Action<byte[], bool> DataReceived;

		public MidiIn(int deviceId)
		{
			sysexMessage = new List<byte>();
			this.deviceId = deviceId;
		}

		public bool IsStarted => running;

		public void Start()
		{
			if (running)
				throw new Exception("Input is already started");

			timeCallback = PortMidi.Pt_Time;
			var err = PortMidi.Pm_OpenInput(out stream, deviceId, (IntPtr)0, 128, timeCallback, (IntPtr)0);
			running = true;
			insideSysexMessage = false;
			this.thread = new Thread(Listen) { IsBackground = true };
			thread.Start();
		}

		public void Stop()
		{
			running = false;
			thread.Join();
		}

		private void Listen()
		{
			while (running)
			{
				var count = PortMidi.Pm_Read(stream, buffer, BufferSize);
				for (int i = 0; i < count; i++)
				{
					var d = (uint)buffer[i].message;
					var b0 = (byte)(d & 0xFF);
					var b1 = (byte)((d & 0xFF00) >> 8);
					var b2 = (byte)((d & 0xFF0000) >> 16);
					var b3 = (byte)((d & 0xFF000000) >> 24);

					if (b0 == 0xF0)
					{
						insideSysexMessage = true;
						sysexMessage.Clear();
					}

					if (insideSysexMessage)
					{
						ParseSysex(new[] { b0, b1, b2, b3 });
						if (!insideSysexMessage && DataReceived != null)
							DataReceived.Invoke(sysexMessage.ToArray(), true);
					}
					else
					{
						var output = new[] { b0, b1, b2, b3 };
						if (DataReceived != null)
							DataReceived.Invoke(output.ToArray(), false);
					}
				}

				Thread.Sleep(1);
			}
		}
		
		private void ParseSysex(byte[] bytes)
		{
			foreach (var b in bytes)
			{
				sysexMessage.Add(b);

				if (b == 0xF7)
				{
					insideSysexMessage = false;
					break;
				}
			}
		}
	}
}

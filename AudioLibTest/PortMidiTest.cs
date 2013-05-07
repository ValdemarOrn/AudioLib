using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AudioLib.PortMidiInterop;

namespace AudioLib.Test
{
	[TestFixture]
	class PortMidiTest
	{
		[Test]
		public void TestOutput()
		{
			int count = PortMidi.Pm_CountDevices();

			for (int i = 0; i < count; i++)
			{
				var info = PortMidi.Pm_GetDeviceInfo(i);
				Console.WriteLine("" + i + " - " + info.interf + " - " + info.name);
			}

			//int select = Convert.ToInt32(Console.ReadLine());
			int select = 10;
			var err = PortMidi.Pt_Start(1, null, (IntPtr)0);
			IntPtr stream;
			var err2 = PortMidi.Pm_OpenOutput(out stream, select, (IntPtr)0, 0, PortMidi.Pt_Time, (IntPtr)0, 1);

			int chordSize = 8;
			PortMidi.PmEvent[] buffer = new PortMidi.PmEvent[chordSize];
			buffer[0].timestamp = PortMidi.Pt_Time((IntPtr)0);
			buffer[0].message = PortMidi.Pm_Message(0xC0, 0, 0);

			err2 = PortMidi.Pm_Write(stream, buffer, 1);

			buffer[0].timestamp = PortMidi.Pt_Time((IntPtr)0);
			buffer[0].message = PortMidi.Pm_Message(0x90, 60, 100);
			err2 = PortMidi.Pm_Write(stream, buffer, 1);

			buffer[0].timestamp = PortMidi.Pt_Time((IntPtr)0);
			buffer[0].message = PortMidi.Pm_Message(0x90, 60, 0);
			err2 = PortMidi.Pm_Write(stream, buffer, 1);

			err2 = PortMidi.Pm_WriteShort(stream, PortMidi.Pt_Time((IntPtr)0), PortMidi.Pm_Message(0x90, 60, 100));

			err2 = PortMidi.Pm_WriteShort(stream, PortMidi.Pt_Time((IntPtr)0), PortMidi.Pm_Message(0x90, 60, 0));

			var timestamp = PortMidi.Pt_Time((IntPtr)0);
			for (int i = 0; i < chordSize; i++)
			{
				buffer[i].timestamp = timestamp + 1000 * i;
				buffer[i].message = PortMidi.Pm_Message(0x90, 65 + 2 * i, 100);
			}
			err2 = PortMidi.Pm_Write(stream, buffer, chordSize);



			byte[] sysex = new byte[] { 0xF0, 0x23, 0x42, 0x56, 0x11, 0xF7 };
			timestamp = PortMidi.Pt_Time((IntPtr)0);
			err2 = PortMidi.Pm_WriteSysEx(stream, timestamp, sysex);

			err2 = PortMidi.Pm_WriteShort(stream, PortMidi.Pt_Time((IntPtr)0), PortMidi.Pm_Message(0x90, 60, 100));

			err2 = PortMidi.Pm_WriteShort(stream, PortMidi.Pt_Time((IntPtr)0), PortMidi.Pm_Message(0x90, 60, 0));

			err2 = PortMidi.Pm_Close(stream);
			err2 = PortMidi.Pm_Terminate();
		}
	}
}

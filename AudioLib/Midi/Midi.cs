using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Midi
{
	public enum MessageType
	{
		NoteOff = 0x80,
		NoteOn = 0x90,
		Aftertouch = 0xA0,
		ControlChange = 0xB0,
		ProgramChange = 0xC0,
		ChannelPressure = 0xD0,
		PitchWheel = 0xE0,
		Sysex = 0xF0,

		MtcQuarterFrame = 0xF1,
		SongPosition = 0xF2,
		SongSelect = 0xF3,
		TuneRequest = 0xF6,
		MidiClock = 0xF8,
		Tick = 0xF9,
		Start = 0xFA,
		Stop = 0xFC,
		Continue = 0xFB,
		ActiveSense = 0xFE,
		Reset = 0xFF,
	}

	public struct MidiHelper
	{
		private readonly static string[] Notes = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

		public MessageType MsgType;
		public int? Channel;

		public byte? Data1;
		public byte? Data2;
		public byte[] Sysex;

		public MidiHelper(MessageType msgType, int? channel, int? data1, int? data2) 
			: this(Pack(msgType, channel, data1, data2))
		{

		}

		public MidiHelper(int data) 
			: this(Unpack(data))
		{
		}

		public MidiHelper(byte[] data)
		{
			if (data == null || data.Length < 2) 
				throw new ArgumentException("data is null or empty");
			if (data.Length > 4)
				throw new ArgumentException("data array should contain between 2 and 4 bytes");

			if (data[0] < 0xF0) // Voice command
			{
				int type = (data[0] & 0xf0);
				MsgType = (MessageType)type;

				Channel = data[0] & 0x0f;
				Data1 = data[1];

				if (MsgType == MessageType.ChannelPressure || MsgType == MessageType.ProgramChange)
				{
					if (data.Length == 2)
					{
						// ok
					}
					else if (data.Length == 3 && data[2] != 0)
						throw new ArgumentException("Midi Voice Message of type " + type + " should have 2 set data bytes");
					else if (data.Length == 4 && (data[2] != 0 || data[3] != 0))
						throw new ArgumentException("Midi Voice Message of type " + type + " should have 2 set data bytes");
				}
				
				if (data.Length >= 3)
					Data2 = data[2];
				else
					Data2 = null;

				Sysex = null;
			}
			else // System command
			{
				MsgType = (MessageType)data[0];
				Channel = null;

				Data1 = null;
				Data2 = null;

				if (data.Length >= 1)
					Data1 = data[1];
				if (data.Length >= 2)
					Data2 = data[2];

				if (MsgType == MessageType.Sysex)
					Sysex = data;
				else
					Sysex = null;
			}
		}

		// --------------------------- Getters/Setters ---------------------------

		public byte[] Data
		{
			get { return new byte[3] { (byte)((int)MsgType | (int)Channel), Data1.GetValueOrDefault(), Data2.GetValueOrDefault() }; }
		}

		public string NoteValue
		{
			get
			{
				var num = NoteNumber;
				if (!num.HasValue) return "";
				// midi note 60 == C4

				var noteIdx = (num.Value - 60 + 120) % 12;
				var octave = Math.Floor((num.Value - 60) / 12.0) + 4;
				var note = Notes[noteIdx];
				return note + octave;
			}
		}

		public byte? NoteNumber
		{
			get
			{
				if (MsgType == MessageType.NoteOn || MsgType == MessageType.NoteOff || MsgType == MessageType.Aftertouch)
					return Data1;

				return null; 
			}
			set
			{
				if (MsgType == MessageType.NoteOn || MsgType == MessageType.NoteOff || MsgType == MessageType.Aftertouch)
					Data1 = value.Value;
			}
		}

		public byte? ControlNumber
		{
			get
			{
				if (MsgType == MessageType.ControlChange)
					return Data1;

				return null; 
			}
			set
			{
				if (MsgType == MessageType.ControlChange)
					Data1 = value.Value;
			}
		}

		public byte? ControlValue
		{
			get
			{
				if (MsgType == MessageType.ControlChange)
					return Data2;

				return null;
			}
			set
			{
				if (MsgType == MessageType.ControlChange)
					Data2 = value.Value;
			}
		}

		public byte? NoteVelocity
		{
			get
			{
				if (MsgType == MessageType.NoteOn || MsgType == MessageType.NoteOff)
					return Data2; 

				return null;
			}
			set
			{
				if (MsgType == MessageType.NoteOn || MsgType == MessageType.NoteOff)
					Data2 = value.Value;
			}
		}

		public byte? Aftertouch
		{
			get
			{
				if (MsgType == MessageType.Aftertouch)
					return Data2; 

				return null;
			}
			set
			{
				if (MsgType == MessageType.Aftertouch)
					Data2 = value.Value;
			}
		}

		public byte? Program
		{
			get
			{
				if (MsgType == MessageType.ProgramChange)
					return Data1; 

				return null;
			}
			set
			{
				if (MsgType == MessageType.ProgramChange)
					Data1 = value.Value;
			}
		}

		public byte? Pressure
		{
			get
			{
				if (MsgType == MessageType.ChannelPressure)
					return Data1; 

				return null;
			}
			set
			{
				if (MsgType == MessageType.ChannelPressure)
					Data1 = value.Value;
			}
		}
		
		public short? Pitchwheel
		{ 
			get 
			{
				if (MsgType == MessageType.PitchWheel)
					return (short)(Data2.GetValueOrDefault() << 7 + Data1.GetValueOrDefault());

				return null;
			}
			set
			{
				if (MsgType == MessageType.PitchWheel)
				{
					Data2 = (byte)(value.Value >> 7);
					Data1 = (byte)(value.Value);
				}
			}
		}

		public override string ToString()
		{
			var d1 = !string.IsNullOrWhiteSpace(NoteValue) ? NoteValue : Data1.ToString();
			return string.Format("{0, 16} Ch {1,2} : {2,3} {3,3}", MsgType, Channel, d1, Data2);
		}

		public static int Pack(MessageType msgType, int? channel, int? data1, int? data2)
		{
			return (((int)msgType) | channel.GetValueOrDefault()) | (data1.GetValueOrDefault() << 8) | (data2.GetValueOrDefault() << 16);
		}

		public static int Pack(byte[] data)
		{
			var d = new byte[4];
			d[0] = data[0];
			d[1] = data[1];
			if (data.Length >= 3)
				d[2] = data[2];

			return BitConverter.ToInt32(d, 0);
		}

		public static byte[] Unpack(int data)
		{
			return BitConverter.GetBytes(data);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public class MidiMessageType
	{
		public const int NoteOff			= 0x80;
		public const int NoteOn				= 0x90;
		public const int Aftertouch			= 0xA0;
		public const int ControlChange		= 0xB0;
		public const int ProgramChange		= 0xC0;
		public const int ChannelPressure	= 0xD0;
		public const int PitchWheel			= 0xE0;

		public const int Sysex				= 0xF0;
		public const int MTCQuarterFrame	= 0xF1;
		public const int SongPosition		= 0xF2;
		public const int SongSelect			= 0xF3;
		public const int TuneRequest		= 0xF6;
		public const int MidiClock			= 0xF8;
		public const int Tick				= 0xF9;
		public const int Start				= 0xFA;
		public const int Stop				= 0xFC;
		public const int Continue			= 0xFB;
		public const int ActiveSense		= 0xFE;
		public const int Reset				= 0xFF;
	}

	public struct Midi
	{
		public int MessageType;
		public int? Channel;

		public byte? Data1;
		public byte? Data2;

		byte[] Data;

		public byte[] Sysex;

		public Midi(byte[] data)
		{
			Data = data;

			if (data == null || data.Length == 0) throw new ArgumentException("data is null or empty");

			if (data[0] < 0xF0) // Voice command
			{
				int type = (data[0] & 0xf0);
				MessageType = type;

				Channel = data[0] & 0x0f;
				Data1 = data[1];

				if (data.Length == 2 && (type != 0xD0 && type != 0xC0))
					throw new ArgumentException("Midi Voice Message of type " + type + " should have 2 data bytes");

				if (data.Length == 3)
					Data2 = data[2];
				else
					Data2 = null;

				Sysex = null;
			}
			else // System command
			{
				MessageType = data[0];
				Channel = null;

				Data1 = null;
				Data2 = null;

				if (data.Length >= 1)
					Data1 = data[1];
				if (data.Length >= 2)
					Data2 = data[2];

				if (MessageType == MidiMessageType.Sysex)
					Sysex = data;
				else
					Sysex = null;
			}
		}

		// --------------------------- Getters ---------------------------

		public byte? _NoteNumber
		{
			get
			{
				if (MessageType != MidiMessageType.NoteOn &&
					MessageType != MidiMessageType.NoteOff &&
					MessageType != MidiMessageType.Aftertouch)
					return null; 

				return Data1;
			}
		}

		public byte? _ControlNumber
		{
			get
			{
				if (MessageType != MidiMessageType.ControlChange)
					return null; 

				return Data1;
			}
		}

		public byte? _ControlValue
		{
			get
			{
				if (MessageType != MidiMessageType.ControlChange)
					return null;

				return Data2;
			}
		}

		public byte? _NoteVelocity
		{
			get
			{
				if (MessageType != MidiMessageType.NoteOn &&
					MessageType != MidiMessageType.NoteOff)
					return null; 

				return Data2;
			}
		}

		public byte? _Aftertouch
		{
			get
			{
				if (MessageType != MidiMessageType.Aftertouch)
					return null; 

				return Data2;
			}
		}

		public byte? _Program
		{
			get
			{
				if (MessageType != MidiMessageType.ProgramChange)
					return null; 

				return Data1;
			}
		}

		public byte? _Pressure
		{
			get
			{
				if (MessageType != MidiMessageType.ChannelPressure)
					return null; 

				return Data1;
			}
		}
		
		public short? _Pitchwheel
		{ 
			get 
			{
				if (MessageType != MidiMessageType.PitchWheel)
					return null;

				return (short)((int)Data2 << 7 + (int)Data1);
			} 
		}
		
	}
}

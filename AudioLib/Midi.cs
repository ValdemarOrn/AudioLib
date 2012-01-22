using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib
{
	public enum MidiMessageType
	{
		NoteOff			= 0x80,
		NoteOn			= 0x90,
		Aftertouch		= 0xA0,
		ControlChange	= 0xB0,
		ProgramChange	= 0xC0,
		ChannelPressure	= 0xD0,
		PitchWheel		= 0xE0,

		Sysex				= 0xF0,
		MTCQuarterFrame		= 0xF1,
		SongPosition		= 0xF2,
		SongSelect			= 0xF3,
		TuneRequest			= 0xF6,
		MidiClock			= 0xF8,
		Tick				= 0xF9,
		Start				= 0xFA,
		Stop				= 0xFC,
		Continue			= 0xFB,
		ActiveSense			= 0xFE,
		Reset				= 0xFF
	}

	public struct Midi
	{
		public MidiMessageType MessageType;
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
				MessageType = (MidiMessageType)type;

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
				MessageType = (MidiMessageType)data[0];
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

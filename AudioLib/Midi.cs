using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioLib.Midi
{
	public sealed class MessageType
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

	public struct MidiHelper
	{
		public int MsgType;
		public int? Channel;

		public byte? Data1;
		public byte? Data2;

		byte[] Data;

		public byte[] Sysex;

		public MidiHelper(byte[] data)
		{
			Data = data;

			if (data == null || data.Length == 0) throw new ArgumentException("data is null or empty");

			if (data[0] < 0xF0) // Voice command
			{
				int type = (data[0] & 0xf0);
				MsgType = type;

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
				MsgType = data[0];
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

		// --------------------------- Getters ---------------------------

		public byte? _NoteNumber
		{
			get
			{
				if (MsgType != MessageType.NoteOn &&
					MsgType != MessageType.NoteOff &&
					MsgType != MessageType.Aftertouch)
					return null; 

				return Data1;
			}
		}

		public byte? _ControlNumber
		{
			get
			{
				if (MsgType != MessageType.ControlChange)
					return null; 

				return Data1;
			}
		}

		public byte? _ControlValue
		{
			get
			{
				if (MsgType != MessageType.ControlChange)
					return null;

				return Data2;
			}
		}

		public byte? _NoteVelocity
		{
			get
			{
				if (MsgType != MessageType.NoteOn &&
					MsgType != MessageType.NoteOff)
					return null; 

				return Data2;
			}
		}

		public byte? _Aftertouch
		{
			get
			{
				if (MsgType != MessageType.Aftertouch)
					return null; 

				return Data2;
			}
		}

		public byte? _Program
		{
			get
			{
				if (MsgType != MessageType.ProgramChange)
					return null; 

				return Data1;
			}
		}

		public byte? _Pressure
		{
			get
			{
				if (MsgType != MessageType.ChannelPressure)
					return null; 

				return Data1;
			}
		}
		
		public short? _Pitchwheel
		{ 
			get 
			{
				if (MsgType != MessageType.PitchWheel)
					return null;

				return (short)((int)Data2 << 7 + (int)Data1);
			} 
		}
		
	}
}

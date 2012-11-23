using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AudioLib.PortMidiInterop
{
	public class PortMidi
	{
		// ------------------------ Additions ------------------------

		public static List<PmDeviceInfo> Devices
		{
			get
			{
				var devices = new List<PmDeviceInfo>();
				for (int i = 0; i < Pm_CountDevices(); i++)
				{
					devices.Add(Pm_GetDeviceInfo(i));
				}

				return devices;
			}
		}


		// ------------------------ Delegates ------------------------

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void PtCallback(int timestamp, IntPtr userData);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int PmTimeProcPtr (IntPtr time_info);

		// ------------------------ Enums and Defines------------------------

		public enum PtError
		{
		    ptNoError = 0,         /* success */
			ptHostError = -10000,  /* a system-specific error occurred */
			ptAlreadyStarted,      /* cannot start timer because it is already started */
			ptAlreadyStopped,      /* cannot stop timer because it is already stopped */
			ptInsufficientMemory   /* memory could not be allocated */
		}

		public enum PmError
		{
			pmNoError = 0,
			pmNoData = 0,			/* A "no error" return that also indicates no data avail. */
			pmGotData = 1,			/* A "no error" return that also indicates data available */
			pmHostError = -10000,
			pmInvalidDeviceId,		/* out of range or output device when input is requested or input device when output is requested or device is already opened */
			pmInsufficientMemory,
			pmBufferTooSmall,
			pmBufferOverflow,
			pmBadPtr,				/* PortMidiStream parameter is NULL or stream is not opened or stream is output when input is required or stream is input when output is required */
			pmBadData,				/* illegal midi data, e.g. missing EOX */
			pmInternalError,
			pmBufferMaxSize			/* buffer is already as large as it can be */
		}

		public struct PmFilter
		{
			/* Filter bit-mask definitions */
			/* filter active sensing messages (0xFE): */
			public const int PM_FILT_ACTIVE = (1 << 0x0E);
			/* filter system exclusive messages (0xF0): */
			public const int PM_FILT_SYSEX = (1 << 0x00);
			/* filter MIDI clock message (0xF8) */
			public const int PM_FILT_CLOCK = (1 << 0x08);
			/* filter play messages (start 0xFA, stop 0xFC, continue 0xFB) */
			public const int PM_FILT_PLAY = ((1 << 0x0A) | (1 << 0x0C) | (1 << 0x0B));
			/* filter tick messages (0xF9) */
			public const int PM_FILT_TICK = (1 << 0x09);
			/* filter undefined FD messages */
			public const int PM_FILT_FD = (1 << 0x0D);
			/* filter undefined real-time messages */
			public const int PM_FILT_UNDEFINED = PM_FILT_FD;
			/* filter reset messages (0xFF) */
			public const int PM_FILT_RESET = (1 << 0x0F);
			/* filter all real-time messages */
			public const int PM_FILT_REALTIME = (PM_FILT_ACTIVE | PM_FILT_SYSEX | PM_FILT_CLOCK | PM_FILT_PLAY | PM_FILT_UNDEFINED | PM_FILT_RESET | PM_FILT_TICK);
			/* filter note-on and note-off (0x90-0x9F and 0x80-0x8F */
			public const int PM_FILT_NOTE = ((1 << 0x19) | (1 << 0x18));
			/* filter channel aftertouch (most midi controllers use this) (0xD0-0xDF)*/
			public const int PM_FILT_CHANNEL_AFTERTOUCH = (1 << 0x1D);
			/* per-note aftertouch (0xA0-0xAF) */
			public const int PM_FILT_POLY_AFTERTOUCH = (1 << 0x1A);
			/* filter both channel and poly aftertouch */
			public const int PM_FILT_AFTERTOUCH = (PM_FILT_CHANNEL_AFTERTOUCH | PM_FILT_POLY_AFTERTOUCH);
			/* Program changes (0xC0-0xCF) */
			public const int PM_FILT_PROGRAM = (1 << 0x1C);
			/* Control Changes (CC's) (0xB0-0xBF)*/
			public const int PM_FILT_CONTROL = (1 << 0x1B);
			/* Pitch Bender (0xE0-0xEF*/
			public const int PM_FILT_PITCHBEND = (1 << 0x1E);
			/* MIDI Time Code (0xF1)*/
			public const int PM_FILT_MTC = (1 << 0x01);
			/* Song Position (0xF2) */
			public const int PM_FILT_SONG_POSITION = (1 << 0x02);
			/* Song Select (0xF3)*/
			public const int PM_FILT_SONG_SELECT = (1 << 0x03);
			/* Tuning request (0xF6)*/
			public const int PM_FILT_TUNE = (1 << 0x06);
			/* All System Common messages (mtc, song position, song select, tune request) */
			public const int PM_FILT_SYSTEMCOMMON = (PM_FILT_MTC | PM_FILT_SONG_POSITION | PM_FILT_SONG_SELECT | PM_FILT_TUNE);

		}

		// ------------------------ Structs ------------------------

		[StructLayout(LayoutKind.Sequential)]
		public struct PmDeviceInfo
		{
			public int structVersion;		/* this internal structure version */ 
			[MarshalAs(UnmanagedType.LPStr)]
			public string interf;			/* underlying MIDI API, e.g. MMSystem or DirectX */
			[MarshalAs(UnmanagedType.LPStr)]
			public string name;				/* device name, e.g. USB MidiSport 1x1 */
			public int input;				/* true iff input is available */
			public int output;				/* true iff output is available */
			public int opened;				/* used by generic PortMidi code to do error checking on arguments */
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct PmEvent
		{
			public int message;
			public int timestamp;
		}

		// ------------------------ Helpers ------------------------

		public static int Pm_Message(int status, int data1, int data2)
		{
			int val = ((data2 << 16) & 0xFF0000) | ((data1 << 8) & 0xFF00) | (status & 0xFF);
			return val;
		}

		public static int Pm_MessageStatus(int msg)
		{
			return (msg) & 0xFF;
		}

		public static int Pm_MessageData1(int msg)
		{
			return (((msg) >> 8) & 0xFF);
		}

		public static int Pm_MessageData2(int msg)
		{
			return (((msg) >> 16) & 0xFF);
		}

		public static int Pm_Channel(int channel)
		{
			return (1 << (channel));
		}

		// ------------------------ Function Calls ------------------------

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PmError Pm_Abort(IntPtr stream);

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PmError Pm_Close(IntPtr stream);

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Pm_CountDevices();

		// Pm_Dequeue

		// Pm_Enqueue

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Pm_GetDefaultInputDeviceID();

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Pm_GetDefaultOutputDeviceID();

		
		[DllImport("PortMidi.dll", EntryPoint = "Pm_GetDeviceInfo", CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr IntPtr_Pm_GetDeviceInfo(int id);

		public static PmDeviceInfo Pm_GetDeviceInfo(int id)
		{
			IntPtr structptr = IntPtr_Pm_GetDeviceInfo(id);
			return (PmDeviceInfo)Marshal.PtrToStructure(structptr, typeof(PmDeviceInfo));
		}

		[DllImport("PortMidi.dll", EntryPoint = "Pm_GetErrorText", CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr IntPtr_Pm_GetErrorText(PmError errnum);

		public static string Pm_GetErrorText(PmError errnum)
		{
			IntPtr strptr = IntPtr_Pm_GetErrorText(errnum);
			return Marshal.PtrToStringAnsi(strptr);
		}


		// WARNING - UNTESTED!
		[DllImport("PortMidi.dll", EntryPoint = "Pm_GetErrorText", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Pm_GetHostErrorText([MarshalAs(UnmanagedType.LPStr)] string msg, int len);

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Pm_HasHostError(IntPtr stream);

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PmError Pm_Initialize();

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PmError Pm_OpenInput(
			out IntPtr stream,
			int inputDevice,
			IntPtr inputDriverInfo,
			int bufferSize,
			PmTimeProcPtr time_proc,
			IntPtr time_info);

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PmError Pm_OpenOutput(
			out IntPtr stream,
			int outputDevice,
			IntPtr outputDriverInfo,
			int bufferSize,
			PmTimeProcPtr time_proc,
			IntPtr time_info,
			int latency);

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Pm_Poll(IntPtr stream);

		// Pm_QueueCreate

		// Pm_QueueDestroy

		// Pm_QueueEmpty

		// Pm_QueueFull

		// Pm_QueuePeek

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Pm_Read(IntPtr stream, [In, Out] PmEvent[] buffer, int length);

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PmError Pm_SetChannelMask(IntPtr stream, int mask);

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PmError Pm_SetFilter(IntPtr stream, int filter);

		// Pm_SetOverflow

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PmError Pm_Terminate();

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PmError Pm_Write(IntPtr stream, [In, Out] PmEvent[] buffer, int length);

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PmError Pm_WriteShort(IntPtr stream, int timestamp, int msg);

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PmError Pm_WriteSysEx(IntPtr stream, int timestamp, byte[] msg);

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Pt_Sleep(int duration);

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PtError Pt_Start(int resolution, PtCallback callback, IntPtr userData);

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Pt_Started();

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PtError Pt_Stop();

		[DllImport("PortMidi.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Pt_Time(IntPtr t);

	}
}

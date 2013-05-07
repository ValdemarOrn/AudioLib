using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AudioLib.Midi;

namespace AudioLib.Test
{
	[TestFixture]
	class MidiTest
	{
		[Test]
		public void TestNoteOff()
		{
			var m = new MidiHelper(new byte[3] { 0x8F, 56, 127 });
			Assert.AreEqual(15, m.Channel);
			Assert.AreEqual(56, m._NoteNumber);
			Assert.AreEqual(127, m._NoteVelocity);
		}

		[Test]
		public void TestNoteOn()
		{
			var m = new MidiHelper(new byte[3] { 0x95, 56, 0 });
			Assert.AreEqual(5, m.Channel);
			Assert.AreEqual(56, m._NoteNumber);
			Assert.AreEqual(0, m._NoteVelocity);
		}

		[Test]
		public void TestAftertouch()
		{
			var m = new MidiHelper(new byte[3] { 0xA6, 56, 23 });
			Assert.AreEqual(6, m.Channel);
			Assert.AreEqual(56, m._NoteNumber);
			Assert.AreEqual(23, m._Aftertouch);
		}

		[Test]
		public void TestController()
		{
			var m = new MidiHelper(new byte[3] { 0xB0, 42, 125 });
			Assert.AreEqual(0, m.Channel);
			Assert.AreEqual(42, m._ControlNumber);
			Assert.AreEqual(125, m._ControlValue);
		}

		[Test]
		public void TestProgramChange()
		{
			var m = new MidiHelper(new byte[2] { 0xC1, 23 });
			Assert.AreEqual(1, m.Channel);
			Assert.AreEqual(23, m._Program);
		}

		[Test]
		public void TestChannelPressure()
		{
			var m = new MidiHelper(new byte[2] { 0xD1, 99 });
			Assert.AreEqual(1, m.Channel);
			Assert.AreEqual(99, m._Pressure);
		}

		[Test]
		public void TestPitchWheel()
		{
			var m = new MidiHelper(new byte[3] { 0xEB, 126, 42 });
			Assert.AreEqual(0xB, m.Channel);
			Assert.AreEqual((42 << 7 + 126), m._Pitchwheel);
		}

		[Test]
		public void TestSysex()
		{
			var data = new byte[8] { 0xF0, 126, 42, 12, 123, 103, 87, 0xF7 };
			var m = new MidiHelper(data);
			Assert.IsNull(m.Channel);
			Assert.AreEqual(data, m.Sysex);
		}
	}
}

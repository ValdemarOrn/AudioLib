using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AudioLib.Modules;

namespace AudioLib.Test
{
	[TestFixture]
	class TestTF
	{
		[Test]
		public void TestLowpass()
		{
			var lp = new TF.Lowpass1(48000.0f);
			lp.SetParam(TF.Lowpass1.P_FREQ, 500);
			
			Assert.AreEqual(3142.7146 / 99142.71, lp.B[0], 0.00001f);
			Assert.AreEqual(3142.7146 / 99142.71, lp.B[1], 0.00001f);
			Assert.AreEqual(1.0, lp.A[0], 0.00001f);
			Assert.AreEqual(-92857.29 / 99142.71, lp.A[1], 0.00001f);
		}

		[Test]
		public void TestHipass()
		{
			var hp = new TF.Highpass1(48000.0f);
			hp.SetParam(TF.Highpass1.P_FREQ, 500);

			Assert.AreEqual(96000 / 99142.714599609375, hp.B[0], 0.00001f);
			Assert.AreEqual(-96000 / 99142.714599609375, hp.B[1], 0.00001f);
			Assert.AreEqual(1.0, hp.A[0], 0.00001f);
			Assert.AreEqual(-92857.285400390625 / 99142.714599609375, hp.A[1], 0.00001f);
		}

		[Test]
		public void TestCombine()
		{
			var t1 = new Transfer();
			t1.B = new double[2] { 1, 1 };
			t1.A = new double[3] { 1, 2, 1 };

			var c = new TF.Combine();
			c.TransferFunctions.Add(t1);
			c.TransferFunctions.Add(t1);
			c.Update();

			Assert.AreEqual(new double[3] { 1, 2, 1 }, c.B);
			Assert.AreEqual(new double[5] { 1, 4, 6, 4, 1 }, c.A);
		}
	}
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using System.Drawing;
using AudioLib.UI;

namespace AudioLib.Test
{
	public class Myform : Form
	{
		public static Bitmap Background;
		public static Bitmap Knobs;
		public static Bitmap Switch;
		public static Bitmap Light1;
		public static Bitmap Light2;

		static int KnobX1 = 124;
		static int KnobX2 = 568;
		static int KnobY1 = 34;
		static int KnobY2 = 142;
		static int KnobH = 108;
		static int KnobW = 109;

		static int BoostX = 473;
		static int BoostY = 165;

		static int Light1X = 365;
		static int Light1Y = 57;

		static int Light2X = 365;
		static int Light2Y = 175;

		static int Light3X = 475;
		static int Light3Y = 57;

		static int SwitchX = 368;
		static int SwitchY = 110;

		static int Positions = 1;
		
		static Myform()
		{
			string dir = @"C:\Src\_Tree\Audio\SharpSoundPlugins\RXG100\img\";

			Background = new Bitmap(dir + "RGbase.png");
			Knobs = new Bitmap(dir + "RGknobs.jpg");
			Switch = new Bitmap(dir + "RGswitch.png");
			Light1 = new Bitmap(dir + "RGlight1.png");
			Light2 = new Bitmap(dir + "RGlight2.png");

			Positions = Knobs.Height / Background.Height;
		}

		Knob GainA, GainB, VolumeA, VolumeB;
		Switch MainSwitch;
		BitmapIndicator LightA, LightB, LightC;

		public Myform()
		{
			ClientSize = new Size(1100, 264);

			GainA = new BitmapKnob(Knobs, KnobW, KnobH, Positions, KnobX1, KnobY1, 0, Background.Height);
			GainA.Left = KnobX1;
			GainA.Top = KnobY1;

			GainB = new BitmapKnob(Knobs, KnobW, KnobH, Positions, KnobX1, KnobY1 + KnobH, 0, Background.Height);
			GainB.Left = KnobX1;
			GainB.Top = KnobY1 + KnobH;

			VolumeA = new BitmapKnob(Knobs, KnobW, KnobH, Positions, KnobX1 + KnobW, KnobY1, 0, Background.Height);
			VolumeA.Left = KnobX1 + KnobW;
			VolumeA.Top = KnobY1;

			VolumeB = new BitmapKnob(Knobs, KnobW, KnobH, Positions, KnobX1 + KnobW, KnobY1 + KnobH, 0, Background.Height);
			VolumeB.Left = KnobX1 + KnobW;
			VolumeB.Top = KnobY1 + KnobH;


			MainSwitch = new BitmapSwitch(Switch, Switch.Width / 2, Switch.Height, false, false);
			MainSwitch.Left = SwitchX;
			MainSwitch.Top = SwitchY;

			LightA = new BitmapIndicator(Light1, Light1.Width / 2, Light1.Height, false, false);
			LightA.Left = Light1X;
			LightA.Top = Light1Y;

			LightB = new BitmapIndicator(Light2, Light2.Width / 2, Light2.Height, false, false);
			LightB.Left = Light2X;
			LightB.Top = Light2Y;

			this.Controls.Add(GainA);
			this.Controls.Add(GainB);
			this.Controls.Add(VolumeA);
			this.Controls.Add(VolumeB);

			this.Controls.Add(MainSwitch);

			this.Controls.Add(LightA);
			this.Controls.Add(LightB);

			MainSwitch.ValueChanged += MainSwitch_ValueChanged;
		}

		void MainSwitch_ValueChanged(object arg1, double arg2)
		{
			LightA.Value = MainSwitch.Value;
			LightB.Value = 1 - MainSwitch.Value;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;
			g.ScaleTransform(1, 1);
			g.DrawImage(Background, 0, 0, Background.Width, Background.Height);

		}
	}
}

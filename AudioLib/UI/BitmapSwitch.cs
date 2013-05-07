using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AudioLib.UI
{
	public class BitmapSwitch : Switch
	{
		Bitmap Resource;
		bool Vertical;
		int XOffset, YOffset;

		public BitmapSwitch(Bitmap resource, int width, int height, bool invert = false, bool imageStackIsVertical = false, int xOffset = 0, int yOffset = 0)
		{
			XOffset = xOffset;
			YOffset = yOffset;
			Resource = resource;
			this.Width = width;
			this.Height = height;
			this.Invert = invert;
			Vertical = imageStackIsVertical;
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			var g = e.Graphics;

			bool on = Value > 0.5;
			if (Invert)
				on = !on;

			if (Mode == SwitchMode.Toggle)
				on = MouseIsDown;

			float srcX = XOffset;
			float srcY = YOffset;

			if(!Vertical && on)
				srcX += Resource.Width / 2;

			if (Vertical && on)
				srcY += Resource.Height / 2;

			g.DrawImage(Resource, new Rectangle(0, 0, Width, Height), srcX, srcY, Width, Height, GraphicsUnit.Pixel);
		}
	}
}

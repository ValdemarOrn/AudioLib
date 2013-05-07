using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AudioLib.UI
{
	public class BitmapKnob : Knob
	{
		public int Positions, XOffset, YOffset, XSpan, YSpan;
		public Bitmap Resource; 

		public BitmapKnob(Bitmap resource, int width, int height, int positions, int xOffset, int yOffset, int xSpan, int ySpan)
		{
			Resource = resource;

			this.Height = height;
			this.Width = width;

			Positions = positions;
			XOffset = xOffset;
			YOffset = yOffset;
			XSpan = xSpan;
			YSpan = ySpan;

		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			var g = e.Graphics;
			var frame = Math.Floor((this.Value * (double)Positions) - 0.00001);
			if (frame < 0)
				frame = 0;

			var imgx = (float)(XOffset + frame * XSpan);
			var imgy = (float)(YOffset + frame * YSpan);

			g.DrawImage(Resource, new Rectangle(0, 0, this.Width, this.Height), imgx, imgy, this.Width, this.Height, GraphicsUnit.Pixel); 
		}
	}
}

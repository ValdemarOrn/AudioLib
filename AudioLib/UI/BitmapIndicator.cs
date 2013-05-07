using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AudioLib.UI
{
	public class BitmapIndicator : Control
	{
		double _value;

		public double Value
		{
			get
			{ 
				return _value; 
			}
			set
			{
				_value = value;
				this.Invalidate();
			}
		}

		public bool Invert;
		public Bitmap Resource;
		bool Vertical;
		int XOffset, YOffset;

		public BitmapIndicator(Bitmap resource, int width, int height, bool invert = false, bool imageStackIsVertical = false, int xOffset = 0, int yOffset = 0)
		{
			Resource = resource;

			XOffset = xOffset;
			YOffset = yOffset;
			Width = width;
			Height = height;
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);
			Value = 0.0;
			Invert = invert;
			Vertical = imageStackIsVertical;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;

			bool on = Value > 0.5;
			if (Invert)
				on = !on;

			float srcX = XOffset;
			float srcY = YOffset;

			if (!Vertical && on)
				srcX += Resource.Width / 2;

			if (Vertical && on)
				srcY += Resource.Height / 2;

			g.DrawImage(Resource, new Rectangle(0, 0, Width, Height), srcX, srcY, Width, Height, GraphicsUnit.Pixel);
		}
	}
}

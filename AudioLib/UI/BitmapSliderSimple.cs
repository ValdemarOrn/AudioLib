using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AudioLib.UI
{
	public class BitmapSliderSimple : Control
	{
		double _value;

		public double Value
		{
			get { return _value; }
			set
			{
				if (_value == value)
					return;

				_value = value;
				this.Invalidate();
			}
		}

		public double Speed { get; set; }
		public event Action<object, double> ValueChanged;

		Bitmap Resource;

		public BitmapSliderSimple(Bitmap resource)
		{
			Resource = resource;
			Speed = 0.005;
			this.Width = Resource.Width;
			this.Height = Resource.Height / 2;
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.Value = 0.0;
			this.Cursor = Cursors.Hand;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;

			int drawLength = (int)(this.Width * Value);

			// draw base
			g.DrawImage(Resource, new Rectangle(0, 0, Width, Height), 0, 0, Width, Height, GraphicsUnit.Pixel);

			// draw active part
			g.DrawImage(Resource, new Rectangle(0, 0, drawLength, Height), 0, Height, drawLength, Height, GraphicsUnit.Pixel);
		}

		bool editing;
		Point lastPos;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				editing = true;
				lastPos = e.Location;
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
				editing = false;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (editing)
			{
				double dx = e.Location.X - lastPos.X;

				if (dx == 0)
					return;

				if (Control.ModifierKeys == Keys.Shift)
					dx *= 0.1;

				Value += dx * Speed;

				if (Value < 0.0)
					Value = 0.0;
				if (Value > 1.0)
					Value = 1.0;

				lastPos = e.Location;
				this.Invalidate();

				if (ValueChanged != null)
					ValueChanged(this, Value);
			}
		}
	}
}

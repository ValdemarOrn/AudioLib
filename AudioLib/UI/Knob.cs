﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AudioLib.UI
{
	public class Knob : Control
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

		public Brush Brush;

		public event Action<object, double> ValueChanged;

		public Knob()
		{
			Speed = 0.005;
			this.Brush = Brushes.Black;
			this.Width = 52;
			this.Height = 50;
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.Value = 0.0;
			this.Cursor = Cursors.Hand;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;
			
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.CompositingQuality = CompositingQuality.HighQuality;

			Pen p = new Pen(Brush, 5);
			p.SetLineCap(LineCap.Flat, LineCap.Flat, DashCap.Flat);

			g.DrawArc(p, 4, 4, 42, 42, 45, -270);

			float vectorX = (float)Math.Cos(Rad(218) - Value * Rad(256));
			float vectorY = (float)-Math.Sin(Rad(218) - Value * Rad(256));
			g.DrawLine(p, 25 + 5*vectorX, 25 + 5*vectorY, 25f + 23f*vectorX, 25 + 23f*vectorY);
			
		}

		double Rad(double deg)
		{
			return deg / 180.0 * Math.PI;
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
				double dx = lastPos.Y - e.Location.Y;

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

				if(ValueChanged != null)
					ValueChanged(this, Value);
			}
		}
	}
}

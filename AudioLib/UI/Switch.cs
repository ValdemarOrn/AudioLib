using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AudioLib.UI
{
	public class Switch : Control
	{
		public enum SwitchMode
		{
			Click,
			Toggle,
			Momentary
		}

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

		public Brush Brush;
		public Brush OffBrush;
		public bool Invert;
		public SwitchMode Mode;

		protected bool MouseIsDown = false;

		public event Action<object, double> ValueChanged;

		public Switch()
		{
			this.Mode = SwitchMode.Click;
			this.Brush = Brushes.Black;
			this.OffBrush = Brushes.Black;
			this.Width = 28;
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

			Pen p = new Pen(Brush, 4);
			p.SetLineCap(LineCap.Flat, LineCap.Flat, DashCap.Flat);

			var gp = new GraphicsPath();
			gp.AddLine(4, 10, 4, 40);
			gp.AddArc(4, 5, 10, 10, 180, 90);
			gp.AddLine(9, 5, 12, 5);
			gp.AddArc(12, 5, 10, 10, 270, 90);
			gp.AddLine(22, 10, 22, 30);
			gp.AddArc(12, 34, 10, 10, 0, 90);
			gp.AddLine(16, 44, 12, 44);
			gp.AddArc(4, 34, 10, 10, 90, 90);

			g.DrawPath(p, gp);

			bool on = Value > 0.5;
			if (Invert)
				on = !on;

			if (Mode == SwitchMode.Toggle)
				on = on && MouseIsDown;

			if(on)
				g.FillRectangle(Brush, 9, 10, 8, 10);
			else
				g.FillRectangle(OffBrush, 9, 29, 8, 10);
			
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			MouseIsDown = true;
			base.OnMouseDown(e);
			if (e.Button != System.Windows.Forms.MouseButtons.Left)
				return;

			double newVal = Value;

			if (Mode == SwitchMode.Click || Mode == SwitchMode.Toggle)
			{
				if (newVal > 0.5)
					newVal = 0.0;
				else
					newVal = 1.0;
			}
			else if (Mode == SwitchMode.Momentary)
			{
				newVal = 1.0;
			}

			if (newVal == Value)
				return;

			Value = newVal;

			this.Invalidate();
			if(ValueChanged != null)
				ValueChanged(this, Value);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			MouseIsDown = false;
			base.OnMouseUp(e);
			if (e.Button != System.Windows.Forms.MouseButtons.Left)
				return;

			if (Mode == SwitchMode.Momentary)
			{
				Value = 0.0;
			}

			this.Invalidate();
		}
	}
}

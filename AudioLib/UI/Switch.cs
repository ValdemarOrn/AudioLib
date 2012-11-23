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
		double _value;

		public double Value
		{
			get { return _value; }
			set
			{
				_value = value;
				this.Invalidate();
			}
		}

		public Brush Brush;
		public Brush OffBrush;
		public bool Invert;

		public event ValueChangedEvent ValueChanged;

		public Switch()
		{
			this.Brush = Brushes.Black;
			this.OffBrush = Brushes.Black;
			this.Width = 28;
			this.Height = 50;
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.Value = 0.0;
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

			if(on)
				g.FillRectangle(Brush, 9, 10, 8, 10);
			else
				g.FillRectangle(OffBrush, 9, 29, 8, 10);
			
		}


		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if (Value > 0.5)
					Value = 0.0;
				else Value = 1.0;

				this.Invalidate();
				ValueChanged(this, Value);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AudioLib.UI
{
	public partial class VelocityMapControl : UserControl
	{
		Pen darkBlue;

		public VelocityMap Map;
		
		public double TriggerValue;
		public Timer TriggerTimer;

		public int InnerWidth
		{
			get { return Width - 2; }
		}

		public int InnerHeight
		{
			get { return Height - 2; }
		}

		public VelocityMapControl()
		{
			TriggerTimer = new Timer();
			TriggerTimer.Interval = 500;
			TriggerTimer.Tick += new EventHandler(StopTrigger);

			TriggerValue = -100;

			darkBlue = new Pen(Brushes.DarkBlue, 1.3f);

			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			InitializeComponent();
		}

		void StopTrigger(object sender, EventArgs e)
		{
			TriggerValue = -100;
			TriggerTimer.Stop();
			Invalidate();
		}

		public void SetTrigger(double value)
		{
			TriggerTimer.Start();
			TriggerValue = value;
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;

			g.Clear(Color.White);
			//g.DrawRectangle(Pens.Black, 0f, 0f, Width - 1.0f, Height - 1.0f);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			if (Map == null)
				return;

			for (int i = 0; i < Map.NumberOfPoints - 1; i++)
			{
				double zoneSize = 1.0 / (Map.NumberOfPoints-1);

				double x1 = zoneSize * i;
				double y1 = Map.Values[i];

				double x2 = zoneSize * (i+1);
				double y2 = Map.Values[i + 1];

				g.DrawLine(darkBlue, (float)(1 + x1 * InnerWidth), (float)(1 + (1 - y1) * InnerHeight), (float)(1 + x2 * InnerWidth), (float)(1 + (1 - y2) * InnerHeight));
			}

			for (int i = 0; i < Map.NumberOfPoints; i++)
			{
				double zoneSize = 1.0/(Map.NumberOfPoints-1);
				float x = (float)(1 + (zoneSize * i) * InnerWidth);

				float y = (float)(1 + (1 - Map.Map(zoneSize * i)) * InnerHeight);
				g.DrawLine(Pens.Black, x, Height, x, y);

				if( i == selectedPoint)
					g.FillEllipse(Brushes.Orange, x - 3, y - 3, 6, 6);
				else
					g.FillEllipse(Brushes.DarkBlue, x - 3, y - 3, 6, 6);
			}

			if (TriggerValue >= 0)
			{
				float x = (float)(1 + TriggerValue * InnerWidth);
				float y = (float)(1 + (1 - Map.Map(TriggerValue)) * InnerHeight);
				g.FillEllipse(Brushes.Red, x - 3, y - 3, 6, 6);
				g.DrawLine(Pens.Red, x, Height, x, y);
			}
			
		}

		int selectedPoint = -1;
		bool mouseDown = false;
		Point lastMousePoint;

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (Map == null)
				return;

			// If we are moving the point, chagne the values
			if (mouseDown)
			{
				double dy = lastMousePoint.Y - e.Location.Y;
				if (dy != 0)
				{
					Map.Values[selectedPoint] += dy / Height;

					if (Map.Values[selectedPoint] > 1.0)
						Map.Values[selectedPoint] = 1.0;

					if (Map.Values[selectedPoint] < 0.0)
						Map.Values[selectedPoint] = 0.0;

					Invalidate();
				}

				lastMousePoint = e.Location;
				return;
			}

			// mouse is not pressed, we are selecting a point

			int x = e.X;

			double zoneSize = 1.0 / (Map.NumberOfPoints - 1);
			
			double minProximity = Width * 100; // big number
			int closestPoint = -1;

			for (int i = 0; i < Map.NumberOfPoints; i++)
			{
				double pointX = zoneSize * i * Width;
				if (Math.Abs(pointX - x) < minProximity)
				{
					minProximity = Math.Abs(pointX - x);
					closestPoint = i;
				}
			}

			int lastPoint = selectedPoint;
			selectedPoint = closestPoint;

			if (lastPoint != selectedPoint)
				Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			this.selectedPoint = -1;
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (Map == null)
				return;

			mouseDown = true;
			lastMousePoint = e.Location;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (Map == null)
				return;

			mouseDown = false;
		}
	}
}

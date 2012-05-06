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
		System.Drawing.Font font;

		public VelocityMap Map;
		
		public double TriggerValue;
		public Timer TriggerTimer;

		double xmin { get { if (Map == null) return 0.0; return Map.XMin; } set { if (Map == null) return; Map.XMin = value; } }
		double xmax { get { if (Map == null) return 1.0; return Map.XMax; } set { if (Map == null) return; Map.XMax = value; } }
		double ymin { get { if (Map == null) return 0.0; return Map.YMin; } set { if (Map == null) return; Map.YMin = value; } }
		double ymax { get { if (Map == null) return 1.0; return Map.YMax; } set { if (Map == null) return; Map.YMax = value; } }

		double ZoomX { get { return (xmax - xmin); } }
		double ZoomY { get { return (ymax - ymin); } }

		public int InnerWidth
		{
			get { return Width - 2; }
		}

		public int InnerHeight
		{
			get { return Height - 21; }
		}

		public VelocityMapControl()
		{
			font = new Font(FontFamily.GenericSansSerif, 8.0f);

			TriggerTimer = new Timer();
			TriggerTimer.Interval = 500;
			TriggerTimer.Tick += new EventHandler(StopTrigger);

			TriggerValue = -100;

			darkBlue = new Pen(Brushes.DarkBlue, 1.3f);

			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			InitializeComponent();
		}

		// Removes the trigger point from the UI
		void StopTrigger(object sender, EventArgs e)
		{
			TriggerValue = -100;
			TriggerTimer.Stop();
			Invalidate();
		}

		// paints the trigger point on the UI. 
		// Starts a timer that fires in 500ms and clears the trigger off the screen ( StopTrigger() )
		public void SetTrigger(double value)
		{
			TriggerTimer.Start();
			TriggerValue = value;
			Invalidate();
		}

		// Take a signal value, x and y, and return the pixel coordinates on the screen
		Point GetScreenPos(double x1, double y1)
		{
			// map based on min and max values
			x1 = (x1 - xmin) / (xmax - xmin);
			y1 = (y1 - ymin) / (ymax - ymin);

			// map onto graphics, by pixel
			x1 = x1 * InnerWidth;
			y1 = InnerHeight - y1 * InnerHeight;

			return new Point(x1, y1);
		}

		Point FromScreenPos(double x, double y)
		{
			x = x / ((double)InnerWidth) * (xmax - xmin) + xmin;
			y = (InnerHeight - y) / ((double)InnerHeight) * (ymax - ymin) + ymin;

			return new Point(x, y);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;

			g.Clear(Color.White);
			//g.DrawRectangle(Pens.Black, 0f, 0f, Width - 1.0f, Height - 1.0f);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			if (Map == null)
				return;

			// paint x,y coordinates of selected point
			if (selectedPoint != -1)
			{
				g.DrawString(String.Format("X: {0:0.00}, Y: {1:0.00}", Map.GetX(selectedPoint), Map.GetY(selectedPoint)), font, Brushes.Black, 2, 2);
			}

			// paint the line leading into the first point
			double x = Map.GetX(0);
			double y = Map.GetY(0);
			x = GetScreenPos(x, y).X;
			y = GetScreenPos(x, y).Y;
			g.DrawLine(darkBlue, (float)-10, (float)y, (float)x, (float)y);

			// paint the line leading out of the last point
			x = Map.GetX(Map.Count-1);
			y = Map.GetY(Map.Count-1);
			x = GetScreenPos(x, y).X;
			y = GetScreenPos(x, y).Y;
			g.DrawLine(darkBlue, (float)x, (float)y, (float)Width+10, (float)y);


			// paint the output line
			for (int i = 0; i < Map.Count - 1; i++)
			{
				double zoneSize = Map.GetX(i + 1) - Map.GetX(i);

				// get base values
				double x1 = Map.GetX(i);
				double y1 = Map.GetY(i);

				double x2 = Map.GetX(i) + zoneSize;
				double y2 = Map.GetY(i + 1);

				x1 = GetScreenPos(x1, y1).X;
				y1 = GetScreenPos(x1, y1).Y;

				x2 = GetScreenPos(x2, y2).X;
				y2 = GetScreenPos(x2, y2).Y;

				// paint lines
				g.DrawLine(darkBlue, (float)x1, (float)y1, (float)x2, (float)y2);

				// paint dots
				if (i == selectedPoint)
					g.FillEllipse(Brushes.Orange, (float)(x1 - 3), (float)(y1 - 3), 6, 6);
				else
					g.FillEllipse(Brushes.DarkBlue, (float)(x1 - 3), (float)(y1 - 3), 6, 6);

				
			}

			// Paint the last point, we need to paint that extra because the loop doesn't cover the last point

			// get base values
			double xLast = Map.GetX(Map.Count-1);
			double yLast = Map.GetY(Map.Count - 1);
			xLast = GetScreenPos(xLast, yLast).X;
			yLast = GetScreenPos(xLast, yLast).Y;
			// paint dots
			if (Map.Count-1 == selectedPoint)
				g.FillEllipse(Brushes.Orange, (float)(xLast - 3), (float)(yLast - 3), 6, 6);
			else
				g.FillEllipse(Brushes.DarkBlue, (float)(xLast - 3), (float)(yLast - 3), 6, 6);
			


			// Paint trigger point
			if (TriggerValue >= 0)
			{
				// get base values
				double x1 = TriggerValue;
				double y1 = Map.Map(TriggerValue);

				// map based on min and max values
				x1 = (x1 - xmin) / (xmax - xmin);
				y1 = (y1 - ymin) / (ymax - ymin);

				// map onto graphics, by pixel
				x1 = x1 * InnerWidth;
				y1 = InnerHeight - y1 * InnerHeight;

				g.FillEllipse(Brushes.Red, (float)(x1 - 3), (float)(y1 - 3), 6, 6);
				g.DrawLine(Pens.Red, (float)x1, Height, (float)x1, (float)y1);
			}

			// draw limits
			g.FillRectangle(Brushes.White, -10, InnerHeight, InnerWidth + 10, 40);
			g.DrawLine(Pens.Black, -10, InnerHeight + 1, InnerWidth + 10, InnerHeight + 1);
			g.DrawString(String.Format("{0:0.00}", xmin), font, Brushes.Black, 0 / 4.0f * InnerWidth + 4, InnerHeight + 3);
			g.DrawString(String.Format("{0:0.00}", xmax), font, Brushes.Black, 1 / 4.0f * InnerWidth + 4, InnerHeight + 3);
			g.DrawString(String.Format("{0:0.00}", ymin), font, Brushes.Black, 2 / 4.0f * InnerWidth + 4, InnerHeight + 3);
			g.DrawString(String.Format("{0:0.00}", ymax), font, Brushes.Black, 3 / 4.0f * InnerWidth + 4, InnerHeight + 3);
			
		}

		EditMode editMode;
		int selectedPoint = -1;
		bool mouseDown = false;
		System.Drawing.Point lastMousePoint;

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (Map == null)
				return;

			double dx = lastMousePoint.X - e.Location.X;
			double dy = lastMousePoint.Y - e.Location.Y;

			if (mouseDown)
			{
				// If we are moving the point, change the values
				if (editMode == EditMode.point)
				{
					if (dx != 0)
					{
						double val = Map.GetX(selectedPoint) - dx / Width * ZoomX;
						Map.SetX(val, selectedPoint);
					}

					if (dy != 0)
					{
						double val = Map.GetY(selectedPoint) + dy / Height * ZoomY;
						Map.SetY(val, selectedPoint);
					}
				}

				// Edit boundaries

				if (editMode == EditMode.xmin)
					xmin = xmin + dy * 0.01;
				else if (editMode == EditMode.xmax)
					xmax = xmax + dy * 0.01;
				else if (editMode == EditMode.ymin)
					ymin = ymin + dy * 0.01;
				else if (editMode == EditMode.ymax)
					ymax = ymax + dy * 0.01;

				lastMousePoint = e.Location;
				Invalidate();
				return;
			}

			// ------------ mouse is not pressed ----------------

			// Editing the maximums
			if (e.Y > InnerHeight)
			{
				if(e.X < InnerWidth / 4)
					editMode = EditMode.xmin;
				else if ( e.X < InnerWidth / 2)
					editMode = EditMode.xmax;
				else if ( e.X < InnerWidth * (3.0/4.0))
					editMode = EditMode.ymin;
				else
					editMode = EditMode.ymax;

				if(selectedPoint != -1)
					Invalidate();

				selectedPoint = -1;
				return;
			}

			// Editing the points

			int x = e.X;

			double shortestDistance = 999999999999999;
			int closestPoint = -1;

			for (int i = 0; i < Map.Count; i++)
			{
				var p = GetScreenPos(Map.GetX(i), Map.GetY(i));
				double distance = Math.Sqrt( (e.X - p.X)*(e.X - p.X) + (e.Y - p.Y)*(e.Y - p.Y) );
				if (distance < shortestDistance && distance < 10)
				{
					shortestDistance = distance;
					closestPoint = i;
				}
			}

			int lastPoint = selectedPoint;
			selectedPoint = closestPoint;

			if (selectedPoint == -1)
				editMode = EditMode.none;
			else
				editMode = EditMode.point;

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

		protected override void OnMouseClick(MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				if (selectedPoint != -1)
					Map.Remove(selectedPoint);
				else
				{
					var p = FromScreenPos(e.X, e.Y);
					Map.Add(p.X, p.Y);
				}

				Invalidate();
			}
		}

		// --------------- internal structures ---------------

		enum EditMode
		{
			xmin,
			xmax,
			ymin,
			ymax,
			point,
			none
		}

		class Point
		{
			public double X, Y;

			public Point(double x, double y)
			{
				X = x;
				Y = y;
			}
		}

	}

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace AudioLib.Plot
{
	public class Line
	{
		public Pen LinePen;
		public bool DrawLine;

		public Brush DotBrush;
		public bool DrawDot;

		public double[] x;
		public double[] y;

		public Line()
		{
			LinePen = Pens.Black;
			DotBrush = Brushes.Black;
			DrawLine = true;
		}

		public Line(float[] y) : this(y.Select(x => (double)x).ToArray())
		{
		}

		public Line(float[] y, float xmin, float xmax) : this(y.Select(x => (double)x).ToArray(), xmin, xmax)
		{
		}

		public Line(float[] x, float[] y) : this(x.Select(k => (double)k).ToArray(), y.Select(h => (double)h).ToArray())
		{
		}


		public Line(double[] y) : this()
		{
			this.y = y;
			this.x = Linspace(0, y.Length-1, y.Length).Select(x => (double)x).ToArray();
		}

		public Line(double[] y, double xmin, double xmax) : this()
		{
			this.y = y;
			this.x = Linspace((float)xmin, (float)xmax, y.Length).Select(x => (double)x).ToArray();
		}

		public Line(double[] x, double[] y) : this()
		{
			this.y = y;
			this.x = x;
		}

		private static float[] Linspace(float min, float max, int num)
		{
			double space = (max - min) / (num - 1);
			float[] output = new float[num];
			output[0] = min;
			double runningVal = min;
			for (int i = 1; i < num; i++)
			{
				runningVal = runningVal + space;
				output[i] = (float)runningVal;
			}

			return output;
		}
	}

	public class Plot
	{
		public static void ShowPlot(Line line)
		{
			ShowPlot(new List<Line>() { line });
		}

		public static void ShowPlot(Line lineA, Line lineB)
		{
			ShowPlot(new List<Line>() { lineA, lineB });
		}

		public static void ShowPlot(Line lineA, Line lineB, Line lineC)
		{
			ShowPlot(new List<Line>() { lineA, lineB, lineC });
		}

		public static void ShowPlot(List<Line> lines)
		{
			Form f = new Form();
			f.FormBorderStyle = FormBorderStyle.Sizable;
			f.Width = 1000;
			f.Height = 700;

			PlotSurface p = new PlotSurface(lines);
			p.Width = f.ClientSize.Width - 10;
			p.Height = f.ClientSize.Height - 10;
			p.Top = 5;
			p.Left = 5;
			
			f.Controls.Add(p);

			// delegate resizes the panel p
			f.Resize += new EventHandler(
				delegate (object sender, EventArgs e)
				{
					Form s = (Form)sender;
					p.Width = s.ClientSize.Width - 10;
					p.Height = s.ClientSize.Height - 10;
					p.Refresh();
				}
			);

			f.ShowDialog();
		}

		
	}

	class PlotSurface : Panel
	{
		List<Line> Lines;
		double xMin, xMax, yMin, yMax;

		public PlotSurface(List<Line> lines)
		{
			this.Lines = lines;

			xMin = lines.Min(x => x.x.Min());
			xMax = lines.Min(x => x.x.Max());

			yMin = lines.Min(x => x.y.Min());
			yMax = lines.Min(x => x.y.Max());
			
			xMin = xMin - (xMax - xMin) * 0.1;
			xMax = xMax + (xMax - xMin) * 0.1;
			yMin = yMin - (yMax - yMin) * 0.1;
			yMax = yMax + (yMax - yMin) * 0.1;

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			var bitmap = new Bitmap(this.Width, this.Height);
			var g = Graphics.FromImage(bitmap);

			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

			g.FillRectangle(Brushes.White, -1, -1, this.Width + 2, this.Height + 2);
			g.DrawLine(Pens.LightGray, TransformX(0), TransformY(yMax), TransformX(0), TransformY(yMin));
			g.DrawLine(Pens.LightGray, TransformX(xMin), TransformY(0), TransformX(xMax), TransformY(0));

			foreach (var line in Lines)
			{

				if (line.DrawLine == true)
				{
					for (int i = 0; i < line.x.Length - 1; i++)
					{
						if ((line.x[i] < xMin && line.x[i + 1] < xMin) || (line.x[i] > xMax && line.x[i + 1] > xMax))
							continue;

						if ((line.y[i] < yMin && line.y[i + 1] < yMin) || (line.y[i] > yMax && line.y[i + 1] > yMax))
							continue;

						g.DrawLine(line.LinePen, TransformX(line.x[i]), TransformY(line.y[i]), TransformX(line.x[i + 1]), TransformY(line.y[i + 1]));
					}
				}

				if (line.DrawDot == true)
				{
					for (int i = 0; i < line.x.Length; i++)
					{
						if (line.x[i] < xMin || line.x[i] > xMax)
							continue;

						if (line.y[i] < yMin || line.y[i] > yMax )
							continue;

						g.FillEllipse(line.DotBrush, TransformX(line.x[i]) - 1.5f, TransformY(line.y[i]) - 1.5f, 3, 3);
					}
				}
			}

			e.Graphics.DrawImageUnscaled(bitmap, 0, 0);
		}

		bool panning;
		bool zooming;

		Point startPoint;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			// ignore if we are already doing something
			if (zooming || panning)
				return;

			if (e.Button == System.Windows.Forms.MouseButtons.Left)
				panning = true;

			if (e.Button == System.Windows.Forms.MouseButtons.Right)
				zooming = true;

			startPoint = e.Location;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
				panning = false;

			if (e.Button == System.Windows.Forms.MouseButtons.Right)
				zooming = false;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			int dx = e.Location.X - startPoint.X;
			int dy = e.Location.Y - startPoint.Y;

			startPoint = e.Location;

			if (zooming)
			{
				double x = TransformInvX(e.Location.X);
				double y = TransformInvY(e.Location.Y);

				if (dx != 0)
				{
					xMax = xMax + (xMax - x) / 100 * (-dx);
					xMin = xMin + (xMin - x) / 100 * (-dx);
				}

				if (dy != 0)
				{
					yMax = yMax + (yMax - y) / 100 * dy;
					yMin = yMin + (yMin - y) / 100 * dy;
				}

				this.Refresh();
			}

			if (panning)
			{
				/*if (Math.Abs(dx) > 20)
				{
					int k = 23;
				}

				if (Math.Abs(dy) > 20)
				{
					int k = 23;
				}*/

				double xScale = (xMax - xMin) / this.Width;
				xMin += -xScale * dx;
				xMax += -xScale * dx;

				double yScale = (yMax - yMin) / this.Height;
				yMin += yScale * dy;
				yMax += yScale * dy;

				this.Refresh();
			}


		}

		public int TransformX(double x)
		{
			int output = (int)((x - xMin) * this.Width / (xMax - xMin));
			return output;
		}

		public int TransformY(double y)
		{
			int output = this.Height - (int)((y - yMin) * this.Height / (yMax - yMin));
			return output;
		}

		public double TransformInvX(double x)
		{
			double output = (x / this.Width) * (xMax - xMin) + xMin;
			return output;
		}

		public double TransformInvY(double y)
		{
			double output = ((this.Height - y) / this.Height) * (yMax - yMin) + yMin;
			return output;
		}
	}

	
}

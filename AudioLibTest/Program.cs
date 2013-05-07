using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AudioLib.Plot;
using AudioLib;
using System.Windows.Forms;

namespace AudioLib.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			Application.Run(new Myform());

			/*TubeTest1();
			TubeTest2();
			TubeTest3();
			TubeTest4();*/
		}

		/// <summary>
		/// Check the plate current, Normal Koren formula
		/// </summary>
		private static void TubeTest1()
		{
			var inp = Utils.Linspace(0, 400, 401);
			var outputs = new List<Line>();

			var t = Valve.Models._12AX7;

			for (int x = 0; x < 10; x++)
			{
				var outp = Utils.Linspace(0, 0, 401);

				for (int i = 0; i < inp.Length; i++)
				{

					outp[i] = t.GetCurrent(inp[i], 3 - x/2.0);
				}

				var line = new Line(outp);
				if ((3 - x / 2.0) > 0)
					line.LinePen = System.Drawing.Pens.Red;

				outputs.Add(line);
			}

			Plot.Plot.ShowPlot(outputs);
		}

		/// <summary>
		/// Solve the current given Eg
		/// </summary>
		private static void TubeTest2()
		{
			var t = Valve.Models._12AX7;
			var tfline = Utils.Linspace(-8, 8, 1000);

			for (int i = 0; i < tfline.Length; i++)
				tfline[i] = t.SolveCurrent(tfline[i]);

			Plot.Plot.ShowPlot(new List<Line>() { new Line(tfline) });
		}

		/// <summary>
		/// Check the Artificial Saturation curve
		/// </summary>
		private static void TubeTest3()
		{
			var t = Valve.Models._12AX7;
			var outputs = new List<Line>();

			for (int z = 0; z < 4; z++)
			{
				t.Parameters.SaturationPoint = 1 + z;

				for (int x = 0; x < 10; x++)
				{
					var tfline = Utils.Linspace(-8, 8, 1000);
					t.Parameters.Knee = 1 + x;
					for (int i = 0; i < tfline.Length; i++)
						tfline[i] = t.SaturateEg(tfline[i]);

					var line = new Line(tfline);
					outputs.Add(line);
				}
			}

			Plot.Plot.ShowPlot(outputs);
		}

		/// <summary>
		/// Plate Current using the artificial saturation curve
		/// </summary>
		private static void TubeTest4()
		{
			var t = Valve.Models._12AX7;
			var outputs = new List<Line>();

			t.Parameters.SaturationPoint = 1;

			for (int x = 0; x < 10; x++)
			{
				var input = Utils.Linspace(-8, 8, 1000);
				var tfline = new double[input.Length];

				t.Parameters.Knee = 10 + x;
				for (int i = 0; i < input.Length; i++)
					tfline[i] = t.SolveCurrent(input[i]);

				var line = new Line(input, tfline);
				outputs.Add(line);
			}
			outputs.Add(new Line(new double[2] { 2, 2 }, new double[2] { 0, 0.01 }));
			Plot.Plot.ShowPlot(outputs);
		}

		private static void FeedbackTest1()
		{
			var lp = new AudioLib.TF.Lowpass1(10000);
			lp.SetParam(0, 1000);

			double omega = 0.008;
			double gain = 10;
			double bias = 0.00;
			double feedb = 0.1;
			Func<double, double> Amp = x => 0.8 * gain * Math.Tanh(gain * (x + bias)) + 0.2 * gain * x;


			var inp = Utils.Linspace(0, 1000, 1001);
			inp = inp.Select(x => Math.Sin(x * omega * 2 * Math.PI)).ToArray();

			var amped = inp.Select(x => Amp(x)).ToArray();

			var dxline = new double[inp.Length];
			var adjusted = new double[inp.Length];
			for (int i = 1; i < adjusted.Length; i++)
			{
				var dx = inp[i] - feedb * lp.Process(adjusted[i - 1]);
				//var dx = inp[i] - feedb * adjusted[i-1];
				dxline[i] = dx;
				adjusted[i] = Amp(dx);
				//adjusted[i] = lp.process(adjusted[i]);
			}

			var line = new Line(inp);

			var line2 = new Line(amped);
			line2.LinePen = System.Drawing.Pens.Red;
			line2.DotBrush = System.Drawing.Brushes.Red;

			var line3 = new Line(adjusted);
			line3.LinePen = System.Drawing.Pens.Blue;
			line3.DotBrush = System.Drawing.Brushes.Blue;

			var line4 = new Line(dxline);
			line4.LinePen = System.Drawing.Pens.Green;
			line4.DotBrush = System.Drawing.Brushes.Green;
			/*
			line.DrawDot = true;
			line.DrawLine = false;

			line2.DrawDot = true;
			line2.DrawLine = false;

			line3.DrawDot = true;
			line3.DrawLine = false;

			line4.DrawDot = true;
			line4.DrawLine = false;*/

			Plot.Plot.ShowPlot(new List<Line>() { line, line2, line3, line4 });
		}
	}
}

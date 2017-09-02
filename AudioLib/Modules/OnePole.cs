using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioLib.Modules
{
	public class ZeroLp1
	{
		
		private double z1_state;
		//float g;
		private double g2;
		
		public ZeroLp1()
		{
			z1_state = 0;
			g2 = 0;
		}

		public double Process(double x)
		{
			// perform one sample tick of the lowpass filter
			//float v = (x - z1_state) * g / (1 + g);
			double v = (x - z1_state) * g2;
			double y = v + z1_state;
			z1_state = y + v;
			return y;
		}

		public double ProcessNoUpdate(double x)
		{
			// perform one sample tick of the lowpass filter
			//float v = (x - z1_state) * g / (1 + g);
			double v = (x - z1_state) * g2;
			return v + z1_state;
		}

		// 0...1
		public void SetFc(double fcRel)
		{
			//this->g = fcRel * M_PI;
			double g = (double)(fcRel * Math.PI);
			g2 = g / (1 + g);
		}
	}

	public class ZeroHp1
	{
		private double z1_state;
		//float g;
		private double g2;
		
		public ZeroHp1()
		{
			z1_state = 0;
			g2 = 0;
		}

		public double Process(double x)
		{
			// perform one sample tick of the lowpass filter
			//float v = (x - z1_state) * g / (1 + g);
			double v = (x - z1_state) * g2;
			double y = v + z1_state;
			z1_state = y + v;
			return x - y;
		}

		// 0...1
		public void SetFc(double fcRel)
		{
			//this->g = fcRel * M_PI;
			double g = (double)(fcRel * Math.PI);
			g2 = g / (1 + g);
		}
	}
}

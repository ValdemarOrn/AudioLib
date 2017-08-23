using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioLib
{
    public class TfUtil
    {
        private int order;
        private double[] zb;
        private double[] za;

        private double x1, x2, x3, x4, x5, x6;
        private double y1, y2, y3, y4, y5, y6;

        public TfUtil(int order)
            : this (new double[order + 1], new double[order + 1])
        {
            
        }

        public TfUtil(double[] zb, double[] za)
        {
            order = zb.Length - 1;
            if (za.Length != zb.Length)
                throw new Exception("lengths of arrays must match");

            this.zb = zb;
            this.za = za;
        }

        public int Order => order;

        public void Update(double[] newZb, double[] newZa)
        {
            if (newZb.Length - 1 != Order || newZb.Length != newZa.Length)
                throw new Exception("Length must match order must match");

            zb = newZb;
            za = newZa;
        }

        public double Process1(double x0, bool updateInternalState = true)
        {
            double y0 = zb[0] * x0 + zb[1] * x1 
                - za[1] * y1;

            if (updateInternalState)
            {
                y1 = y0;

                x1 = x0;
            }

            return y0;
        }

        public double Process2(double x0, bool updateInternalState = true)
        {
            double y0 = zb[0] * x0 + zb[1] * x1 + zb[2] * x2 
                - za[1] * y1 - za[2] * y2;

            if (updateInternalState)
            {
                y2 = y1;
                y1 = y0;

                x2 = x1;
                x1 = x0;
            }

            return y0;
        }

        public double Process3(double x0, bool updateInternalState = true)
        {
            double y0 = zb[0] * x0 + zb[1] * x1 + zb[2] * x2 + zb[3] * x3 
                - za[1] * y1 - za[2] * y2 - za[3] * y3;

            if (updateInternalState)
            {
                y3 = y2;
                y2 = y1;
                y1 = y0;

                x3 = x2;
                x2 = x1;
                x1 = x0;
            }

            return y0;
        }

        public double Process4(double x0, bool updateInternalState = true)
        {
            double y0 = zb[0] * x0 + zb[1] * x1 + zb[2] * x2 + zb[3] * x3 + zb[4] * x4
                - za[1] * y1 - za[2] * y2 - za[3] * y3 - za[4] * y4;

            if (updateInternalState)
            {
                y4 = y3;
                y3 = y2;
                y2 = y1;
                y1 = y0;

                x4 = x3;
                x3 = x2;
                x2 = x1;
                x1 = x0;
            }

            return y0;
        }

        public double Process5(double x0, bool updateInternalState = true)
        {
            double y0 = zb[0] * x0 + zb[1] * x1 + zb[2] * x2 + zb[3] * x3 + zb[4] * x4 + zb[5] * x5
                - za[1] * y1 - za[2] * y2 - za[3] * y3 - za[4] * y4 - za[5] * y5;

            if (updateInternalState)
            {
                y5 = y4;
                y4 = y3;
                y3 = y2;
                y2 = y1;
                y1 = y0;

                x5 = x4;
                x4 = x3;
                x3 = x2;
                x2 = x1;
                x1 = x0;
            }

            return y0;
        }

        public double Process6(double x0, bool updateInternalState = true)
        {
            double y0 = zb[0] * x0 + zb[1] * x1 + zb[2] * x2 + zb[3] * x3 + zb[4] * x4 + zb[5] * x5 + zb[6] * x6
                - za[1] * y1 - za[2] * y2 - za[3] * y3 - za[4] * y4 - za[5] * y5 - za[6] * y6;

            if (updateInternalState)
            {
                y6 = y5;
                y5 = y4;
                y4 = y3;
                y3 = y2;
                y2 = y1;
                y1 = y0;

                x6 = x5;
                x5 = x4;
                x4 = x3;
                x3 = x2;
                x2 = x1;
                x1 = x0;
            }

            return y0;
        }
    }
}

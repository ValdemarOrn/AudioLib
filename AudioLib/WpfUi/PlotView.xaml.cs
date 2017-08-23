using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AudioLib.WpfUi
{
	/// <summary>
	/// Interaction logic for WaveView.xaml
	/// </summary>
	public partial class PlotView : UserControl
	{
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(IList<KeyValuePair<double, double>>), typeof(PlotView), new PropertyMetadata(null));

		public static readonly DependencyProperty PointsProperty =
			DependencyProperty.Register("Points", typeof(PointCollection), typeof(PlotView), new PropertyMetadata(null));

        public static readonly DependencyProperty ZeroPointsProperty =
            DependencyProperty.Register("ZeroPoints", typeof(PointCollection), typeof(PlotView), new PropertyMetadata(null));

        public PlotView()
		{
			InitializeComponent();

			var prop = DependencyPropertyDescriptor.FromProperty(DataProperty, this.GetType());
			prop.AddValueChanged(this, (s, e) => ConvertData());
			Canvas.SizeChanged += (s, e) => ConvertData();
		}

		public IList<KeyValuePair<double, double>> Data
		{
			get { return (IList<KeyValuePair<double, double>>)GetValue(DataProperty); }
			set 
			{ 
				SetValue(DataProperty, value);
				ConvertData();
			}
		}

        public double? MinX { get; set; }
        public double? MaxX { get; set; }
        public double? MinY { get; set; }
        public double? MaxY { get; set; }

        protected PointCollection Points
		{
			get { return (PointCollection)GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); }
		}

        protected PointCollection ZeroPoints
        {
            get { return (PointCollection)GetValue(ZeroPointsProperty); }
            set { SetValue(ZeroPointsProperty, value); }
        }

        private void ConvertData()
		{
			var margin = 1;
			var values = Data;
			if (values == null || values.Count() == 0)
			{
				Points = new PointCollection();
				return;
			}

			var width = this.Canvas.ActualWidth - 2 * margin;
			var height = this.Canvas.ActualHeight - 2 * margin;

            var maxX = MaxX ?? values.Max(x => x.Key);
            var minX = MinX ?? values.Min(x => x.Key);
            var maxY = MaxY ?? values.Max(x => x.Value);
            var minY = MinY ?? values.Min(x => x.Value);

            var dx = maxX - minX;
            var dy = maxY - minY;

            var converted = new List<Point>();
            foreach (var kvp in values)
            {
                var x = kvp.Key;
                var y = kvp.Value;
                var xx = (x - minX) / dx;
                var yy = (y - minY) / dy;
                var point = new Point(margin + xx * width, margin + height - yy * height);
                converted.Add(point);
            }
            
			Points = new PointCollection(converted);

            var zeroes = new List<Point>();
            zeroes.Add(new Point(0, margin + height - (0 - minY) / dy * height));
            zeroes.Add(new Point(width, margin + height - (0 - minY) / dy * height));
            ZeroPoints = new PointCollection(zeroes);
        }
	}
}

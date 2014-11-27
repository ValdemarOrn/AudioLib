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
	public partial class WaveformView : UserControl
	{
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(IEnumerable<double>), typeof(WaveformView), new PropertyMetadata(null));

		public static readonly DependencyProperty PointsProperty =
			DependencyProperty.Register("Points", typeof(PointCollection), typeof(WaveformView), new PropertyMetadata(null));

		public WaveformView()
		{
			InitializeComponent();

			var prop = DependencyPropertyDescriptor.FromProperty(DataProperty, this.GetType());
			prop.AddValueChanged(this, (s, e) => ConvertData());
			Canvas.SizeChanged += (s, e) => ConvertData();
		}

		public IEnumerable<double> Data
		{
			get { return (IEnumerable<double>)GetValue(DataProperty); }
			set 
			{ 
				SetValue(DataProperty, value);
				ConvertData();
			}
		}

		protected PointCollection Points
		{
			get { return (PointCollection)GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); }
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
			var min = values.Min();
			var max = values.Max();
			var abs = max - min;
			var count = (double)values.Count();
			var i = 0;
			var converted = values.Select(x => new Point(margin + i++ / count * width, margin + height - ((x - min) / abs * height))).ToList();
			var pc = new PointCollection(converted);
			Points =  pc;
		}
	}
}

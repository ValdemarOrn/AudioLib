using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace Audiolib.WpfUi
{
	/// <summary>
	/// Interaction logic for Spinner.xaml
	/// </summary>
	public partial class Spinner : UserControl
	{
		static internal DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double?), typeof(Spinner),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		static internal DependencyProperty ValueTextProperty = DependencyProperty.Register("ValueText", typeof(string), typeof(Spinner));

		static internal DependencyProperty FormattingProperty = DependencyProperty.Register("Formatting", typeof(Func<double?, string>), typeof(Spinner),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		static new internal DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Spinner),
				new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		
		public Spinner()
		{
			Delta = 0.005;
			Min = 0;
			Max = 1;
			InitializeComponent();

			// add change listeners to props
			DependencyPropertyDescriptor prop = DependencyPropertyDescriptor.FromProperty(ValueProperty, this.GetType());
			prop.AddValueChanged(this, (x, y) => SetValue(ValueTextProperty, FormattedValue));

			prop = DependencyPropertyDescriptor.FromProperty(FormattingProperty, this.GetType());
			prop.AddValueChanged(this, (x, y) => SetValue(ValueTextProperty, FormattedValue));
		}

		public double? Value
		{
			get { return (double?)base.GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public string ValueText
		{
			get { return (string)base.GetValue(ValueTextProperty); }
			set { SetValue(ValueTextProperty, value); }
		}

		private string FormattedValue
		{
			get 
			{
				if (Formatting == null)
					return string.Format(CultureInfo.InvariantCulture, "{0:0.000}", Value);
				else
					return Formatting(Value); 
			}
		}

		public Func<double?, string> Formatting
		{
			get { return (Func<double?, string>)base.GetValue(FormattingProperty); }
			set 
			{ 
				SetValue(FormattingProperty, value);
				ValueText = FormattedValue;
			}
		}

		public new Brush BorderBrush
		{
			get { return (Brush)base.GetValue(BorderBrushProperty); }
			set { SetValue(BorderBrushProperty, value); }
		}

		public double Delta { get; set; }
		public double Min { get; set; }
		public double Max { get; set; }
		public double Default { get; set; }

		bool Selected;
		Point MousePos;

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Mouse.LeftButton == MouseButtonState.Released)
				return;

			Selected = true;
			Mouse.Capture(this);
			MousePos = e.GetPosition(this);
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			Selected = false;
			Mouse.Capture(null);
			MousePos = e.GetPosition(this);
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (Mouse.LeftButton == MouseButtonState.Released)
			{
				Selected = false;
				Mouse.Capture(null);
				MousePos = e.GetPosition(this);
				return;
			}

			var oldPos = MousePos;
			MousePos = e.GetPosition(this);

			if (!Selected)
				return;

			var dx = oldPos.Y - MousePos.Y;

			if (Math.Abs(dx) < 0.5)
				return;

			if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
				dx *= 0.1;

			var oldVal = Value ?? 0.0;
			var val = oldVal + Delta * dx;

			if (val < Min)
				val = Min;
			else if (val > Max)
				val = Max;

			if (val != oldVal)
				Value = val;
		}

		private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Value = Default;
		}

	}
}

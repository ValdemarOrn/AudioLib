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
	public partial class Knob2 : UserControl, INotifyPropertyChanged
	{
		public static readonly DependencyProperty CaptionProperty =
			DependencyProperty.Register("Caption", typeof(string), typeof(Knob2), 
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(double), typeof(Knob2), 
			new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty StepsProperty =
			DependencyProperty.Register("Steps", typeof(int), typeof(Knob2), 
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty MinProperty =
			DependencyProperty.Register("Min", typeof(double), typeof(Knob2), 
			new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty MaxProperty =
			DependencyProperty.Register("Max", typeof(double), typeof(Knob2), 
			new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty ThicknessProperty =
			DependencyProperty.Register("Thickness", typeof(double), typeof(Knob2), 
			new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty FillColorProperty =
			DependencyProperty.Register("FillColor", typeof(Brush), typeof(Knob2), 
			new FrameworkPropertyMetadata(Brushes.CornflowerBlue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty InnerPaddingProperty =
			DependencyProperty.Register("InnerPadding", typeof(double), typeof(Knob2), 
			new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		private double internalValue;
		private bool disableInternalUpdate;
		private bool UpdatesEnabled;

		private static Func<double, string> DefaultFormatter = x => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", x);

		public Knob2()
		{
			UpdatesEnabled = false;
			ValueFormatter = DefaultFormatter;
			Delta = 0.005;
			InitializeComponent();

			var prop = DependencyPropertyDescriptor.FromProperty(ValueProperty, this.GetType());
			prop.AddValueChanged(this, (s, e) =>
			{
				Update();

				if (!disableInternalUpdate)
					internalValue = Value;

				Announce(true);
			});

			prop = DependencyPropertyDescriptor.FromProperty(MinProperty, this.GetType());
			prop.AddValueChanged(this, (s, e) => Update());
			prop = DependencyPropertyDescriptor.FromProperty(MaxProperty, this.GetType());
			prop.AddValueChanged(this, (s, e) => Update());
			prop = DependencyPropertyDescriptor.FromProperty(InnerPaddingProperty, this.GetType());
			prop.AddValueChanged(this, (s, e) => Update());
			prop = DependencyPropertyDescriptor.FromProperty(ThicknessProperty, this.GetType());
			prop.AddValueChanged(this, (s, e) => Update());

			prop = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, this.GetType());
			prop.AddValueChanged(this, (s, e) => Update());
			prop = DependencyPropertyDescriptor.FromProperty(FillColorProperty, this.GetType());
			prop.AddValueChanged(this, (s, e) => Update());

			prop = DependencyPropertyDescriptor.FromProperty(WidthProperty, this.GetType());
			prop.AddValueChanged(this, (s, e) => Update());
			prop = DependencyPropertyDescriptor.FromProperty(HeightProperty, this.GetType());
			prop.AddValueChanged(this, (s, e) => Update());

			UpdatesEnabled = true;
			Update();
		}

		public string Caption
		{
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}

		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public int Steps
		{
			get { return (int)GetValue(StepsProperty); }
			set { SetValue(StepsProperty, value); }
		}

		public double Min
		{
			get { return (double)GetValue(MinProperty); }
			set { SetValue(MinProperty, value); }
		}

		public double Max
		{
			get { return (double)GetValue(MaxProperty); }
			set { SetValue(MaxProperty, value); }
		}

		public double Thickness
		{
			get { return (double)GetValue(ThicknessProperty); }
			set { SetValue(ThicknessProperty, value); }
		}

		public Brush FillColor
		{
			get { return (Brush)GetValue(FillColorProperty); }
			set { SetValue(FillColorProperty, value); }
		}

		public double InnerPadding
		{
			get { return (double)GetValue(InnerPaddingProperty); }
			set { SetValue(InnerPaddingProperty, value); }
		}


		private bool Selected { get; set; }
		private Point MousePos { get; set; }

		public Func<Knob2, string> GetAnnouncerCaption { get; set; }

		private Func<double, string> valueFormatter;
		public Func<double, string> ValueFormatter
		{
			get { return valueFormatter; }
			set { valueFormatter = value; NotifyChange(() => valueFormatter); }
		}

		private double? defaultValue;
		public double? DefaultValue
		{
			get { return defaultValue; }
			set { defaultValue = value; NotifyChange(() => DefaultValue); }
		}


		private double delta;
		public double Delta
		{
			get { return delta; }
			set { delta = value; NotifyChange(() => Delta); }
		}

		private bool central;
		public bool Central
		{
			get { return central; }
			set
			{
				central = value;
				NotifyChange(() => Central);
				Update();
			}
		}

		private Brush brush1;
		public Brush Brush1
		{
			get { return brush1; }
			private set { brush1 = value; NotifyChange(() => Brush1); }
		}

		private Brush brush2;
		public Brush Brush2
		{
			get { return brush2; }
			private set { brush2 = value; NotifyChange(() => Brush2); }
		}

		private Brush brush3;
		public Brush Brush3
		{
			get { return brush3; }
			private set { brush3 = value; NotifyChange(() => Brush3); }
		}

		string _path1;
		public string Path1
		{
			//get { return "M 10,90 A 30,30 25 0 1 50,10"; }
			get { return _path1 ?? ""; }
			private set { _path1 = value; NotifyChange(() => Path1); }
		}

		string _path2;
		public string Path2
		{
			get { return _path2 ?? ""; }
			private set { _path2 = value; NotifyChange(() => Path2); }
		}

		string _path3;
		public string Path3
		{
			get { return _path3 ?? ""; }
			private set { _path3 = value; NotifyChange(() => Path3); }
		}

		string _path4;
		public string Path4
		{
			get { return _path4 ?? ""; }
			private set { _path4 = value; NotifyChange(() => Path4); }
		}

		private void Announce(bool timeout)
		{
			var caption = GetAnnouncerCaption != null ? GetAnnouncerCaption(this) : "";
			this.SendAnnouncement(caption, valueFormatter(Value), timeout);
		}

		public void Update()
		{
			if (!UpdatesEnabled)
				return;

			if (Value > Max)
				Value = Max;
			if (Value < Min)
				Value = Min;

			var delta = (Value - Min) / (Max - Min);
			var angle = delta * 360 * 0.75;

			var padding = InnerPadding + Thickness / 2;
			var width = Width;
			var height = Height - 20;
			canvas.Width = width;
			canvas.Height = height;
			rect.Width = canvas.Width;
			rect.Height = Height;

			var half = width / 2;
			var radius = half - padding;

			var midX = width / 2;
			var midY = padding;

			var startX = Math.Cos(225.0 / 360 * 2 * Math.PI) * radius + half;
			var startY = -Math.Sin(225.0 / 360 * 2 * Math.PI) * radius + half;

			var endX = Math.Cos(-45.0 / 360 * 2 * Math.PI) * radius + half;
			var endY = -Math.Sin(-45.0 / 360 * 2 * Math.PI) * radius + half;

			var mainX = Math.Cos((225 - angle) / 360 * 2 * Math.PI) * radius + half;
			var mainY = -Math.Sin((225 - angle) / 360 * 2 * Math.PI) * radius + half;

			var lineStartX = Math.Cos((225 - angle) / 360 * 2 * Math.PI) * (radius - Thickness / 2 - 0.3 * radius) + half;
			var lineStartY = -Math.Sin((225 - angle) / 360 * 2 * Math.PI) * (radius - Thickness / 2 - 0.3 * radius) + half;

			var lineEndX = Math.Cos((225 - angle) / 360 * 2 * Math.PI) * (radius + Thickness / 2) + half;
			var lineEndY = -Math.Sin((225 - angle) / 360 * 2 * Math.PI) * (radius + Thickness / 2) + half;

			Path4 = string.Format(CultureInfo.InvariantCulture, "M {0},{1} L {2},{3}", lineStartX, lineStartY, lineEndX, lineEndY);

			if (!Central)
			{
				var p1Large = delta > 2 / 3.0 ? "1" : "0";
				var p2Large = delta < 1 / 3.0 ? "1" : "0";

				Path1 = string.Format(CultureInfo.InvariantCulture, "M {0},{1} A {2},{2} {3} {6} 1 {4},{5}", startX, startY, radius, 135, mainX, mainY, p1Large);
				Path2 = string.Format(CultureInfo.InvariantCulture, "M {0},{1} A {2},{2} {3} {6} 1 {4},{5}", mainX, mainY, radius, 135, endX, endY, p2Large);
				Brush1 = FillColor;
				Brush2 = Foreground;
			}
			else
			{
				if (delta <= 0.5)
				{
					Path1 = string.Format(CultureInfo.InvariantCulture, "M {0},{1} A {2},{2} {3} 0 1 {4},{5}", startX, startY, radius, 135, mainX, mainY);
					Path2 = string.Format(CultureInfo.InvariantCulture, "M {0},{1} A {2},{2} {3} 0 1 {4},{5}", mainX, mainY, radius, 135, midX, midY);
					Path3 = string.Format(CultureInfo.InvariantCulture, "M {0},{1} A {2},{2} {3} 0 1 {4},{5}", midX, midY, radius, 135, endX, endY);
				}
				else
				{
					Path1 = string.Format(CultureInfo.InvariantCulture, "M {0},{1} A {2},{2} {3} 0 1 {4},{5}", startX, startY, radius, 135, midX, midY);
					Path2 = string.Format(CultureInfo.InvariantCulture, "M {0},{1} A {2},{2} {3} 0 1 {4},{5}", midX, midY, radius, 135, mainX, mainY);
					Path3 = string.Format(CultureInfo.InvariantCulture, "M {0},{1} A {2},{2} {3} 0 1 {4},{5}", mainX, mainY, radius, 135, endX, endY);
				}

				Brush1 = Foreground;
				Brush2 = FillColor;
				Brush3 = Foreground;
			}
		}

		#region Notify Change

		public event PropertyChangedEventHandler PropertyChanged;

		// I used this and GetPropertyName to avoid having to hard-code property names
		// into the NotifyChange events. This makes the application much easier to refactor
		// leter on, if needed.
		protected void NotifyChange<T>(System.Linq.Expressions.Expression<Func<T>> exp)
		{
			var name = GetPropertyName(exp);
			NotifyChange(name);
		}

		protected void NotifyChange(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
		}

		protected static string GetPropertyName<T>(System.Linq.Expressions.Expression<Func<T>> exp)
		{
			return (((System.Linq.Expressions.MemberExpression)(exp.Body)).Member).Name;
		}

		#endregion

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Mouse.LeftButton == MouseButtonState.Released)
				return;

			if (e.ClickCount == 2)
			{
				if (DefaultValue.HasValue)
					Value = GetExternalValue(DefaultValue.Value);
				else
					Value = GetExternalValue((Max - Min) * 0.5 + Min);
			}

			Selected = true;
			Mouse.Capture(canvas);
			MousePos = e.GetPosition(this);
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			Announce(true);

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
				dx *= 0.2;

			var oldVal = internalValue;
			var val = oldVal + Delta * dx * (Max - Min);

			if (val < Min)
				val = Min;
			else if (val > Max)
				val = Max;

			if (val != oldVal)
			{
				internalValue = val;
				disableInternalUpdate = true;
				Value = GetExternalValue(val);
				disableInternalUpdate = false;
			}
		}

		private double GetExternalValue(double val)
		{
			if (Steps == 0)
				return val;

			var delta = (val - Min) / (Max - Min);
			var adjusted = Math.Round(delta * (Steps - 1));
			return (adjusted / (Steps - 1)) * (Max - Min) + Min;
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			Selected = false;
			Mouse.Capture(null);
			MousePos = e.GetPosition(this);
		}
	}
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GradientProgressRing
{
    /// <summary>
    /// Main logic for GradientProgressRing.xaml
    /// </summary>
    public partial class GradientProgressRing : UserControl
    {
        //properties
        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }
        public double Thick
        {
            get { return (double)GetValue(ThickProperty); }
            set { SetValue(ThickProperty, value); }
        }
        //dependency Properties
        public static DependencyProperty PercentageProperty;
        public static DependencyProperty ThickProperty;

        //events
        public static readonly RoutedEvent PercentageChangedEvent;
        public static readonly RoutedEvent ThickChangedEvent;

        //eventhandlers
        public event RoutedPropertyChangedEventHandler<double> PercentageChanged
        {
            add { AddHandler(PercentageChangedEvent, value); }
            remove { RemoveHandler(PercentageChangedEvent, value); }
        }
        public event RoutedPropertyChangedEventHandler<double> ThickChanged
        {
            add { AddHandler(ThickChangedEvent, value); }
            remove { RemoveHandler(ThickChangedEvent, value); }
        }

        public GradientProgressRing()
        {
            InitializeComponent();
            //default state
            Percentage = 0;
            Thick = 40;
        }

        static GradientProgressRing()
        {
            PercentageProperty = DependencyProperty.Register
                (
                "Percentage", typeof(double), typeof(GradientProgressRing), 
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnPercentageChanged))
                );
            ThickProperty = DependencyProperty.Register
                (
                "Thick", typeof(double), typeof(GradientProgressRing),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnThickChanged))
                );



        }

        //Quick HSV to RGB convetter (only HUE)
        Color getHSV(int hue)
        {
            const double tr = 4.25;
            byte red = 0; byte green = 0; byte blue = 0;
            if (hue >= 0 && hue < 60)
            {
                red = 255; green = Convert.ToByte(hue * tr);
            }
            if (hue >= 60 && hue < 120)
            {
                red = Convert.ToByte((120 - hue) * tr); green = 255;
            }
            if (hue >= 120 && hue < 180)//@!
            {
                green = 255; blue = Convert.ToByte((hue - 120) * tr);
            }
            if (hue >= 180 && hue < 240)
            {
                green = Convert.ToByte((240-hue) * tr); blue = 255;
            }
            if (hue >= 240 && hue < 300)//@!
            {
                blue = 255; red = Convert.ToByte((hue - 240) * tr);
            }
            if (hue >= 300 && hue < 360)
            {
                blue = Convert.ToByte((360 - hue) * tr); red = 255;
            }
            if (hue == 360) { red = 255; }
            Color rgbColor = new Color { A = 255, R = red, G = green, B = blue };
            return rgbColor;
        }

        //Change percentage
        private static void OnPercentageChanged(DependencyObject sender, 
            DependencyPropertyChangedEventArgs e)
        {
            
            double newPercent = (double)e.NewValue;
            GradientProgressRing rainbowLoader = (GradientProgressRing)sender;
            rainbowLoader.Percentage = newPercent;
            int roundState = (Convert.ToInt32(rainbowLoader.Percentage * 3.6));
            Point startpt = new Point(100, 100);
            rainbowLoader.CanvasPath.Children.Clear();
            for (int i = 0; i < roundState; i++)
            {
                //figure
                var xFigure = new PathFigure();
                xFigure.StartPoint = startpt;
                var xPathSegmentCollection = new PathSegmentCollection();
                //out
                var xOut = new LineSegment();
                xOut.Point = new Point(50 * Math.Cos(Math.PI * i / 180) + 100, 50 * Math.Sin(Math.PI * i / 180) + 100);
                xPathSegmentCollection.Add(xOut);
                Point ArcStart = new Point(50 * Math.Cos(Math.PI * (i + 2) / 180) + 100, 50 * Math.Sin(Math.PI * (i + 2) / 180) + 100);
                //arc
                var xArc = new ArcSegment(ArcStart, new Size(10, 10), 0, false, SweepDirection.Clockwise, false);
                xPathSegmentCollection.Add(xArc);
                //in
                var xIn = new LineSegment();
                xIn.Point = startpt;

                xPathSegmentCollection.Add(xIn);
                xFigure.Segments = xPathSegmentCollection;
                var xFigureCollection = new PathFigureCollection();
                xFigureCollection.Add(xFigure);            

                var xPathGeometry = new PathGeometry(xFigureCollection);
                Path xPath = new Path();
                xPath.Data = xPathGeometry;
                int d = Convert.ToInt32(i);
                xPath.Fill = new SolidColorBrush(rainbowLoader.getHSV(i));
                rainbowLoader.CanvasPath.Children.Add(xPath);

            }
        }

        //Change thickness
        private static void OnThickChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            double newThick = (double)e.NewValue;
            GradientProgressRing rainbowLoader = (GradientProgressRing)sender;
            rainbowLoader.Thick = newThick;
            rainbowLoader.MaskEllipse.RadiusX = newThick;
            rainbowLoader.MaskEllipse.RadiusY = newThick;
        }
    }
}

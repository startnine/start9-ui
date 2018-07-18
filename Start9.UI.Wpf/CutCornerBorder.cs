using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Start9.UI.Wpf
{
    public partial class CutCornerBorder : Border
    {
        public CornerRadius CornerDistance
        {
            get => (CornerRadius)GetValue(CornerDistanceProperty);
            set => SetValue(CornerDistanceProperty, value);
        }

        public static readonly DependencyProperty CornerDistanceProperty =
            DependencyProperty.Register("CornerDistance", typeof(CornerRadius), typeof(CutCornerBorder), new PropertyMetadata(new CornerRadius(5)));

        new CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        new static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(CutCornerBorder), new PropertyMetadata(new CornerRadius(0)));

        new public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        new public static DependencyProperty BackgroundProperty =
            DependencyProperty.RegisterAttached("Background", typeof(Brush), typeof(CutCornerBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnBackgroundPropertyChangedCallback));

        static void OnBackgroundPropertyChangedCallback(Object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as CutCornerBorder).path.Fill = (Brush)(e.NewValue);
        }

        new public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        new public static DependencyProperty BorderBrushProperty =
            DependencyProperty.RegisterAttached("BorderBrush", typeof(Brush), typeof(CutCornerBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnBorderBrushPropertyChangedCallback));

        static void OnBorderBrushPropertyChangedCallback(Object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as CutCornerBorder).path.Stroke = (Brush)(e.NewValue);
        }

        new public Double BorderThickness
        {
            get => (Double)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        new public static DependencyProperty BorderThicknessProperty =
            DependencyProperty.RegisterAttached("BorderThickness", typeof(Double), typeof(CutCornerBorder), new FrameworkPropertyMetadata((Double)0, FrameworkPropertyMetadataOptions.AffectsRender, OnBorderThicknessPropertyChangedCallback));

        static void OnBorderThicknessPropertyChangedCallback(Object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as CutCornerBorder).path.StrokeThickness = (Double)(e.NewValue);
        }

        Path path = new Path()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Visibility = Visibility.Visible
        };

        public CutCornerBorder()
        {
            base.Background = new VisualBrush()
            {
                Visual = path
            };
            //Child = path;
            SizeChanged += CutCornerBorder_SizeChanged;
        }

        private void CutCornerBorder_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            var constraint = e.NewSize;
            path.Width = constraint.Width;
            path.Height = constraint.Height;
            SetBorder();
        }

        void SetBorder()
        {
            var geom = Geometry.Parse("M 0 " + CornerDistance.TopLeft.ToString() + " L " + CornerDistance.TopLeft.ToString() + " 0 " +
                "L " + (path.Width - CornerDistance.TopRight).ToString() + " 0 L " + path.Width.ToString() + " " + CornerDistance.TopRight.ToString() + " " +
                "L " + path.Width.ToString() + " " + (path.Height - CornerDistance.BottomRight).ToString() + " L " + (path.Width - CornerDistance.BottomRight).ToString() + " " + path.Height.ToString() + " " +
                "L " + CornerDistance.BottomLeft.ToString() + " " + path.Height.ToString() + " L 0 " + (path.Height - CornerDistance.BottomLeft).ToString()
                + " Z");
            path.Data = geom;
            Clip = geom;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            path.Width = constraint.Width;
            path.Height = constraint.Height;
            SetBorder();
            return constraint;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Start9.Wpf.Styles.EDNA
{
    public enum EDNABorderStyle
    {
        None,
        WindowTitlebarActive,
        WindowTitlebarInactive,
        WindowTitlebarEdge,
        WindowBody,
        WindowFooter,

        Header,

        ComboBoxIdle,
        ComboBoxActive,
        ComboBoxDisabled,
    }

    public class EDNABorderDecorator : Decorator
    {
        static FrameworkPropertyMetadataOptions _options = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange;

        public static LinearGradientBrush ActiveTitlebarBackgroundBrush = new LinearGradientBrush(new GradientStopCollection()
            {
                new GradientStop(Color.FromArgb(0x98, 0x53, 0x5C, 0x61), 0),
                new GradientStop(Color.FromArgb(0x79, 0x6D, 0x7B, 0x82), 0.125),
                new GradientStop(Color.FromArgb(0x5C, 0x4E, 0x5B, 0x5E), 0.625),
                new GradientStop(Color.FromArgb(0x7A, 0x75, 0x82, 0x88), 0.875)
            })
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1)
        };

        public static LinearGradientBrush InactiveTitlebarBackgroundBrush = new LinearGradientBrush(new GradientStopCollection()
            {
                new GradientStop(Color.FromArgb(0x98, 0x96, 0x96, 0x96), 0),
                new GradientStop(Color.FromArgb(0x79, 0x7F, 0x7F, 0x7F), 0.125),
                new GradientStop(Color.FromArgb(0x5C, 0x5B, 0x5B, 0x5B), 0.625),
                new GradientStop(Color.FromArgb(0x7A, 0x87, 0x87, 0x87), 0.875)
                /*new GradientStop(Color.FromArgb(0x68, 0xD6, 0xD6, 0xD6), 0),
                new GradientStop(Color.FromArgb(0x49, 0xD3, 0xD3, 0xD3), 0.125),
                new GradientStop(Color.FromArgb(0x2C, 0xCE, 0xCE, 0xCE), 0.625),
                new GradientStop(Color.FromArgb(0x4A, 0xDB, 0xDB, 0xDB), 0.875)*/
            })
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1)
        };

        public static LinearGradientBrush TitlebarInnerBorderBrush = new LinearGradientBrush(new GradientStopCollection()
            {
                new GradientStop(Color.FromArgb(0x78, 0xBB, 0xD2, 0xDA), 0),
                new GradientStop(Color.FromArgb(0x60, 0xC4, 0xD7, 0xDF), 1)
            })
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1)
        };

        public static LinearGradientBrush TitlebarOuterBorderBrush = new LinearGradientBrush(new GradientStopCollection()
            {
                new GradientStop(Color.FromArgb(0x00, 0xFB, 0xFB, 0xFB), 0),
                new GradientStop(Color.FromArgb(0x82, 0xFB, 0xFB, 0xFB), 1)
            })
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1)
        };

        public static LinearGradientBrush HeaderBackgroundBrush = new LinearGradientBrush(new GradientStopCollection(){
                new GradientStop(Color.FromArgb(0x68, 0xB7, 0xCD, 0xD7), 0),
                new GradientStop(Color.FromArgb(0x49, 0xB2, 0xCA, 0xD5), 0.125),
                new GradientStop(Color.FromArgb(0x2C, 0xAD, 0xCA, 0xD0), 0.625),
                new GradientStop(Color.FromArgb(0x4A, 0xBD, 0xD2, 0xDC), 0.875)
            })
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1)
        };

        static StackPanel GetBodyBackgroundBrushVisual()
        {
            var panel = new StackPanel()
            {
                Width = 4,
                Height = 4,
                Orientation = Orientation.Vertical
            };
            panel.Children.Add(new Rectangle()
            {
                Height = 1,
                Fill = new SolidColorBrush(Color.FromArgb(0xB5, 0xA, 0xA, 0xA))
            });
            panel.Children.Add(new Rectangle()
            {
                Height = 3,
                Fill = new SolidColorBrush(Color.FromArgb(0xB5, 0x5, 0x5, 0x5))
            });
            return panel;
        }

        public static VisualBrush BodyBackgroundBrush = new VisualBrush()
        {
            TileMode = TileMode.Tile,
            ViewboxUnits = BrushMappingMode.Absolute,
            ViewportUnits = BrushMappingMode.Absolute,
            Viewbox = new Rect(0, 0, 4, 4),
            Viewport = new Rect(0, 0, 8, 8),
            Visual = GetBodyBackgroundBrushVisual()
        }; //SolidColorBrush(Color.FromArgb(0xB5, 0x05, 0x05, 0x05));

        public static RadialGradientBrush BodyBackground2Brush = new RadialGradientBrush()
        {
            GradientStops = new GradientStopCollection()
            {
                new GradientStop(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), 0.5),
                new GradientStop(Color.FromArgb(0x20, 0xFF, 0xFF, 0xFF), 1)
            },
            Center = new Point(0.5, 0.5),
            GradientOrigin = new Point(0.5, 0.5)
        };

        public static SolidColorBrush BodyBorderBrush = new SolidColorBrush(Color.FromArgb(0xA3, 0xFF, 0xFF, 0xFF));

        /*public static RadialGradientBrush FooterBackgroundBrush = new RadialGradientBrush()
        {
            GradientStops = new GradientStopCollection()
            {
                new GradientStop(Color.FromArgb(0x2B, 0xBD, 0xD5, 0xDB), 0.5),
                new GradientStop(Color.FromArgb(0xE2, 0xD0, 0xE1, 0xE6), 1)
            },
            Center = new Point(0.5, 0.5),
            GradientOrigin = new Point(0.5, 0.5)
        };*/


        static Color _footerBackgroundColor0 = Color.FromArgb(0xC1, 0xD0, 0xE1, 0xE6); //0xE2


        public static Brush GetFooterBackgroundBrush(bool vertical, double dimension)
        {
            if (vertical)
            {
                return new LinearGradientBrush()
                {
                    GradientStops = new GradientStopCollection()
                {
                    new GradientStop(_footerBackgroundColor0, (5 / dimension)),
                    new GradientStop(Colors.Transparent, (15.0 / dimension)),
                    new GradientStop(Colors.Transparent, ((dimension - 10.0) / dimension)),
                    new GradientStop(_footerBackgroundColor0, 1)
                },
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1)
                };
            }
            else
            {
                return  new LinearGradientBrush()
                {
                    GradientStops = new GradientStopCollection()
                {
                    new GradientStop(_footerBackgroundColor0, 0),
                    new GradientStop(Colors.Transparent, (10.0 / dimension)), //Color.FromArgb(0x2B, 0xBD, 0xD5, 0xDB)
                    new GradientStop(Colors.Transparent, ((dimension - 10.0) / dimension)),
                    new GradientStop(_footerBackgroundColor0, 1)
                },
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 0)//,
                                                //MappingMode = BrushMappingMode.Absolute
                };


                //brush.EndPoint = new Point(1, 0);
            }
        }

        public static LinearGradientBrush FooterBackgroundBrush = new LinearGradientBrush(new GradientStopCollection(){
                new GradientStop(Color.FromArgb(0x53, 0xC7, 0xE0, 0xE6), 0),
                new GradientStop(Color.FromArgb(0x31, 0xB6, 0xCA, 0xD0), 1) //0x2B, 0xBD, 0xD5, 0xDB)
            })
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1)
        };

        public static SolidColorBrush FooterBorderBrush = new SolidColorBrush(Color.FromArgb(0xE2, 0xD0, 0xE1, 0xE6)); //Color.FromArgb(0xFF, 0xD3, 0xE6, 0xEC)







        public static LinearGradientBrush ComboBoxIdleBackgroundBrush = new LinearGradientBrush(new GradientStopCollection()
            {
                new GradientStop(Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF), 0),
                new GradientStop(Color.FromArgb(0x33, 0x84, 0x84, 0x84), 0.625),
                new GradientStop(Color.FromArgb(0x33, 0, 0, 0), 0.625),
                new GradientStop(Color.FromArgb(0x59, 0x30, 0x62, 0x8A), 1)
            })
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1)
        };

        public static LinearGradientBrush ComboBoxIdleBorderBrush = new LinearGradientBrush(new GradientStopCollection()
            {
                new GradientStop(Color.FromArgb(0x5F, 0xFC, 0xFC, 0xFC), 0),
                new GradientStop(Color.FromArgb(0x6B, 0x50, 0x76, 0x93), 0.625),
                new GradientStop(Color.FromArgb(0x6C, 0x24, 0x49, 0x67), 0.625),
                new GradientStop(Color.FromArgb(0x88, 0x3A, 0x78, 0xA8), 1)
            })
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1)
        };

        public EDNABorderStyle BorderStyle
        {
            get => (EDNABorderStyle)GetValue(BorderStyleProperty);
            set => SetValue(BorderStyleProperty, value);
        }

        public static readonly DependencyProperty BorderStyleProperty =
                    DependencyProperty.Register(
                            nameof(BorderStyle),
                            typeof(EDNABorderStyle),
                            typeof(EDNABorderDecorator),
                            new FrameworkPropertyMetadata(EDNABorderStyle.WindowBody, _options));

        public double CutoutWidth
        {
            get => (double)GetValue(CutoutWidthProperty);
            set => SetValue(CutoutWidthProperty, value);
        }

        public static readonly DependencyProperty CutoutWidthProperty =
            DependencyProperty.Register(nameof(CutoutWidth), typeof(double), typeof(EDNABorderDecorator), new FrameworkPropertyMetadata(17.0, _options));

        public double CutoutHeight
        {
            get => (double)GetValue(CutoutHeightProperty);
            set => SetValue(CutoutHeightProperty, value);
        }

        public static readonly DependencyProperty CutoutHeightProperty =
            DependencyProperty.Register(nameof(CutoutHeight), typeof(double), typeof(EDNABorderDecorator), new FrameworkPropertyMetadata(6.0, _options));

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            EDNABorderStyle style = BorderStyle;
            switch (style)
            {
                case EDNABorderStyle.WindowTitlebarActive:
                    DrawHeader(drawingContext, 0);
                    break;
                case EDNABorderStyle.WindowTitlebarInactive:
                    DrawHeader(drawingContext, 1);
                    break;
                case EDNABorderStyle.WindowTitlebarEdge:
                    DrawHeader(drawingContext, 3);
                    break;
                case EDNABorderStyle.WindowBody:
                    DrawBody(drawingContext);
                    break;
                case EDNABorderStyle.WindowFooter:
                    DrawFooter(drawingContext);
                    break;
                case EDNABorderStyle.Header:
                    DrawHeader(drawingContext, 2);
                    break;
                case EDNABorderStyle.ComboBoxIdle:
                    DrawComboBox(drawingContext, 0);
                    break;
                case EDNABorderStyle.ComboBoxActive:
                    DrawComboBox(drawingContext, 1);
                    break;
                case EDNABorderStyle.ComboBoxDisabled:
                    DrawComboBox(drawingContext, 2);
                    break;
                default:
                    break;
            }
        }

        private void DrawHeader(DrawingContext dc, int state)
        {
            if (state == 0)
                dc.DrawGeometry(ActiveTitlebarBackgroundBrush, new Pen(TitlebarInnerBorderBrush, 1.0), GetHeaderGeometry(ActualWidth, ActualHeight));
            else if (state == 1)
                dc.DrawGeometry(InactiveTitlebarBackgroundBrush, new Pen(TitlebarInnerBorderBrush, 1.0), GetHeaderGeometry(ActualWidth, ActualHeight));
            else if (state == 2)
                dc.DrawGeometry(HeaderBackgroundBrush, new Pen(TitlebarInnerBorderBrush, 1.0), GetHeaderGeometry(ActualWidth, ActualHeight));
            else
                dc.DrawGeometry(new SolidColorBrush(Colors.Transparent), new Pen(TitlebarOuterBorderBrush, 1.0), GetHeaderGeometry(ActualWidth, ActualHeight));
        }

        private Geometry GetHeaderGeometry(double width, double height)
        {
            return PathGeometry.Parse("M 0 " + CutoutHeight + " L " + CutoutHeight + " 0 L " + (width - CutoutHeight) + " 0 L " + width + " " + CutoutHeight + "L " + width + " " + CutoutHeight + " L " + width + " " + height + " L " + (width - CutoutWidth) + " " + height + " L " + (width - (CutoutWidth + CutoutHeight)) + " " + (height - CutoutHeight) + "L " + (CutoutWidth + CutoutHeight) + " " + (height - CutoutHeight) + "L " + CutoutWidth + " " + height + " L 0 " + height + " Z");
        }

        private void DrawBody(DrawingContext dc)
        {
            var geom = GetBodyGeometry(ActualWidth, ActualHeight);
            dc.DrawGeometry(BodyBackgroundBrush, new Pen(BodyBorderBrush, 1.0), geom);
            dc.DrawGeometry(BodyBackground2Brush, null, geom);
        }

        private Geometry GetBodyGeometry(double width, double height)
        {
            return PathGeometry.Parse("M 0 " + CutoutHeight + " L " + CutoutWidth + " " + CutoutHeight + " L " + (CutoutWidth + CutoutHeight) + " 0 L " + (width - (CutoutWidth + CutoutHeight)) + " 0 L " + (width - CutoutWidth) + " " + CutoutHeight + " L " + width + " " + CutoutHeight + " L " + width + " " + (height - CutoutHeight) + " L " + (width - CutoutHeight) + " " + height + " L " + CutoutHeight + " " + height + " L 0 " + (height - CutoutHeight) + " Z");
        }

        private void DrawFooter(DrawingContext dc)
        {
            var geom = GetFooterGeometry(ActualWidth, ActualHeight);
            dc.DrawGeometry(FooterBackgroundBrush, new Pen(FooterBorderBrush, 1.0), geom);
            dc.DrawGeometry(GetFooterBackgroundBrush(false, ActualWidth), null, geom);
            dc.DrawGeometry(GetFooterBackgroundBrush(true, ActualHeight), null, geom);
        }

        private Geometry GetFooterGeometry(double width, double height)
        {
            string geomString = "M 0 0 L " + CutoutHeight + " " + CutoutHeight + " L " + (width - CutoutHeight) + " " + CutoutHeight + " L " + width + " 0 L " + width + " " + (height - CutoutHeight) + " L " + (width - CutoutHeight) + " " + height + " L " + CutoutHeight + " " + height + " L 0 " + (height - CutoutHeight) + " Z";
            Debug.WriteLine(geomString);
            return PathGeometry.Parse(geomString);
        }


        private void DrawComboBox(DrawingContext dc, int state)
        {
            if (state == 2)
                return; //dc.DrawGeometry(ComboBoxDisabledBackgroundBrush, new Pen(ComboBoxDisabledBorderBrush, 1.5), GetComboBoxGeometry(ActualWidth, ActualHeight));
            else if (state == 1)
                return; //dc.DrawGeometry(ComboBoxActiveBackgroundBrush, new Pen(ComboBoxActiveBorderBrush, 1.5), GetComboBoxGeometry(ActualWidth, ActualHeight));
            else
                dc.DrawGeometry(ComboBoxIdleBackgroundBrush, new Pen(ComboBoxIdleBorderBrush, 1.5), GetComboBoxGeometry(ActualWidth, ActualHeight));
        }

        private Geometry GetComboBoxGeometry(double width, double height)
        {
            return PathGeometry.Parse("M 0 " + CutoutWidth + " L " + CutoutWidth + " 0 L " + (width - CutoutWidth) + " 0 L " + width + " " + CutoutWidth + " L " + width + " " + (height - CutoutHeight) + " L " + (width - CutoutHeight) + " " + height + " L " + CutoutWidth + " " + height + " L 0 " + (height - CutoutWidth) + " Z");
        }
    }
}

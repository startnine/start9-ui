using Start9.UI.Wpf.Behaviors;
using System.Windows;
using System.Windows.Controls;

#if NETCOREAPP3_0
using Microsoft.Xaml.Behaviors;
#else
using System.Windows.Interactivity;
#endif

namespace Start9.UI.Wpf.Statics
{
    public class CornerCurves : Freezable
    {
        public bool TopLeft
        {
            get => (bool)GetValue(TopLeftProperty);
            set => SetValue(TopLeftProperty, value);
        }

        public static readonly DependencyProperty TopLeftProperty =
            DependencyProperty.Register("TopLeft", typeof(bool), typeof(CornerCurves), new PropertyMetadata(true));

        public bool TopRight
        {
            get => (bool)GetValue(TopRightProperty);
            set => SetValue(TopRightProperty, value);
        }

        public static readonly DependencyProperty TopRightProperty =
            DependencyProperty.Register("TopRight", typeof(bool), typeof(CornerCurves), new PropertyMetadata(true));

        public bool BottomLeft
        {
            get => (bool)GetValue(BottomLeftProperty);
            set => SetValue(BottomLeftProperty, value);
        }

        public static readonly DependencyProperty BottomLeftProperty =
            DependencyProperty.Register("BottomLeft", typeof(bool), typeof(CornerCurves), new PropertyMetadata(true));

        public bool BottomRight
        {
            get => (bool)GetValue(BottomRightProperty);
            set => SetValue(BottomRightProperty, value);
        }

        public static readonly DependencyProperty BottomRightProperty =
            DependencyProperty.Register("BottomRight", typeof(bool), typeof(CornerCurves), new PropertyMetadata(true));

        /*public bool TopLeft { get; set; } = true;
        public bool TopRight { get; set; } = true;
        public bool BottomRight { get; set; } = true;
        public bool BottomLeft { get; set; } = true;*/

        public CornerCurves()
        {
        }

        public CornerCurves(bool uniformValue)
        {
            TopLeft = uniformValue;
            TopRight = uniformValue;
            BottomRight = uniformValue;
            BottomLeft = uniformValue;
        }

        public CornerCurves(bool topLeft, bool topRight, bool bottomRight, bool bottomLeft)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new CornerCurves(true);
        }
    }

    public class AttachedProperties : DependencyObject
    {
        /*static AttachedProperties()
        {
            AttachedProperties.CornerCurvesProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(new CornerCurves(true)));
        }*/

        public static readonly DependencyProperty CornerCurvesProperty =
            DependencyProperty.RegisterAttached("CornerCurves", typeof(CornerCurves), typeof(AttachedProperties), new FrameworkPropertyMetadata(new CornerCurves(true)));

        public static CornerCurves GetCornerCurves(DependencyObject element)
        {
            return (CornerCurves)element.GetValue(CornerCurvesProperty);
        }

        public static void SetCornerCurves(DependencyObject element, CornerCurves value)
        {
            element.SetValue(CornerCurvesProperty, value);
        }

        public static readonly DependencyProperty IsContextMenuTouchableProperty =
            DependencyProperty.RegisterAttached("IsContextMenuTouchable", typeof(bool), typeof(AttachedProperties), new FrameworkPropertyMetadata(false, OnIsContextMenuTouchablePropertyChangedCallback));

        public static bool GetIsContextMenuTouchable(DependencyObject element)
        {
            return (bool)element.GetValue(IsContextMenuTouchableProperty);
        }

        public static void SetIsContextMenuTouchable(DependencyObject element, bool value)
        {
            element.SetValue(IsContextMenuTouchableProperty, value);
        }

        internal static void OnIsContextMenuTouchablePropertyChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ContextMenu menu)
            {
                if (((bool)e.NewValue) && (!(bool)e.OldValue))
                {
                    Interaction.GetBehaviors(menu).Add(new TouchableContextMenuBehavior());

                    /*if (!menu.IsOpen)
                    {
                        menu.IsOpen = true;
                        menu.IsOpen = false;
                    }*/
                }
                else if (((bool)e.OldValue) && (!(bool)e.NewValue))
                {
                    var behaviors = Interaction.GetBehaviors(menu);
                    for (int i = 0; i < behaviors.Count; i++)
                    {
                        var behavior = behaviors[i];
                        if (behavior is TouchableContextMenuBehavior)
                        {
                            Interaction.GetBehaviors(menu).Remove(behavior);
                            break;
                        }
                    }
                }
            }
        }
        
        public bool OpenedWithTouch
        {
            get => (bool)GetValue(OpenedWithTouchProperty);
            set => SetValue(OpenedWithTouchProperty, value);
        }

        public static readonly DependencyProperty OpenedWithTouchProperty = DependencyProperty.Register("OpenedWithTouch",
            typeof(bool), typeof(AttachedProperties),
            new FrameworkPropertyMetadata(false/*, FrameworkPropertyMetadataOptions.AffectsRender, OnOpenedWithTouchChanged*/));

        public static bool GetOpenedWithTouch(DependencyObject element)
        {
            return (bool)element.GetValue(OpenedWithTouchProperty);
        }

        public static void SetOpenedWithTouch(DependencyObject element, bool value)
        {
            element.SetValue(OpenedWithTouchProperty, value);
        }
    }
}

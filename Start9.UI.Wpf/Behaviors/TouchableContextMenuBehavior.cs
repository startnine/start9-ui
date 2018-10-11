using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Start9.UI.Wpf.Behaviors
{
    public class TouchableContextMenuBehavior : Behavior<ContextMenu>
    {
        ContextMenu _targetMenu;

        bool _wasOpenedWithTouch = false;

        public Boolean OpenedWithTouch
        {
            get => (Boolean)GetValue(OpenedWithTouchProperty);
            set => SetValue(OpenedWithTouchProperty, value);
        }

        public static readonly DependencyProperty OpenedWithTouchProperty = DependencyProperty.Register("OpenedWithTouch",
            typeof(Boolean), typeof(TouchableContextMenuBehavior),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnOpenedWithTouchChanged));

        static void OnOpenedWithTouchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetMenuOpenedWithTouch((d as TouchableContextMenuBehavior)._targetMenu, (bool)(e.NewValue));
            Debug.WriteLine("OnOpenedWithTouchChanged " + e.NewValue.ToString());
        }

        public static readonly DependencyProperty MenuOpenedWithTouchProperty = DependencyProperty.RegisterAttached("MenuOpenedWithTouch", typeof(bool), typeof(ContextMenu), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static bool GetMenuOpenedWithTouch(ContextMenu element)
        {
            var retrievedValue = (bool)(element.GetValue(MenuOpenedWithTouchProperty));
            Debug.WriteLine("GetMenuOpenedWithTouch " + retrievedValue.ToString());
            return retrievedValue;
        }

        public static void SetMenuOpenedWithTouch(ContextMenu element, bool value)
        {
            element.SetValue(MenuOpenedWithTouchProperty, value);
            Debug.WriteLine("SetMenuOpenedWithTouch " + value.ToString());
        }

        /*, OnAttachedTouchableBehaviorChanged*/

        /*static void OnAttachedTouchableBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //SetAttachedTouchableBehavior((d as TouchableContextMenuBehavior)._targetMenu, (bool)e.NewValue);
            Interaction.GetBehaviors((d as TouchableContextMenuBehavior)._targetMenu).Add(new TouchableContextMenuBehavior());
        }*/

        public static readonly DependencyProperty AttachedTouchableBehaviorProperty =
            DependencyProperty.RegisterAttached("AttachedTouchableBehavior", typeof(bool), typeof(ContextMenu),
                new FrameworkPropertyMetadata(false,
                                              FrameworkPropertyMetadataOptions.AffectsArrange |
                                              FrameworkPropertyMetadataOptions.AffectsMeasure |
                                              FrameworkPropertyMetadataOptions.AffectsRender |
                                              FrameworkPropertyMetadataOptions.AffectsParentArrange |
                                              FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                                              OnAttachedTouchableBehaviorChanged));

        public static void SetAttachedTouchableBehavior(DependencyObject element, bool value)
        {
            element.SetValue(AttachedTouchableBehaviorProperty, (bool)value);
        }

        public static bool GetAttachedTouchableBehavior(DependencyObject element)
        {
            return (bool)element.GetValue(AttachedTouchableBehaviorProperty);
        }

        private static void OnAttachedTouchableBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //////Interaction.GetBehaviors((d as TouchableContextMenuBehavior)._targetMenu).Add(new TouchableContextMenuBehavior());
        }

        public TouchableContextMenuBehavior()
        {

        }

        protected override void OnAttached()
        {
            _targetMenu = AssociatedObject;
            _targetMenu.Opened += (sneder, args) =>
            {
                Debug.WriteLine("_wasOpenedWithTouch: " + _wasOpenedWithTouch.ToString());
                OpenedWithTouch = _wasOpenedWithTouch;
            };
            if (_targetMenu.IsLoaded)
                Load();
            else
                _targetMenu.Loaded += (sneder, args) =>
                {
                    Load();
                };
            base.OnAttached();
        }

        void Load()
        {
            var source = ContextMenuService.GetPlacementTarget(_targetMenu);
            var placeTarget = _targetMenu.PlacementTarget as UIElement;
            if (source != null)
            {
                (source as UIElement).TouchDown += Source_TouchDown;
                (source as UIElement).MouseDown += Source_MouseDown;
            }
            else if (placeTarget != null)
            {
                placeTarget.TouchDown += Source_TouchDown;
                placeTarget.MouseDown += Source_MouseDown;
            }
        }

        private void Source_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            if (!((e.OriginalSource as UIElement).AreAnyTouchesOver))
                _wasOpenedWithTouch = false;
        }

        private void Source_TouchDown(Object sender, TouchEventArgs e)
        {
            //TouchStarted = DateTime.Now;
            Timer touchTimer = new Timer(1);
            touchTimer.Elapsed += delegate
            {
                _targetMenu.Dispatcher.Invoke(new Action(() =>
                {
                    /*if (IsOpen)
                    {*/
                    _wasOpenedWithTouch = true;
                    /*}
                    else */
                    if (!((e.OriginalSource as UIElement).AreAnyTouchesOver))
                    {
                        touchTimer.Stop();
                    }
                }));
            };
            touchTimer.Start();
        }
    }

    public static class ContextMenuProperties
    {

    }
}
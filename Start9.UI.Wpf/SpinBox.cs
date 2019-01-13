using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Start9.UI.Wpf
{
    [TemplatePart(Name = PartDecrementButton, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PartIncrementButton, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PartContentBox, Type = typeof(TextBox))]
    public class SpinBox : Control
    {
        const String PartDecrementButton = "PART_DecrementButton";
        const String PartIncrementButton = "PART_IncrementButton";
        const String PartContentBox = "PART_ContentBox";

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(SpinBox), new PropertyMetadata(0.0, OnValuePropertyChangedCallback));

        static void OnValuePropertyChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            var box = sender as SpinBox;
            //string val = box.Value.ToString();

            //if (box._contentBox.Text != val)
            box.ValidateContent(); //contentBox.Text = e.NewValue.ToString();
        }

        void ValidateContent()
        {
            if (_contentBox != null)
                _contentBox.Text = Value.ToString();
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(SpinBox), new PropertyMetadata(double.PositiveInfinity));

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(SpinBox), new PropertyMetadata(0.0));

        public double Increment
        {
            get => (double)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment", typeof(double), typeof(SpinBox), new PropertyMetadata(1.0));

        RepeatButton _decrementButton;
        RepeatButton _incrementButton;
        TextBox _contentBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _decrementButton = GetTemplateChild(PartDecrementButton) as RepeatButton;
            if (_decrementButton != null)
                _decrementButton.Click += DecrementButton_Click;

            _incrementButton = GetTemplateChild(PartIncrementButton) as RepeatButton;
            if (_incrementButton != null)
                _incrementButton.Click += IncrementButton_Click;

            _contentBox = GetTemplateChild(PartContentBox) as TextBox;
            if (_contentBox != null)
            {
                _contentBox.TextChanged += ContentBox_TextChanged;
                _contentBox.LostFocus += ContentBox_LostFocus;
                ValidateContent();
            }
        }

        private void DecrementButton_Click(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine(Increment);
            double newVal = Value - Increment;
            if (newVal >= Minimum)
                Value = newVal;
        }

        private void IncrementButton_Click(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine(Increment);
            double newVal = Value + Increment;
            if (newVal <= Maximum)
                Value = newVal;
        }

        private void ContentBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_contentBox != null)
            {
                //Debug.WriteLine("TYPE: " + e.OriginalSource.GetType().FullName);
                if (double.TryParse(_contentBox.Text, out double val))
                {
                    if (Value != val)
                        Value = val;
                }
                //Debug.WriteLine("New value: " + Value.ToString());
            }
        }

        private void ContentBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateContent();
        }
    }
}

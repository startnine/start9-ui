using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Start9.UI.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for MessageBoxContent.xaml
    /// </summary>
    internal partial class MessageBoxContent : UserControl
    {
        internal ObservableCollection<string> EnumStrings
        {
            get => (ObservableCollection<string>)GetValue(EnumStringsProperty);
            set => SetValue(EnumStringsProperty, value);
        }

        internal static readonly DependencyProperty EnumStringsProperty =
                    DependencyProperty.RegisterAttached("EnumStrings", typeof(ObservableCollection<string>), typeof(MessageBoxContent),
                        new PropertyMetadata(new ObservableCollection<string>()));

        internal string ValueString
        {
            get => (string)GetValue(ValueStringProperty);
            set => SetValue(ValueStringProperty, value);
        }

        internal static readonly DependencyProperty ValueStringProperty =
                    DependencyProperty.RegisterAttached("ValueString", typeof(string), typeof(MessageBoxContent),
                        new PropertyMetadata());

        Type _enumType;

        internal MessageBoxContent(Type enumType, string bodyText)
        {
            InitializeComponent();
            _enumType = enumType;

            foreach (string s in Enum.GetNames(_enumType))
            {
                EnumStrings.Add(s);
            }

            BodyTextBlock.Text = bodyText;
        }

        internal event EventHandler<MessageBoxEventArgs> ResultButtonClicked;

        private void EnumButton_Click(object sender, RoutedEventArgs e)
        {
            ValueString = (sender as Button).Content as string;
            ResultButtonClicked.Invoke(sender, new MessageBoxEventArgs(_enumType, Enum.Parse(_enumType, ValueString)));
            Window.GetWindow(this).Close();
        }

        private void ButtonsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValueString = (sender as ListView).SelectedItem as string;
            ResultButtonClicked.Invoke(sender, new MessageBoxEventArgs(_enumType, Enum.Parse(_enumType, ValueString)));
            Window.GetWindow(this).Close();
        }
    }

    public class MessageBoxEventArgs : EventArgs
    {
        Type _type;

        internal MessageBoxEventArgs(Type type, object value)
        {

            Result = value;
        }

        public Type Type { get; }

        public object Result { get; }
    }
}

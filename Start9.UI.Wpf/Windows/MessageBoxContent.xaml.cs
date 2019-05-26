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
        internal ObservableCollection<MessageBoxAction> Actions
        {
            get => (ObservableCollection<MessageBoxAction>)GetValue(ActionsProperty);
            set => SetValue(ActionsProperty, value);
        }

        internal static readonly DependencyProperty ActionsProperty =
                    DependencyProperty.RegisterAttached(nameof(Actions), typeof(ObservableCollection<MessageBoxAction>), typeof(MessageBoxContent),
                        new PropertyMetadata(new ObservableCollection<MessageBoxAction>()));

        internal MessageBoxAction SelectedAction
        {
            get => (MessageBoxAction)GetValue(SelectedActionProperty);
            set => SetValue(SelectedActionProperty, value);
        }

        internal static readonly DependencyProperty SelectedActionProperty =
                    DependencyProperty.RegisterAttached(nameof(SelectedAction), typeof(MessageBoxAction), typeof(MessageBoxContent),
                        new PropertyMetadata());


        internal MessageBoxContent(IMessageBoxActionSet actionSet, string bodyText, FrameworkElement icon)
        {
            InitializeComponent();
            Actions.Clear();
            foreach (object s in actionSet.GetValues())
                Actions.Add(new MessageBoxAction(s, actionSet.GetDisplayName(s)));

            BodyTextBlock.Text = bodyText;

            if (icon != null)
            {
                IconContentControl.Visibility = Visibility.Visible;
                IconContentControl.Content = icon;
            }
            else
                IconContentControl.Visibility = Visibility.Collapsed;
        }

        internal event EventHandler<MessageBoxEventArgs> ResultButtonClicked;

        /*private void EnumButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedAction = (sender as Button).Content as string;
            EndDialog(sender, SelectedAction);
        }

        private void ButtonsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedAction = (sender as ListView).SelectedItem as string;
            EndDialog(sender, SelectedAction);
        }*/

        public void EndDialog(object sender, MessageBoxAction value)
        {
            ResultButtonClicked.Invoke(sender, new MessageBoxEventArgs(value.Value));
            Window.GetWindow(this).Close();
        }
    }

    public class MessageBoxEventArgs : EventArgs
    {
        internal MessageBoxEventArgs(object value)
        {
            Result = value;
        }

        public object Result { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

/*#if NETCOREAPP3_0
using Microsoft.Xaml.Behaviors;
#else*/
using System.Windows.Interactivity;
//#endif
using Start9.UI.Wpf.Windows;

namespace Start9.UI.Wpf.Behaviors
{
    public class MessageBoxButtonBehavior : Behavior<Button>
    {
        public MessageBoxContent Content
        {
            get => (MessageBoxContent)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(MessageBoxContent), typeof(MessageBoxButtonBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            /*if (Window.GetWindow(AssociatedObject).Content is MessageBoxContent content)
            {*/
            AssociatedObject.Click += (sneder, args) =>
            {
                Content.EndDialog(AssociatedObject, (MessageBoxAction)AssociatedObject.DataContext);
                //content.ButtonsListView.SelectedIndex = content.EnumStrings.IndexOf((string)(AssociatedObject.Content));
            };
            //}
        }
    }
}

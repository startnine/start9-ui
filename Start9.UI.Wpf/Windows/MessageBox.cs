using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Start9.UI.Wpf.Windows
{
    public static class MessageBoxEnums
    {
        public enum OkButton
        {
            OK
        }

        public enum OkCancelButtons
        {
            OK,
            Cancel
        }

        public enum IgnoreRetryAbortButtons
        {
            Ignore,
            Retry,
            Abort
        }

        public enum YesNoButtons
        {
            Yes,
            No
        }
    }

    public static class MessageBox<T>
    {
        public enum TargetContainer
        {
            Client,
            Title,
            FullWindow
        }

        public static T Show()
        {
            return Show(string.Empty, string.Empty, TargetContainer.Client, null);
        }

        public static T Show(ResourceDictionary skin)
        {
            return Show(string.Empty, string.Empty, TargetContainer.Client, skin);
        }

        public static T Show(TargetContainer container)
        {
            return Show(string.Empty, string.Empty, container, null);
        }

        public static T Show(TargetContainer container, ResourceDictionary skin)
        {
            return Show(string.Empty, string.Empty, container, skin);
        }

        public static T Show(string text)
        {
            return Show(text, string.Empty, TargetContainer.Client, null);
        }

        public static T Show(string text, ResourceDictionary skin)
        {
            return Show(text, string.Empty, TargetContainer.Client, skin);
        }

        public static T Show(string text, TargetContainer container)
        {
            return Show(text, string.Empty, container, null);
        }

        public static T Show(string text, TargetContainer container, ResourceDictionary skin)
        {
            return Show(text, string.Empty, container, skin);
        }

        public static T Show(string text, string caption)
        {
            return Show(text, caption, TargetContainer.Client, null);
        }

        public static T Show(string text, string caption, ResourceDictionary skin)
        {
            return Show(text, caption, TargetContainer.Client, skin);
        }

        public static T Show(string text, string caption, TargetContainer container)
        {
            return Show(text, caption, container, null);
        }

        //Show(type, text, caption, container, skin);
        public static T Show(string text, string caption, TargetContainer container, ResourceDictionary skin)
        {
            MessageBoxContent content = new MessageBoxContent(typeof(T), text);
            DecoratableWindow window = new DecoratableWindow()
            {
                Title = caption,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight
            };

            if (skin != null)
                window.Resources.MergedDictionaries.Add(skin);

            if (container == TargetContainer.Client)
                window.Content = content;
            else if (container == TargetContainer.Title)
                window.TitleBarContent = content;
            else if (container == TargetContainer.FullWindow)
                window.FullWindowContent = content;

            T value = default(T);

            content.ResultButtonClicked += (sneder, args) =>
            {
                var arg = (args as MessageBoxEventArgs);
                value = (T)(arg.Result);
                //window.Close();
            };

            window.ShowDialog();

            return value;
        }
    }
}

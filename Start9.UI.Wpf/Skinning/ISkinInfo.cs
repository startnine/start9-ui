using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Start9.UI.Wpf.Skinning
{
    public interface ISkinInfo
    {
        Page GetSettingsPage();

        ResourceDictionary OnApplySkinSettings();

        string SkinName
        {
            get;
        }
    }
}

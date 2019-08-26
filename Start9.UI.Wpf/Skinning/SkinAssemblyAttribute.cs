using System;
using System.Collections.Generic;
using System.Text;

namespace Start9.UI.Wpf.Skinning
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class SkinAssemblyAttribute : Attribute
    {
        public Type InterfaceImplType;

        public SkinAssemblyAttribute(Type interfaceImplType)
        {
            InterfaceImplType = interfaceImplType;
            bool containsSkinInfo = false;
            foreach (Type intr in InterfaceImplType.GetInterfaces())
            {
                if (intr == typeof(ISkinInfo))
                {
                    containsSkinInfo = true;
                    break;
                }
            }
            if (!containsSkinInfo)
                throw new Exception("Type must implement \"Start9.UI.Wpf.Skinning.ISkinInfo\"!");
        }
    }
}

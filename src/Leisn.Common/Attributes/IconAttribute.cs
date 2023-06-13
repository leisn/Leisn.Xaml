using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leisn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IconAttribute : Attribute
    {
        public string IconName { get; }
        public IconAttribute(string iconName)
        {
            IconName = iconName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Leisn.Common
{
    public static class AttributeEx
    {
        public static bool Contains<T>(this AttributeCollection self) where T : Attribute
        {
            return self.OfType<T>().Any();
        }
        public static T? Attr<T>(this PropertyDescriptor propertyDescriptor) where T : Attribute
        {
            return propertyDescriptor.Attributes.OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> Attrs<T>(this PropertyDescriptor propertyDescriptor) where T : Attribute
        {
            return propertyDescriptor.Attributes.OfType<T>();
        }
    }
}

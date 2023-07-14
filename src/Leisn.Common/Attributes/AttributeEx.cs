// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel
{
    public static class AttributeEx
    {
        public static bool Contains<T>(this AttributeCollection self) where T : Attribute
        {
            return self.OfType<T>().Any();
        }
        public static bool ContainsAttribute<T>(this PropertyDescriptor self) where T : Attribute
        {
            return self.Attributes.OfType<T>().Any();
        }
        public static T? Attr<T>(this PropertyDescriptor propertyDescriptor) where T : Attribute
        {
            return propertyDescriptor.Attributes.Attr<T>();
        }
        public static T? Attr<T>(this AttributeCollection attributes) where T : Attribute
        {
            return attributes.OfType<T>().FirstOrDefault();
        }
        public static IEnumerable<T> Attrs<T>(this PropertyDescriptor propertyDescriptor) where T : Attribute
        {
            return propertyDescriptor.Attributes.OfType<T>();
        }

    }
}

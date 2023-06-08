using System;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UpdateSourceTriggerAttribute : Attribute
    {
        public UpdateSourceTrigger Trigger { get; }
        public UpdateSourceTriggerAttribute(UpdateSourceTrigger trigger)
        {
            Trigger = trigger;
        }
    }
}

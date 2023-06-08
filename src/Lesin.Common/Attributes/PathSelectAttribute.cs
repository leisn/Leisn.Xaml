using System;

namespace Leisn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PathSelectAttribute : Attribute
    {
        public bool IsSelectFolder { get; set; } = true;
        public string? FileFilter { get; set; }
        public string? DialogTitle { get; set; }
        public bool IsTextReadOnly { get; set; } = true;
    }
}

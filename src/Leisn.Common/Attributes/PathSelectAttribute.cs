using Leisn.Common.Models;

using System;

namespace Leisn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PathSelectAttribute : Attribute
    {
        public PathSelectMode Mode { get; set; }
        public string? FileFilter { get; set; }
        public string? DialogTitle { get; set; }
        public bool IsTextReadOnly { get; set; } = true;
    }
}

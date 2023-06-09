using Leisn.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Models
{
#nullable disable
    public class DataDeclaration : IDataDeclaration<object>
    {
        public object Value { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }
    }
#nullable enable
}

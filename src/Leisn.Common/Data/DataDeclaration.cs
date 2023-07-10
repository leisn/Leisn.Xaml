// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.Common.Data
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

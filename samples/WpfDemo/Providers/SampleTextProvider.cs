using Leisn.Common.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDemo.Providers
{
    internal class SampleTextProvider : IDataProvider<string>
    {
        public IEnumerable<string> GetData()
        {
            return new List<string>()
            {
                "Sample Text1",
                "Sample Text2",
                "Sample Text3",
                "Sample Text4",
                "Sample Text5",
            };
        }
    }
}

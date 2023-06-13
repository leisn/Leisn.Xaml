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
        private static SampleTextProvider _instance = null!;
        public SampleTextProvider()
        {
            if (_instance != null)
                throw new InvalidOperationException("Already has a instance.");
            _instance = this;
        }
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

using Leisn.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfDemo.Providers
{
    internal class SampleTextWidthDescProvider : IDataProvider<string[]>
    {
        public IEnumerable<string[]> GetData()
        {
            return new List<string[]>()
            {
                new string[] { "Sample Text1", "Name1","Sample Desc 1" },
                new string[] { "Sample Text2", "Name2","Sample Desc 2" },
                new string[] { "Sample Text3", "Name3","Sample Desc 3" },
                new string[] { "Sample Text4", "Name4","Sample Desc 4" },
            };
        }


    }
}

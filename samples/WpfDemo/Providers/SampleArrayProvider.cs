using Leisn.Common.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfDemo.Providers
{
    internal class SampleArrayProvider : IDataProvider<object[]>
    {
        public IEnumerable<object[]> GetData()
        {
            return new List<object[]>()
            {
                //value,display name,desc
                new object[] { 1, "Name1","Sample Desc 1" },
                new object[] { 2, "Name2","Sample Desc 2" },
                new object[] { 3, "Name3","Sample Desc 3" },
                new object[] { 4, "Name4","Sample Desc 4" },
            };
        }


    }
}

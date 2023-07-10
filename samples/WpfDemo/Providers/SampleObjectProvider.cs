using Leisn.Common.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDemo.Providers
{
    internal class SampleObject/* : IDataDeclaration<int>*/
    {
        public int Value { get; set; }
    }

    internal class SampleObjectDeclaration : IDataDeclaration<SampleObject>
    {
        public SampleObject Value { get; }
        public SampleObjectDeclaration(SampleObject value)
        {
            Value = value;
        }

        public string DisplayName => $"Value={Value.Value}";

        public string Description => $"HashCode={Value.GetHashCode()}";
    }

    internal class SampleObjectProvider : IDataProvider<SampleObjectDeclaration>
    {
        public IEnumerable<SampleObjectDeclaration> GetData()
        {
            return new List<SampleObjectDeclaration>
            {
                new SampleObjectDeclaration(new SampleObject{ Value=1 }),
                 new SampleObjectDeclaration(new SampleObject{ Value=2 }),
                  new SampleObjectDeclaration(new SampleObject { Value = 3 } )
            };
        }
    }
}

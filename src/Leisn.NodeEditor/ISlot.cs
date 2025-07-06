// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.NodeEditor;

public interface ISlot
{
    string Header { get; set; }
}

public class NodeSlot : ISlot
{
    public string Header { get; set; } = null!;
}

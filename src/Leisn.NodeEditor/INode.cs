// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.NodeEditor;

public interface INode
{
    string Header { get; set; }
    ISlot[] Inputs { get; set; }
    ISlot[] Outputs { get; set; }
}

using Microsoft.OData.Core.UriParser.TreeNodeKinds;

namespace ALS.Glance.Api.Helpers.Binder
{
    public class BinderNode
    {
        // Summary:
        //     Gets the left operand.
        public string Left { get; set; }
        //
        // Summary:
        //     Gets the operator represented by this node.
        public BinaryOperatorKind OperatorKind { get; set; }
        //
        // Summary:
        //     Gets the right operand.
        public string Right { get; set; }
    }
}
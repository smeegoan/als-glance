using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.OData.Query;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;

namespace ALS.Glance.Api.Helpers.Binder
{
    public class ODataQueryOptionsBinder : IODataQueryOptionsBinder
    {

        public ICollection<BinderNode> BindFilter(FilterQueryOption filterQuery, Action<Exception> exceptionHandler)
        {
            var nodes = new HashSet<BinderNode>();
            if (filterQuery != null)
            {
                try
                {
                    Bind(filterQuery.FilterClause.Expression, nodes);
                }
                catch (Exception ex)
                {
                    exceptionHandler(ex);
                }
            }

            return nodes;
        }

        protected string Bind(QueryNode node, ICollection<BinderNode> nodes)
        {
            var collectionNode = node as CollectionNode;
            var singleValueNode = node as SingleValueNode;

            if (collectionNode != null)
            {
                switch (node.Kind)
                {
                    case QueryNodeKind.CollectionNavigationNode:
                        var navigationNode = node as CollectionNavigationNode;
                        return BindNavigationPropertyNode(navigationNode.Source, navigationNode.NavigationProperty, nodes);

                    case QueryNodeKind.CollectionPropertyAccess:
                        return BindCollectionPropertyAccessNode(node as CollectionPropertyAccessNode, nodes);
                }
            }
            else if (singleValueNode != null)
            {
                switch (node.Kind)
                {
                    case QueryNodeKind.BinaryOperator:
                        return BindBinaryOperatorNode(node as BinaryOperatorNode, nodes);

                    case QueryNodeKind.Constant:
                        return BindConstantNode(node as ConstantNode);

                    case QueryNodeKind.Convert:
                        return BindConvertNode(node as ConvertNode, nodes);

                    case QueryNodeKind.EntityRangeVariableReference:
                        return string.Empty;

                    case QueryNodeKind.NonentityRangeVariableReference:
                        return BindRangeVariable((node as NonentityRangeVariableReferenceNode).RangeVariable);

                    case QueryNodeKind.SingleValuePropertyAccess:
                        return BindPropertyAccessQueryNode(node as SingleValuePropertyAccessNode, nodes);

                    case QueryNodeKind.UnaryOperator:
                        return BindUnaryOperatorNode(node as UnaryOperatorNode, nodes);

                    case QueryNodeKind.SingleValueFunctionCall:
                        return BindSingleValueFunctionCallNode(node as SingleValueFunctionCallNode, nodes);

                    case QueryNodeKind.SingleNavigationNode:
                        var navigationNode = node as SingleNavigationNode;
                        return BindNavigationPropertyNode(navigationNode.Source, navigationNode.NavigationProperty, nodes);

                    case QueryNodeKind.Any:
                        return BindAnyNode(node as AnyNode, nodes);

                    case QueryNodeKind.All:
                        return BindAllNode(node as AllNode, nodes);
                }
            }

            throw new NotSupportedException(String.Format("Nodes of type {0} are not supported", node.Kind));
        }

        #region Private Methods

        private string BindCollectionPropertyAccessNode(CollectionPropertyAccessNode collectionPropertyAccessNode, ICollection<BinderNode> nodes)
        {
            return Bind(collectionPropertyAccessNode.Source, nodes) + "." + collectionPropertyAccessNode.Property.Name;
        }

        private string BindNavigationPropertyNode(SingleValueNode singleValueNode, IEdmNavigationProperty edmNavigationProperty, ICollection<BinderNode> nodes)
        {
            var bindedNode = Bind(singleValueNode, nodes);
            if (!string.IsNullOrEmpty(bindedNode))
                bindedNode += "/";
            return bindedNode + edmNavigationProperty.Name;
        }

        private string BindAllNode(AllNode allNode, ICollection<BinderNode> nodes)
        {
            string innerQuery = "not exists ( from " + Bind(allNode.Source, nodes) + " " + allNode.RangeVariables.First().Name;
            innerQuery += " where NOT(" + Bind(allNode.Body, nodes) + ")";
            return innerQuery + ")";
        }

        private string BindAnyNode(AnyNode anyNode, ICollection<BinderNode> nodes)
        {
            const string pattern = @"^\.";
            var rgx = new Regex(pattern);

            var source = rgx.Replace(Bind(anyNode.Source, nodes), String.Empty);
            string body = null;

            var innerQuery = "exists ( from " + source + " " + anyNode.RangeVariables.First().Name;
            if (anyNode.Body != null)
            {
                body = Bind(anyNode.Body, nodes);
                innerQuery += " where " + body;
            }

            nodes.Add(new BinderNode
            {
                Left = source,
                OperatorKind = BinaryOperatorKind.Equal,
                Right = body,
            });

            return innerQuery + ")";
        }

        private string BindSingleValueFunctionCallNode(SingleValueFunctionCallNode singleValueFunctionCallNode, ICollection<BinderNode> nodes)
        {
            var arguments = singleValueFunctionCallNode.Parameters.ToList();
            switch (singleValueFunctionCallNode.Name)
            {
                case "concat":
                    return singleValueFunctionCallNode.Name + "(" + Bind(arguments[0], nodes) + "," + Bind(arguments[1], nodes) + ")";
                default:
                    return singleValueFunctionCallNode.Name + "(" + Bind(arguments[0], nodes) + ")";
            }
        }

        private string BindUnaryOperatorNode(UnaryOperatorNode unaryOperatorNode, ICollection<BinderNode> nodes)
        {
            return ToString(unaryOperatorNode.OperatorKind) + "(" + Bind(unaryOperatorNode.Operand, nodes) + ")";
        }

        private string BindPropertyAccessQueryNode(SingleValuePropertyAccessNode singleValuePropertyAccessNode, ICollection<BinderNode> nodes)
        {
            var bindedNode = Bind(singleValuePropertyAccessNode.Source, nodes);
            if (!string.IsNullOrEmpty(bindedNode))
                bindedNode += "/";
            return bindedNode + singleValuePropertyAccessNode.Property.Name;
        }

        private string BindRangeVariable(NonentityRangeVariable nonentityRangeVariable)
        {
            return nonentityRangeVariable.Name;
        }



        private string BindConvertNode(ConvertNode convertNode, ICollection<BinderNode> nodes)
        {
            return Bind(convertNode.Source, nodes);
        }

        private string BindConstantNode(ConstantNode constantNode)
        {
            //MR# sem este if o valor de qualquer enum tinha o valor "Microsoft.OData.Core.ODataEnumValue"
            if (Convert.ToString(constantNode.Value) == "Microsoft.OData.Core.ODataEnumValue")
            {
                var enumType = (Microsoft.OData.Core.ODataEnumValue)constantNode.Value;

                return Convert.ToString(enumType.Value);
            }
            return Convert.ToString(constantNode.Value);
        }

        private string BindBinaryOperatorNode(BinaryOperatorNode binaryOperatorNode, ICollection<BinderNode> nodes)
        {
            var left = Bind(binaryOperatorNode.Left, nodes);
            var right = Bind(binaryOperatorNode.Right, nodes);

            nodes.Add(new BinderNode
            {
                Left = left,
                OperatorKind = binaryOperatorNode.OperatorKind,
                Right = right,
            });
            return "(" + left + " " + ToString(binaryOperatorNode.OperatorKind) + " " + right + ")";
        }

        private string ToString(BinaryOperatorKind binaryOpertor)
        {
            switch (binaryOpertor)
            {
                case BinaryOperatorKind.Add:
                    return "+";
                case BinaryOperatorKind.And:
                    return "AND";
                case BinaryOperatorKind.Divide:
                    return "/";
                case BinaryOperatorKind.Equal:
                    return "=";
                case BinaryOperatorKind.GreaterThan:
                    return ">";
                case BinaryOperatorKind.GreaterThanOrEqual:
                    return ">=";
                case BinaryOperatorKind.LessThan:
                    return "<";
                case BinaryOperatorKind.LessThanOrEqual:
                    return "<=";
                case BinaryOperatorKind.Modulo:
                    return "%";
                case BinaryOperatorKind.Multiply:
                    return "*";
                case BinaryOperatorKind.NotEqual:
                    return "!=";
                case BinaryOperatorKind.Or:
                    return "OR";
                case BinaryOperatorKind.Subtract:
                    return "-";
                default:
                    return null;
            }
        }

        private string ToString(UnaryOperatorKind unaryOperator)
        {
            switch (unaryOperator)
            {
                case UnaryOperatorKind.Negate:
                    return "!";
                case UnaryOperatorKind.Not:
                    return "NOT";
                default:
                    return null;
            }
        }

        #endregion
    }
}
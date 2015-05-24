using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.OData.Query;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;

namespace ALS.Glance.Api.Helpers.Binder
{
    public class ODataQueryOptionsBinder : IODataQueryOptionsBinder
    {
        private readonly List<object> _positionalParmeters = new List<object>();
        private readonly ICollection<BinderNode> _parameters =
            new HashSet<BinderNode>();

        public ICollection<BinderNode> BindFilter(FilterQueryOption filterQuery, Action<Exception> exceptionHandler)
        {
            _parameters.Clear();
            _positionalParmeters.Clear();
            if (filterQuery != null)
            {
                try
                {
                    Bind(filterQuery.FilterClause.Expression);
                }
                catch (Exception ex)
                {
                    exceptionHandler(ex);
                }
            }

            return _parameters;
        }

        protected string Bind(QueryNode node)
        {
            var collectionNode = node as CollectionNode;
            var singleValueNode = node as SingleValueNode;

            if (collectionNode != null)
            {
                switch (node.Kind)
                {
                    case QueryNodeKind.CollectionNavigationNode:
                        var navigationNode = node as CollectionNavigationNode;
                        return BindNavigationPropertyNode(navigationNode.Source, navigationNode.NavigationProperty);

                    case QueryNodeKind.CollectionPropertyAccess:
                        return BindCollectionPropertyAccessNode(node as CollectionPropertyAccessNode);
                }
            }
            else if (singleValueNode != null)
            {
                switch (node.Kind)
                {
                    case QueryNodeKind.BinaryOperator:
                        return BindBinaryOperatorNode(node as BinaryOperatorNode);

                    case QueryNodeKind.Constant:
                        return BindConstantNode(node as ConstantNode);

                    case QueryNodeKind.Convert:
                        return BindConvertNode(node as ConvertNode);

                    case QueryNodeKind.EntityRangeVariableReference:
                        return string.Empty;

                    case QueryNodeKind.NonentityRangeVariableReference:
                        return BindRangeVariable((node as NonentityRangeVariableReferenceNode).RangeVariable);

                    case QueryNodeKind.SingleValuePropertyAccess:
                        return BindPropertyAccessQueryNode(node as SingleValuePropertyAccessNode);

                    case QueryNodeKind.UnaryOperator:
                        return BindUnaryOperatorNode(node as UnaryOperatorNode);

                    case QueryNodeKind.SingleValueFunctionCall:
                        return BindSingleValueFunctionCallNode(node as SingleValueFunctionCallNode);

                    case QueryNodeKind.SingleNavigationNode:
                        var navigationNode = node as SingleNavigationNode;
                        return BindNavigationPropertyNode(navigationNode.Source, navigationNode.NavigationProperty);

                    case QueryNodeKind.Any:
                        return BindAnyNode(node as AnyNode);

                    case QueryNodeKind.All:
                        return BindAllNode(node as AllNode);
                }
            }

            throw new NotSupportedException(String.Format("Nodes of type {0} are not supported", node.Kind));
        }

        #region Private Methods

        private string BindCollectionPropertyAccessNode(CollectionPropertyAccessNode collectionPropertyAccessNode)
        {
            return Bind(collectionPropertyAccessNode.Source) + "." + collectionPropertyAccessNode.Property.Name;
        }

        private string BindNavigationPropertyNode(SingleValueNode singleValueNode, IEdmNavigationProperty edmNavigationProperty)
        {
            var bindedNode = Bind(singleValueNode);
            if (!string.IsNullOrEmpty(bindedNode))
                bindedNode += "/";
            return bindedNode + edmNavigationProperty.Name;
        }

        private string BindAllNode(AllNode allNode)
        {
            string innerQuery = "not exists ( from " + Bind(allNode.Source) + " " + allNode.RangeVariables.First().Name;
            innerQuery += " where NOT(" + Bind(allNode.Body) + ")";
            return innerQuery + ")";
        }

        private string BindAnyNode(AnyNode anyNode)
        {
            string innerQuery = "exists ( from " + Bind(anyNode.Source) + " " + anyNode.RangeVariables.First().Name;
            if (anyNode.Body != null)
            {
                innerQuery += " where " + Bind(anyNode.Body);
            }
            return innerQuery + ")";
        }

        private string BindSingleValueFunctionCallNode(SingleValueFunctionCallNode singleValueFunctionCallNode)
        {
            var arguments = singleValueFunctionCallNode.Parameters.ToList();
            switch (singleValueFunctionCallNode.Name)
            {
                case "concat":
                    return singleValueFunctionCallNode.Name + "(" + Bind(arguments[0]) + "," + Bind(arguments[1]) + ")";
                default:
                    return singleValueFunctionCallNode.Name + "(" + Bind(arguments[0]) + ")";
            }
        }

        private string BindUnaryOperatorNode(UnaryOperatorNode unaryOperatorNode)
        {
            return ToString(unaryOperatorNode.OperatorKind) + "(" + Bind(unaryOperatorNode.Operand) + ")";
        }

        private string BindPropertyAccessQueryNode(SingleValuePropertyAccessNode singleValuePropertyAccessNode)
        {
            var bindedNode = Bind(singleValuePropertyAccessNode.Source);
            if (!string.IsNullOrEmpty(bindedNode))
                bindedNode += "/";
            return bindedNode + singleValuePropertyAccessNode.Property.Name;
        }

        private string BindRangeVariable(NonentityRangeVariable nonentityRangeVariable)
        {
            return nonentityRangeVariable.Name;
        }



        private string BindConvertNode(ConvertNode convertNode)
        {
            return Bind(convertNode.Source);
        }

        private string BindConstantNode(ConstantNode constantNode)
        {
            _positionalParmeters.Add(constantNode.Value);
            return Convert.ToString(constantNode.Value);
        }

        private string BindBinaryOperatorNode(BinaryOperatorNode binaryOperatorNode)
        {
            var left = Bind(binaryOperatorNode.Left);
            var right = Bind(binaryOperatorNode.Right);

            _parameters.Add(new BinderNode
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
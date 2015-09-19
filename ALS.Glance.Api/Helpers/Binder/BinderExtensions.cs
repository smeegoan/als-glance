using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.OData.Query;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;

namespace ALS.Glance.Api.Helpers.Binder
{
    internal static class BinderExtensions
    {
        public static ICollection<string> GetExpandedFields(this SelectExpandQueryOption selectExpandQueryOption)
        {
           return GetExpandedFields(selectExpandQueryOption.SelectExpandClause.SelectedItems);
        }

        public static bool IsValid(SelectExpandQueryOption selectExpandQueryOption)
        {
            return selectExpandQueryOption?.SelectExpandClause?.SelectedItems != null;
        }

        public static DateTime? StartDate(this IEnumerable<BinderNode> parameters, string attributeName)
        {
            DateTime? startDate = null;
            var dates = parameters.Where(e => e.Left == attributeName).ToArray();
            var date = dates.FirstOrDefault(e => e.OperatorKind == BinaryOperatorKind.GreaterThan);

            if (date == null)
            {
                date = dates.FirstOrDefault(e => e.OperatorKind == BinaryOperatorKind.GreaterThanOrEqual);
                if (date != null)
                {
                    startDate = DateTime.Parse(date.Right);
                }
            }
            else
            {
                startDate = DateTime.Parse(date.Right).AddSeconds(1);
            }
            return startDate;
        }

        public static string SingleOrDefault(this IEnumerable<BinderNode> parameters, string attributeName, Func<Exception> multipleValuesExceptionHandler)
        {
            var value = parameters.Where(e => e.Left == attributeName && e.OperatorKind == BinaryOperatorKind.Equal);
            var binderNodes = value as BinderNode[] ?? value.ToArray();
            if (binderNodes.Length > 1)
            {
                throw multipleValuesExceptionHandler();
            }
            return binderNodes.Length == 0 ? string.Empty : binderNodes.Single().Right;
        }


        public static int? Min(this IEnumerable<BinderNode> parameters, string attributeName)
        {
            var attribute = parameters.Where(e => e.Left == attributeName).ToArray();
            var value = attribute.FirstOrDefault(e => e.OperatorKind == BinaryOperatorKind.GreaterThanOrEqual);
            int min;

            if (value == null)
            {
                value = attribute.FirstOrDefault(e => e.OperatorKind == BinaryOperatorKind.GreaterThan);
                if (value != null)
                {
                    if (Int32.TryParse(value.Right, out min))
                        return min + 1;
                }
            }
            if (value == null)
            {
                value = attribute.FirstOrDefault(e => e.OperatorKind == BinaryOperatorKind.Equal);
                if (value != null)
                {
                    if (Int32.TryParse(value.Right, out min))
                        return min + 1;
                }
            }

            if (value != null && Int32.TryParse(value.Right, out min))
                return min;
            return null;
        }

        public static DateTime? EndDate(this IEnumerable<BinderNode> parameters, string attributeName)
        {
            DateTime? startDate = null;
            var dates = parameters.Where(e => e.Left == attributeName).ToArray();
            var date = dates.FirstOrDefault(e => e.OperatorKind == BinaryOperatorKind.LessThan);

            if (date == null)
            {
                date = dates.FirstOrDefault(e => e.OperatorKind == BinaryOperatorKind.LessThanOrEqual);
                if (date != null)
                {
                    startDate = DateTime.Parse(date.Right);
                }
            }
            else
            {
                startDate = DateTime.Parse(date.Right).AddSeconds(-1);
            }
            return startDate;
        }

        #region Private Methods


        private static ICollection<string> GetExpandedFields(IEnumerable<SelectItem> selectedItems)
        {
            var expandedFields = new List<string>();
            foreach (var expandedNavigationSelectItem in selectedItems.OfType<ExpandedNavigationSelectItem>())
            {
                if (expandedNavigationSelectItem.SelectAndExpand != null)
                {
                    expandedFields.AddRange(GetExpandedFields(expandedNavigationSelectItem.SelectAndExpand.SelectedItems));
                }
                expandedFields.AddRange(expandedNavigationSelectItem.PathToNavigationProperty.OfType<NavigationPropertySegment>().Select(navigationPropertySegment => navigationPropertySegment.NavigationProperty.Name));
            }
            return expandedFields;
        }

        #endregion

    }
}
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using ALS.Glance.Models.Core;
using ALS.Glance.Models.Core.Interfaces;

namespace ALS.Glance.Api
{
    public static class ModelBuilderExtensions
    {
        public static EntitySetConfiguration<TEntityType> EntitySet<TEntityType>(
         this ODataModelBuilder builder,  Action<EntityTypeConfiguration<TEntityType>> action)
         where TEntityType : class
        {
            var entitySet = builder.EntitySet<TEntityType>(typeof(TEntityType).Name);
            action(entitySet.EntityType);
            return entitySet;
        }
      
        public static EntitySetConfiguration<TEntityType> HasEditLinkForKey<TEntityType, TIdType>(
             this EntitySetConfiguration<TEntityType> entitySetConfiguration,
             bool followsConventions, Expression<Func<TEntityType, TIdType>> property)
             where TEntityType : class
        {
            if (property == null) throw new ArgumentNullException("property");

            var propertyResumedInfo = property.GetPropertyResumedInfo();

            entitySetConfiguration.HasEditLink(
                ctx =>
                {
                    object value;
                    ctx.EdmObject.TryGetPropertyValue(propertyResumedInfo.Item1, out value);
                    var path =
                        propertyResumedInfo.Item2
                            ? "'" + value + "'"
                            : value.ToString();

                    var oDataLink =
                        ctx.Url.CreateODataLink(
                            new EntitySetPathSegment(typeof(TEntityType).Name),
                            new KeyValuePathSegment(path));
                    return new Uri(oDataLink);
                }, followsConventions);

            return entitySetConfiguration;
        }

        public static EntitySetConfiguration<TEntityType> HasEditLinkForKey<TEntityType>(
            this EntitySetConfiguration<TEntityType> entitySetConfiguration,
            bool followsConventions,
            Expression<Func<TEntityType, object>> property01, Expression<Func<TEntityType, object>> property02,
            params Expression<Func<TEntityType, object>>[] properties)
            where TEntityType : class
        {
            if (property01 == null) throw new ArgumentNullException("property01");
            if (property02 == null) throw new ArgumentNullException("property02");

            var propertyList =
                new[]
                {
                    property01.GetPropertyResumedInfo(),
                    property02.GetPropertyResumedInfo()
                };
            if (properties != null && properties.Length > 0)
                propertyList = propertyList.Concat(
                    properties.Select(p => p.GetPropertyResumedInfo())).ToArray();

            entitySetConfiguration.HasEditLink(
                ctx =>
                {
                    object value;
                    ctx.EdmObject.TryGetPropertyValue(propertyList[0].Item1, out value);
                    var sb = new StringBuilder(
                        propertyList[0].Item2
                            ? propertyList[0].Item1 + "='" + value + "'"
                            : propertyList[0].Item1 + "=" + value);
                    for (var i = 1; i < propertyList.Length; i++)
                    {
                        ctx.EdmObject.TryGetPropertyValue(propertyList[i].Item1, out value);
                        sb.Append(
                            propertyList[i].Item2
                                ? "," + propertyList[i].Item1 + "='" + value + "'"
                                : "," + propertyList[i].Item1 + "=" + value);
                    }

                    var oDataLink =
                        ctx.Url.CreateODataLink(
                            new EntitySetPathSegment(typeof(TEntityType).Name),
                            new KeyValuePathSegment(sb.ToString()));
                    return new Uri(oDataLink);
                }, followsConventions);

            return entitySetConfiguration;
        }
        
        public static ComplexTypeConfiguration<TEntityType> ComplexType<TEntityType>(
       this ODataModelBuilder builder, Action<ComplexTypeConfiguration<TEntityType>> action)
       where TEntityType : class
        {
            var cfg = builder.ComplexType<TEntityType>();
            action(cfg);
            return cfg;
        }

        public static EntityTypeConfiguration<T> EntitySet<T, TKey>(this ODataModelBuilder builder, Action<EntityTypeConfiguration<T>> action)
            where T : class,IModel<TKey>
            where TKey : struct
        {
            var entitySet = builder.EntitySet<T, TKey>();
            action(entitySet);
            return entitySet;
        }

        public static EntityTypeConfiguration<T> EntitySet<T, TKey>(this ODataModelBuilder builder)
            where T : class,IModel<TKey>
            where TKey : struct
        {

            var type = builder.EntitySet<T>(typeof(T).Name).EntityType;
            type.HasKey(e => e.Id);
            return type;
        }

        #region Private Methods
        private static Tuple<string, bool> GetPropertyResumedInfo<TEntityType, TIdType>(
            this Expression<Func<TEntityType, TIdType>> propertyLambda)
        {
            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda));

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda));

            return new Tuple<string, bool>(
                propInfo.Name, propInfo.PropertyType == typeof(string));
        }

        #endregion
    }
}
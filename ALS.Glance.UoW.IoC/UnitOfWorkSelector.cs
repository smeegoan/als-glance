using System;
using System.Reflection;
using Castle.Facilities.TypedFactory;

namespace ALS.Glance.UoW.IoC
{
    public class UnitOfWorkSelector : DefaultTypedFactoryComponentSelector
    {
        protected override Type GetComponentType(MethodInfo method, object[] arguments)
        {
            return arguments.Length == 0 ? method.ReturnType : (arguments[0] as Type ?? method.ReturnType);
        }

        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            if (arguments.Length == 0) return method.ReturnType.Name;

            var type = arguments[0] as Type;
            return type == null ? method.ReturnType.Name : type.Name;
        }
    }
}
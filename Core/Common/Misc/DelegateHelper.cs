using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Hermit.Common
{
    public static class DelegateHelper
    {
        public static Delegate CreateInstance(object instance, MethodInfo method)
        {
            var parameters = method.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();

            var call = Expression.Call(Expression.Constant(instance), method, parameters);
            return Expression.Lambda(call, parameters).Compile();
        }
    }
}
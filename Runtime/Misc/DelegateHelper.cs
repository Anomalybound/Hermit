using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Hermit
{
    public static class DelegateHelper
    {
        #region Methods

        public static Delegate CreateDelegate(this MethodInfo methodInfo, object target)
        {
            Func<Type[], Type> getType;
            var isAction = methodInfo.ReturnType == typeof(void);
            var types = methodInfo.GetParameters().Select(p => p.ParameterType);
            if (isAction) { getType = Expression.GetActionType; }
            else
            {
                getType = Expression.GetFuncType;
                types = types.Concat(new[] {methodInfo.ReturnType});
            }

            return methodInfo.IsStatic
                ? Delegate.CreateDelegate(getType(types.ToArray()), methodInfo)
                : Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
        }

        #endregion

        #region Fields

        public static Func<object, object> CreateGetter(this FieldInfo field)
        {
            if (field.DeclaringType == null)
            {
                throw new Exception($"Declaring type is null, please checkout: {field}");
            }

            var typeExp = Expression.Parameter(typeof(object));
            var convertedTypeExp = Expression.Convert(typeExp, field.DeclaringType);

            var fieldExp = Expression.Field(convertedTypeExp, field);
            var convertedResultExp = Expression.Convert(fieldExp, typeof(object));

            return Expression.Lambda<Func<object, object>>(convertedResultExp, typeExp).Compile();
        }

        public static Action<object, object> CreateSetter(this FieldInfo field)
        {
            if (field.DeclaringType == null)
            {
                throw new Exception($"Declaring type is null, please checkout: {field}");
            }

            var typeExp = Expression.Parameter(typeof(object), "target");
            var valueExp = Expression.Parameter(typeof(object), "value");

            var convertedTypeExp = Expression.Convert(typeExp, field.DeclaringType);
            var convertedValueExp = Expression.Convert(valueExp, field.FieldType);

            var fieldExp = Expression.Field(convertedTypeExp, field);
            var assignExp = Expression.Assign(fieldExp, convertedValueExp);

            return Expression.Lambda<Action<object, object>>(assignExp, typeExp, valueExp).Compile();
        }

        public static Func<TType, TInstance> CreateGetter<TType, TInstance>(this FieldInfo field)
        {
            var typeExp = Expression.Parameter(typeof(TType));
            var fieldExp = Expression.Field(typeExp, field);
            return Expression.Lambda<Func<TType, TInstance>>(fieldExp, typeExp).Compile();
        }

        public static Action<TValue, TTarget> CreateSetter<TValue, TTarget>(this FieldInfo field)
        {
            var targetExp = Expression.Parameter(typeof(TTarget), "target");
            var valueExp = Expression.Parameter(typeof(TValue), "value");

            var fieldExp = Expression.Field(targetExp, field);
            var assignExp = Expression.Assign(fieldExp, valueExp);

            return Expression.Lambda<Action<TValue, TTarget>>(assignExp, targetExp, valueExp).Compile();
        }

        #endregion

        #region Proeprty

        public static Func<object, object> CreateGetter(this PropertyInfo property)
        {
            if (property.DeclaringType == null)
            {
                throw new Exception($"Declaring type is null, please checkout: {property}");
            }

            var getMethod = property.GetMethod;

            var typeExp = Expression.Parameter(typeof(object), "type");
            var convertedTypeExp = Expression.Convert(typeExp, property.DeclaringType);

            var callGetMethodExp = Expression.Call(convertedTypeExp, getMethod);
            var convertedResultExp = Expression.Convert(callGetMethodExp, typeof(object));

            return Expression.Lambda<Func<object, object>>(convertedResultExp, typeExp).Compile();
        }

        public static Action<object, object> CreateSetter(this PropertyInfo property)
        {
            if (property.DeclaringType == null)
            {
                throw new Exception($"Declaring type is null, please checkout: {property}");
            }

            var typeExp = Expression.Parameter(typeof(object), "target");
            var valueExp = Expression.Parameter(typeof(object), "value");

            var convertedTypeExp = Expression.Convert(typeExp, property.DeclaringType);
            var convertedValueExp = Expression.Convert(valueExp, property.PropertyType);

            var propExp = Expression.Property(convertedTypeExp, property);
            var assignExp = Expression.Assign(propExp, convertedValueExp);

            return Expression.Lambda<Action<object, object>>(assignExp, typeExp, valueExp).Compile();
        }

        public static Func<TType, TInstance> CreateGetter<TType, TInstance>(this PropertyInfo property)
        {
            var typeExp = Expression.Parameter(typeof(TType));
            var propertyExp = Expression.Property(typeExp, property);
            return Expression.Lambda<Func<TType, TInstance>>(propertyExp, typeExp).Compile();
        }

        public static Action<TValue, TTarget> CreateSetter<TValue, TTarget>(this PropertyInfo property)
        {
            var targetExp = Expression.Parameter(typeof(TTarget), "target");
            var valueExp = Expression.Parameter(typeof(TValue), "value");

            var propertyExp = Expression.Property(targetExp, property);
            var assignExp = Expression.Assign(propertyExp, valueExp);

            return Expression.Lambda<Action<TValue, TTarget>>(assignExp, targetExp, valueExp).Compile();
        }

        #endregion

        #region Helper Methods

        public static Delegate CreateInstance(object instance, MethodInfo method)
        {
            var parameters = method.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();

            var call = Expression.Call(Expression.Constant(instance), method, parameters);
            return Expression.Lambda(call, parameters).Compile();
        }

        #endregion
    }
}

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Hermit
{
    public static class DelegateExtensions
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
            string methodName = field.ReflectedType.FullName + ".get_" + field.Name;
            DynamicMethod setterMethod = new DynamicMethod(methodName, typeof(object), new[] {typeof(object)}, true);
            ILGenerator gen = setterMethod.GetILGenerator();
            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldsfld, field);
                gen.Emit(field.FieldType.IsClass ? OpCodes.Castclass : OpCodes.Box, field.FieldType);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Castclass, field.DeclaringType);
                gen.Emit(OpCodes.Ldfld, field);
                gen.Emit(field.FieldType.IsClass ? OpCodes.Castclass : OpCodes.Box, field.FieldType);
            }

            gen.Emit(OpCodes.Ret);
            return (Func<object, object>) setterMethod.CreateDelegate(typeof(Func<object, object>));
        }

        public static Action<object, object> CreateSetter(this FieldInfo field)
        {
            string methodName = field.ReflectedType.FullName + ".set_" + field.Name;
            DynamicMethod setterMethod =
                new DynamicMethod(methodName, null, new[] {typeof(object), typeof(object)}, true);
            ILGenerator gen = setterMethod.GetILGenerator();
            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(field.FieldType.IsClass ? OpCodes.Castclass : OpCodes.Unbox_Any, field.FieldType);
                gen.Emit(OpCodes.Stsfld, field);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Castclass, field.DeclaringType);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(field.FieldType.IsClass ? OpCodes.Castclass : OpCodes.Unbox_Any, field.FieldType);
                gen.Emit(OpCodes.Stfld, field);
            }

            gen.Emit(OpCodes.Ret);
            return (Action<object, object>) setterMethod.CreateDelegate(typeof(Action<object, object>));
        }

        public static Func<S, T> CreateGetter<S, T>(this FieldInfo field)
        {
            var methodName = field.ReflectedType.FullName + ".get_" + field.Name;
            var setterMethod = new DynamicMethod(methodName, typeof(T), new[] {typeof(S)}, true);
            var gen = setterMethod.GetILGenerator();
            if (field.IsStatic) { gen.Emit(OpCodes.Ldsfld, field); }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
            }

            gen.Emit(OpCodes.Ret);
            return (Func<S, T>) setterMethod.CreateDelegate(typeof(Func<S, T>));
        }

        public static Action<S, T> CreateSetter<S, T>(this FieldInfo field)
        {
            var methodName = field.ReflectedType.FullName + ".set_" + field.Name;
            var setterMethod = new DynamicMethod(methodName, null, new[] {typeof(S), typeof(T)}, true);
            var gen = setterMethod.GetILGenerator();
            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stsfld, field);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stfld, field);
            }

            gen.Emit(OpCodes.Ret);
            return (Action<S, T>) setterMethod.CreateDelegate(typeof(Action<S, T>));
        }

        #endregion

        #region Proeprty

        public static Func<object, object> CreateGetter(this PropertyInfo propertyInfo)
        {
            var type = propertyInfo.DeclaringType;
            var propGetMethod = propertyInfo.GetGetMethod(nonPublic: true);
            var propType = propertyInfo.PropertyType;

            var dynamicMethod = new DynamicMethod("m", typeof(object), new[] {typeof(object)}, type.Module);

            var iLGenerator = dynamicMethod.GetILGenerator();

            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Castclass, type);
            iLGenerator.Emit(OpCodes.Call, propGetMethod);
            iLGenerator.Emit(OpCodes.Box, propType);
            iLGenerator.Emit(OpCodes.Ret);

            return (Func<object, object>) dynamicMethod.CreateDelegate(typeof(Func<object, object>));
        }

        public static Action<object, object> CreateSetter(this PropertyInfo propertyInfo)
        {
            var type = propertyInfo.DeclaringType;
            var propSetMethod = propertyInfo.GetSetMethod(nonPublic: true);
            var propType = propertyInfo.PropertyType;

            var dynamicMethod =
                new DynamicMethod("m", typeof(object), new[] {typeof(object), typeof(object)}, type.Module);

            var iLGenerator = dynamicMethod.GetILGenerator();
            var local0 = iLGenerator.DeclareLocal(propType);

            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Castclass, type);
            iLGenerator.Emit(OpCodes.Call, propSetMethod);
            iLGenerator.Emit(OpCodes.Box, propType);
            iLGenerator.Emit(OpCodes.Ret );

            return (Action<object, object>) dynamicMethod.CreateDelegate(typeof(Action<object, object>));
        }

        public static Func<S, T> CreateGetter<S, T>(this PropertyInfo propertyInfo)
        {
            return (Func<S, T>) Delegate.CreateDelegate(typeof(S), typeof(T), propertyInfo.GetMethod);
        }

        public static Action<S, T> CreateSetter<S, T>(this PropertyInfo propertyInfo)
        {
            return (Action<S, T>) Delegate.CreateDelegate(typeof(S), typeof(T), propertyInfo.SetMethod);
        }

        #endregion
    }
}
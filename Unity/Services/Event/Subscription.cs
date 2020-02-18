using System;
using System.Reflection;

namespace Hermit
{
    public class Subscription : IComparable<Subscription>
    {
        public string Endpoint { get; }

        public bool IsSticky { get; }

        public short Priority { get; }

        public ThreadMode ThreadMode { get; }

        public Type EventDataType { get; }

        public string MethodName { get; }

        public Delegate MethodInvoker { get; }

        public Subscription(string endpoint, bool isSticky, short priority, ThreadMode threadMode,
            MethodInfo methodInfo, object target)
        {
            Endpoint = endpoint;
            IsSticky = isSticky;
            Priority = priority;
            ThreadMode = threadMode;

            MethodName = methodInfo.Name;

            var parameters = methodInfo.GetParameters();
            if (parameters.Length <= 0) { MethodInvoker = methodInfo.CreateDelegate(target); }
            else
            {
                EventDataType = methodInfo.GetParameters()[0].ParameterType;
                MethodInvoker = methodInfo.CreateDelegate(target);
            }
        }

        public int CompareTo(Subscription other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return other?.Priority.CompareTo(Priority) ?? 1;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.DI
{
    internal class TypeComparer : IEqualityComparer<Type>
    {
        public static readonly TypeComparer Instance = new TypeComparer();
        private static readonly Type _runtimeType = typeof(int).GetType();
        public bool Equals(Type x, Type y)
        {
            if (x == null || y == null)
            {
                return x.Equals(y);
            }
            if ((x.GetType() != _runtimeType && x.IsGenericType) || (y.GetType() != _runtimeType && y.IsGenericType))
            {
                if (!Equals(x.GetGenericTypeDefinition(), y.GetGenericTypeDefinition()))
                {
                    return false;
                }
                if (x.IsGenericTypeDefinition || y.IsGenericTypeDefinition)
                {
                    return x.IsGenericTypeDefinition == y.IsGenericTypeDefinition;
                }
                var args1 = x.GetGenericArguments();
                var args2 = y.GetGenericArguments();
                if (args1.Length != args2.Length)
                {
                    return false;
                }
                for (var i = 0; i < args1.Length; i++)
                {
                    if (!Equals(args1[i], args2[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return x.Equals(y);
        }

        public int GetHashCode(Type obj)
        {
            if (obj != null && obj.GetType() != _runtimeType && obj.IsGenericType)
            {
                var hashcode = obj.GetGenericTypeDefinition().GetHashCode();
                if (!obj.IsGenericTypeDefinition)
                {
                    foreach (var item in obj.GetGenericArguments())
                    {
                        hashcode ^= item.GetHashCode();
                    }
                }
                return hashcode;
            }
            return obj?.GetHashCode() ?? 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.DI
{
    internal class TypeComparer : IEqualityComparer<Type>
    {
        public static readonly TypeComparer Instance = new TypeComparer();
        public bool Equals(Type x, Type y)
        {
            if (x?.GetType().Name == "TypeBuilderInstantiation" && y?.GetType().Name == "TypeBuilderInstantiation")
            {
                if (!Equals(x.GetGenericTypeDefinition(), y.GetGenericTypeDefinition()))
                {
                    return false;
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
            return x != null && y != null && x.Equals(y);
        }

        public int GetHashCode(Type obj)
        {
            if (obj?.GetType().Name == "TypeBuilderInstantiation")
            {
                var hashcode = obj.GetGenericTypeDefinition().GetHashCode();
                foreach (var item in obj.GetGenericArguments())
                {
                    hashcode ^= item.GetHashCode();
                }
                return hashcode;
            }
            return obj.GetHashCode();
        }
    }
}

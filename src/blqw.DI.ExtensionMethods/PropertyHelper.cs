using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace blqw.DI
{
    static class PropertyHelper
    {
        static readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> _setters = new ConcurrentDictionary<PropertyInfo, Action<object, object>>();

        interface ISetter { void Set(object instance, object value); }

        class Setter<TInstance, TValue> : ISetter
        {
            private Action<TInstance, TValue> _handle;
            public Setter(MethodInfo setMethod) => _handle = (Action<TInstance, TValue>)setMethod.CreateDelegate(typeof(Action<TInstance, TValue>));
            public void Set(object instance, object value) => _handle((TInstance)instance, (TValue)value);
        }

        static Action<object, object> CreateSetter(PropertyInfo property)
        {
            var method = property.GetSetMethod(true);
            if (method == null)
            {
                if (property.DeclaringType.GetField($"<{property.Name}>k__BackingField", (BindingFlags)(-1)) is FieldInfo field)
                {
                    return field.SetValue;
                }
                return null;
            }
            return ((ISetter)Activator.CreateInstance(typeof(Setter<,>).MakeGenericType(property.ReflectedType, property.PropertyType), method)).Set;
        }

        public static void Set(this PropertyInfo property, object instance, object value) => _setters.GetOrAdd(property, CreateSetter)?.Invoke(instance, value);

    }
}

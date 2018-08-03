using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 命名类型
    /// </summary>
    internal sealed class NamedType : TypeDelegator, IServiceTypePretender, IServiceProvider
    {
        private static readonly ConcurrentDictionary<(string, Type), NamedType> _cache = new ConcurrentDictionary<(string, Type), NamedType>();

        public static NamedType Get(string name, Type serviceType = null) =>
            _cache.GetOrAdd((name, serviceType), x => new NamedType(x.Item1, x.Item2));

        private NamedType(string name, Type serviceType)
            : base(typeof(object))
        {
            Name = name;
            using (var md5Provider = new MD5CryptoServiceProvider())
            {
                var bytes = Encoding.UTF8.GetBytes(name);
                var hash = md5Provider.ComputeHash(bytes);
                GUID = new Guid(hash);
            }
            ServiceType = serviceType;
        }

        public Type ServiceType { get; }
        public override string Name { get; }
        public override Guid GUID { get; }
        public override bool Equals(object obj) => Equals(obj as NamedType);
        public override int GetHashCode() => Name.GetHashCode();
        public override bool Equals(Type o) => o is NamedType t && t.Name == Name;
        public override string FullName => $"NamedTypes.{Name}";

        public object GetService(Type serviceType) => serviceType == typeof(Type) ? ServiceType : null;
    }
}

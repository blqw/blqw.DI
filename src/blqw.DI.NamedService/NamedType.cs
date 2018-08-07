using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace blqw.DI
{
    /// <summary>
    /// 命名类型
    /// </summary>
    public sealed class NamedType : TypeDelegator, IServiceProvider
    {
        /// <summary>
        /// 类型缓存
        /// </summary>
        private static readonly ConcurrentDictionary<(string, Type), NamedType> _cache = new ConcurrentDictionary<(string, Type), NamedType>();
        /// <summary>
        /// 获取指定名称的服务
        /// </summary>
        /// <param name="name">服务名称</param>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        public static NamedType Get(string name, Type serviceType = null) =>
            _cache.GetOrAdd((name, serviceType), x => new NamedType(x.Item1, x.Item2));
        /// <summary>
        /// 私有构造函数
        /// </summary>
        /// <param name="name">类型名称</param>
        /// <param name="serviceType">服务类型</param>
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
        /// <summary>
        /// 实际服务类型
        /// </summary>
        public Type ServiceType { get; }
        /// <inherit />
        public override string Name { get; }
        /// <inherit />
        public override Guid GUID { get; }
        /// <inherit />
        public override bool Equals(object obj) => Equals(obj as NamedType);
        /// <inherit />
        public override int GetHashCode() => Name.GetHashCode();
        /// <inherit />
        public override bool Equals(Type o) => o is NamedType t && t.Name == Name;
        /// <inherit />
        public override string FullName => $"NamedTypes.{Name}";
        /// <inherit />
        public object GetService(Type serviceType) => serviceType == typeof(Type) ? ServiceType : null;
    }
}

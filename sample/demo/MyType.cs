using Microsoft.Extensions.Logging.Abstractions.Internal;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace demo
{
    /// <summary>
    /// 命名类型
    /// </summary>
    internal sealed class MyType : TypeDelegator, IServiceProvider
    {
        /// <summary>
        /// 类型缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, MyType> _cache = new ConcurrentDictionary<Type, MyType>();
        /// <summary>
        /// 获取指定名称的服务
        /// </summary>
        /// <param name="name">服务名称</param>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        public static MyType Get(Type serviceType) =>
            _cache.GetOrAdd(serviceType, x => new MyType(x));
        /// <summary>
        /// 私有构造函数
        /// </summary>
        /// <param name="name">类型名称</param>
        /// <param name="serviceType">服务类型</param>
        private MyType(Type serviceType)
            : base(typeof(object))
        {
            Name = serviceType.Name;
            using (var md5Provider = new MD5CryptoServiceProvider())
            {
                var bytes = Encoding.UTF8.GetBytes(Name);
                var hash = md5Provider.ComputeHash(bytes);
                GUID = new Guid(hash);
            }
            ServiceType = serviceType;
            GenericTypeDefinition = serviceType.IsGenericTypeDefinition ? serviceType : serviceType.GetGenericTypeDefinition();
        }
        /// <summary>
        /// 实际服务类型
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        ///
        /// </summary>
        public Type GenericTypeDefinition { get; }
        /// <inherit />
        public override string Name { get; }
        /// <inherit />
        public override Guid GUID { get; }
        /// <inherit />
        public override bool Equals(object obj) => Equals(obj as Type);
        /// <inherit />
        public override int GetHashCode() => GenericTypeDefinition.GetHashCode();
        /// <inherit />
        public override bool Equals(Type o)
        {
            if (o is MyType t)
            {
                return t.GenericTypeDefinition == GenericTypeDefinition;
            }

            if (o.IsGenericTypeDefinition)
            {
                return o == GenericTypeDefinition;
            }
            else if (o.IsGenericType)
            {
                return o.GetGenericTypeDefinition() == GenericTypeDefinition;
            }
            return false;
        }

        /// <inherit />
        public override string FullName => $"MyTypes.{Name}";
        /// <inherit />
        public object GetService(Type serviceType) => serviceType == typeof(Type) ? ServiceType : null;
    }
}

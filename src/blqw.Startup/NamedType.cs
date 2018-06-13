using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 命名类型
    /// </summary>
    internal sealed class NamedType : TypeDelegator, IReflectableType
    {
        private readonly string _name;
        private readonly Guid _guid;
        public NamedType(string name) : this(name, null) { }
        public NamedType(string name, Type delegatingType)
            : base(delegatingType ?? typeof(object))
        {
            _name = name;
            using (var md5Provider = new MD5CryptoServiceProvider())
            {
                var bytes = Encoding.UTF8.GetBytes(name);
                var hash = md5Provider.ComputeHash(bytes);
                _guid = new Guid(hash);
            }
        }
        public Type ExportType { get; set; }
        public override Guid GUID => _guid;
        public override bool Equals(object obj) => Equals(obj as NamedType);
        public override int GetHashCode() => _name.GetHashCode();
        public override bool Equals(Type o) => o is NamedType t && t._name == _name;
        public override string Name => _name;
        public override string FullName => $"NamedTypes.{_name}";
        public TypeInfo GetTypeInfo() => BaseType?.GetTypeInfo() ?? new TypeDelegator(this);
    }
}

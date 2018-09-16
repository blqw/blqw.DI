using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace blqw.DI
{
    /// <summary>
    /// 服务提供程序上下文
    /// </summary>
    public static class ServiceContext
    {

        /// <summary>
        /// 根节点 <see cref="IServiceProvider"/>
        /// </summary>
        public static IServiceProvider RootProvider => ProviderImpl?.Root;

        /// <summary>
        /// 上下文路径
        /// </summary>
        public static string Path => ProviderImpl?.ToString();

        /// <summary>
        /// 当前 <see cref="IServiceProvider"/>
        /// </summary>
        public static IServiceProvider Provider => _local.Value?.Provider;

        /// <summary>
        /// 当前上下文 <see cref="IServiceProvider"/> 的id
        /// </summary>
        public static long Id => ProviderImpl?.Id ?? -1;

        /// <summary>
        /// 用于获取上下文中的指定 <see cref="IServiceProvider"/> 的Id
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static long GetId(this IServiceProvider provider) => ((SupportContextServiceProvider)provider)?.Id ?? -1;


        private readonly static AsyncLocal<ServiceProviderAccessor> _local = new AsyncLocal<ServiceProviderAccessor>(LocalValueChanged);

        private static void LocalValueChanged(AsyncLocalValueChangedArgs<ServiceProviderAccessor> obj)
        {
            if (obj.ThreadContextChanged)
            {
                var prev = obj.PreviousValue?.Provider;
                var curr = obj.CurrentValue?.Provider;
                if (curr == null || prev?.IsMyParent(curr) == true)
                {
                    ProviderImpl = prev;
                }
                else
                {
                    ProviderImpl = curr;
                }
            }
        }

        private static SupportContextServiceProvider ProviderImpl
        {
            get => _local.Value?.Provider;
            set
            {
                var accessor = value.Accessor;
                if (!ReferenceEquals(accessor, _local.Value))
                {
                    _local.Value = value.Accessor;
                }
            }
        }


        internal static bool Push(SupportContextServiceProvider provider)
        {
            if (provider == null)
            {
                return false;
            }
            var current = ProviderImpl;
            if (current == null || provider.IsMyParent(current))
            {
                ProviderImpl = provider;
                return true;
            }
            return false;
        }

        internal static bool PopTo(SupportContextServiceProvider provider)
        {
            provider = provider.Accessor.Provider;
            if (provider == null)
            {
                return false;
            }
            ProviderImpl = provider;
            return true;
        }
    }
}

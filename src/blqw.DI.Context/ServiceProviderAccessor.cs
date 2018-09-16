using System.Diagnostics;

namespace blqw.DI
{
    [DebuggerDisplay("{DebugText}")]
    class ServiceProviderAccessor
    {
        public ServiceProviderAccessor(SupportContextServiceProvider provider)
        {
            _provider = provider;
            _id = provider?.Id ?? -1;
        }

        private SupportContextServiceProvider _provider;
        private readonly long _id;

        internal SupportContextServiceProvider Provider
        {
            get
            {
                var current = _provider;
                while (current?.IsDisposed == true)
                {
                    _provider = current = current.Parent;
                }
                return current;
            }
        }

        private string DebugText =>
            $"Id: {_id}, Provider: {_provider}{(_provider?.IsDisposed == true ? " (disposed)" : "")}";
    }
}

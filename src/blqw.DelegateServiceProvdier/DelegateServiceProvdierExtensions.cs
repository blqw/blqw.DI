using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace blqw
{
    public static class DelegateServiceProvdierExtensions
    {
        public static IServiceProvider BuildSupportDelegateServiceProvdier(this IServiceCollection services)
        {
            var methodServices = new List<ServiceDescriptor>();
            foreach (var item in services)
            {
                //针对委托类型的服务, 新增一个MethodInfo类型的服务
                if (item.ImplementationInstance is Delegate && item.ServiceType != typeof(MethodInfo))
                {
                    methodServices.Add(new ServiceDescriptor(typeof(MethodInfo), ((Delegate)item.ImplementationInstance).Method));
                }
                else if (item.ImplementationFactory != null && item.ServiceType != typeof(MethodInfo))
                {
                    if (typeof(Delegate).IsAssignableFrom(item.ImplementationFactory.Method.ReturnType))
                    {
                        methodServices.Add(new ServiceDescriptor(typeof(MethodInfo), p => ((Delegate)item.ImplementationFactory(p)).Method, item.Lifetime));
                    }
                    else if (FactoryIsDelegateService(item.ImplementationFactory.Method))
                    {
                        methodServices.Add(new ServiceDescriptor(typeof(MethodInfo), item.ImplementationFactory.Method));
                    }
                }
            }
            methodServices.ForEach(services.Add);
            var provider = services.BuildServiceProvider();
            return new DelegateServiceProvdier(provider);
        }

        private static bool FactoryIsDelegateService(MethodInfo factoryMethod)
        {
            if (factoryMethod == null)
            {
                return false;
            }
            if (factoryMethod.ReturnType != typeof(object))
            {
                return true;
            }
            var paras = factoryMethod.GetParameters();
            if (paras.Length != 1 || paras[0].ParameterType != typeof(IServiceProvider))
            {
                return true;
            }
            return false;
        }
    }
}

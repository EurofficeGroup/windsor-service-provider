/*
 * Copyright 2020 Lukas Tines <ltines@euroffice.co.uk>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;

namespace WindsorServiceProvider
{
    internal static class ServiceCollectionExtensions
    {
        public static IWindsorContainer CreateContainer(this IServiceCollection serviceCollection, WindsorServiceProviderFactory factory)
        {
            var container = new WindsorContainer();
            if(serviceCollection == null)
                return container;

            container.Register(
                    Component
                        .For<IWindsorContainer>()
                        .Instance(container),
                    Component
                        .For<IServiceProvider, ISupportRequiredService>()
                        .ImplementedBy<WindsorScopedServiceProvider>()
                        .LifeStyle.ScopedToNetCoreScope(),
                    Component
                        .For<IServiceScopeFactory>()
                        .ImplementedBy<WindsorScopeFactory>()
                        .LifestyleSingleton(),
                    Component
                        .For<WindsorServiceProviderFactory>()
                        .Instance(factory)
                        .LifestyleSingleton()
            );

            //From https://github.com/volosoft/castle-windsor-ms-adapter/blob/master/src/Castle.Windsor.MsDependencyInjection/WindsorRegistrationHelper.cs
            container.Kernel.Resolver.AddSubResolver(new NetCoreCollectionResolver(container.Kernel));
            container.Kernel.Resolver.AddSubResolver(new OptionsSubResolver(container.Kernel));
            
            container.Kernel.Resolver.AddSubResolver(new LoggerDependencyResolver(container.Kernel));

            foreach(var service in serviceCollection)
            {
                container.Register(service.CreateWindsorRegistration());
            }

            return container;
        }

        
        public static IRegistration CreateWindsorRegistration(this Microsoft.Extensions.DependencyInjection.ServiceDescriptor service)
        {
            if(service.ServiceType.ContainsGenericParameters)
            {
                return RegistrationAdapter.FromOpenGenericServiceDescriptor(service);
            }
            else
            {
                var method = typeof(RegistrationAdapter).GetMethod("FromServiceDescriptor", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(service.ServiceType);
                return method.Invoke(null, new object[] {service}) as IRegistration;
            }
        }
    }
}
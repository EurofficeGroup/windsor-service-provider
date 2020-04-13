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
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace WindsorServiceProvider
{
    internal class RegistrationAdapter
    {
        public static IRegistration FromOpenGenericServiceDescriptor(Microsoft.Extensions.DependencyInjection.ServiceDescriptor service)
        {
            ComponentRegistration<object> registration = Component.For(service.ServiceType)
                .NamedAutomatically(UniqueComponentName(service));

            if(service.ImplementationType != null)
            {
                registration = UsingImplementation(registration, service);
            }
            else
            {
                throw new System.ArgumentException("Unsupported ServiceDescriptor");
            }

            return ResolveLifestyle(registration, service)
                .IsDefault();
        }

        public static IRegistration FromServiceDescriptor<TService>(Microsoft.Extensions.DependencyInjection.ServiceDescriptor service) where TService : class
        {
            var registration = Component.For<TService>()
                .NamedAutomatically(UniqueComponentName(service));

            if(service.ImplementationFactory != null)
            {
                registration = UsingFactoryMethod<TService>(registration, service);
            }
            else if(service.ImplementationInstance != null)
            {
                registration = UsingInstance<TService>(registration, service);
            }
            else if(service.ImplementationType != null)
            {
                registration = UsingImplementation<TService>(registration, service);
            }

            return ResolveLifestyle<TService>(registration, service)
                .IsDefault();
        }

        private static string UniqueComponentName(Microsoft.Extensions.DependencyInjection.ServiceDescriptor service)
        {
            return 
                (service.ImplementationType?.FullName ??
                    service.ImplementationInstance?.GetType().FullName ??
                    service.ImplementationFactory.GetType().FullName
                ) + "@" + Guid.NewGuid().ToString();
        }
        public static string OriginalComponentName(string uniqueComponentName)
        {
            if(uniqueComponentName == null)
                return null;
            if(!uniqueComponentName.Contains("@"))
                return uniqueComponentName;
            return uniqueComponentName.Split('@')[0];
        }

        private static ComponentRegistration<TService> UsingFactoryMethod<TService>(ComponentRegistration<TService> registration, Microsoft.Extensions.DependencyInjection.ServiceDescriptor service) where TService : class
        {
            return registration.UsingFactoryMethod((kernel) => {
                var serviceProvider = kernel.Resolve<System.IServiceProvider>();
                return service.ImplementationFactory(serviceProvider) as TService;
            });
        }

        private static ComponentRegistration<TService> UsingInstance<TService>(ComponentRegistration<TService> registration, Microsoft.Extensions.DependencyInjection.ServiceDescriptor service) where TService : class
        {
            return registration.Instance(service.ImplementationInstance as TService);
        }

        private static ComponentRegistration<TService> UsingImplementation<TService>(ComponentRegistration<TService> registration, Microsoft.Extensions.DependencyInjection.ServiceDescriptor service) where TService : class
        {
            return registration.ImplementedBy(service.ImplementationType);
        }

        private static ComponentRegistration<TService> ResolveLifestyle<TService>(ComponentRegistration<TService> registration, Microsoft.Extensions.DependencyInjection.ServiceDescriptor service) where TService : class
        {
            switch(service.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    return registration.LifeStyle.Singleton;
                case ServiceLifetime.Scoped:
                    return registration.LifeStyle.ScopedToNetCoreScope();
                case ServiceLifetime.Transient:
                    return registration
                    .Attribute(NetCoreScope.NetCoreTransientMarker).Eq(Boolean.TrueString)
                    .LifeStyle.ScopedToNetCoreScope();  //.NET core expects new instances but release on scope dispose
                default:
                    throw new System.ArgumentException($"Invalid lifetime {service.Lifetime}");
            }
        }
    }
}
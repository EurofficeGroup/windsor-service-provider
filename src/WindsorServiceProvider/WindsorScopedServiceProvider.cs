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
using System.Collections.Generic;
using System.Reflection;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;

namespace WindsorServiceProvider
{
    internal class WindsorScopedServiceProvider : IServiceProvider, ISupportRequiredService
    {
        internal IWindsorContainer Container {get; private set;}

        public WindsorScopedServiceProvider(IWindsorContainer container)
        {
            Container = container;
        }

        public object GetService(Type serviceType)
        {
            return ResolveInstanceOrNull(serviceType, true);
        }

        public object GetRequiredService(Type serviceType)
        {
            return ResolveInstanceOrNull(serviceType, false);
        }

        private object ResolveInstanceOrNull(Type serviceType, bool isOptional)
        {
            if (Container.Kernel.HasComponent(serviceType))
            {
                return Container.Resolve(serviceType);
            }

            if (serviceType.GetTypeInfo().IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var allObjects = Container.ResolveAll(serviceType.GenericTypeArguments[0]);
                Array.Reverse(allObjects);
                return allObjects;
            }

            if (isOptional)
            {
                return null;
            }

            return Container.Resolve(serviceType);
        }
    }
}
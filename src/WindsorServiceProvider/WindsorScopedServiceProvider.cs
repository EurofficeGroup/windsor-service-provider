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
    internal class WindsorScopedServiceProvider : IServiceProvider, ISupportRequiredService, IDisposable
    {
        internal IWindsorContainer Container {get; private set;}
        private readonly NetCoreScope _scope;

        public WindsorScopedServiceProvider(IWindsorContainer container)
        {
            Container = container;
            _scope = NetCoreScope.Current;
        }

        public object GetService(Type serviceType)
        {
            using(var fs = new NetCoreScope.ForcedScope(_scope))
            {
                return ResolveInstanceOrNull(serviceType, true);    
            }
        }

        public object GetRequiredService(Type serviceType)
        {
            using(var fs = new NetCoreScope.ForcedScope(_scope))
            {
                return ResolveInstanceOrNull(serviceType, false);    
            }
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
        private bool _disposing = false;
        public void Dispose()
        {
            if(_scope is NetCoreRootScope)
            {
                if(!_disposing)
                {
                    _disposing = true;
                    var scope = _scope as IDisposable;
                    if(scope != null)
                    {
                        scope.Dispose();
                    }
                    Container.Dispose();
                }
                
            }
        }
    }
}
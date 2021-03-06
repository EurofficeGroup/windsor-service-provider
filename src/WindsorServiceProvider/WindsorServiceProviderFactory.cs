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
using Castle.MicroKernel;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;

namespace WindsorServiceProvider
{
    internal class WindsorServiceProviderFactory : IServiceProviderFactory<IWindsorContainer>, IDisposable
    {
        public WindsorServiceProviderFactory()
        {
            
        }
        public IWindsorContainer CreateBuilder(IServiceCollection services)
        {
            var container = services.CreateContainer(this);
            return container;
        }

        public IServiceProvider CreateServiceProvider(IWindsorContainer container)
        {
            var rootScope = NetCoreRootScope.BeginRootScope();
            return container.Resolve<IServiceProvider>();
        }

        public void Dispose()
        {
            
        }
    }
}
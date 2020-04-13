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

using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;

namespace WindsorServiceProvider
{
    internal class WindsorScopeFactory : IServiceScopeFactory
    {
        private readonly WindsorServiceProviderFactory _serviceProviderFactory;
        private readonly IWindsorContainer _container;

        public WindsorScopeFactory(
            WindsorServiceProviderFactory serviceProviderFactory,
            IWindsorContainer container)
        {
            _serviceProviderFactory = serviceProviderFactory;
            _container = container;
        }

        public IServiceScope CreateScope()
        {
            var scope = NetCoreScope.BeginScope(NetCoreScope.Current);
            //since WindsorServiceProvider is scoped, this gives us new instance
            var provider = _serviceProviderFactory.CreateServiceProvider(_container);

            return new NetCoreServiceScope(scope, provider);
        }
    }
}
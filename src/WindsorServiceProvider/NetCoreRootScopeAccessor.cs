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
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace WindsorServiceProvider
{
    internal class NetCoreRootScopeAccessor : IScopeAccessor
    {
        public ILifetimeScope GetScope(CreationContext context)
        {
            if(NetCoreScope.Current == null)
            {
                throw new InvalidOperationException("No scope");
            }

            if(NetCoreRootScope.Current.RootScope == null)
            {
                throw new InvalidOperationException("No root scope");
            }

            return NetCoreRootScope.Current.RootScope;     
        }

        public void Dispose()
        {
        }
    }
}
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
using System.Threading;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace WindsorServiceProvider
{
    //heavily based on https://github.com/castleproject/Windsor/blob/master/src/Castle.Windsor/MicroKernel/Lifestyle/Scoped/DefaultLifetimeScope.cs
    internal class NetCoreScope : ILifetimeScope, IDisposable
    {
        private static readonly AsyncLocal<NetCoreScope> _current = new AsyncLocal<NetCoreScope>();
        public static NetCoreScope Current => _current.Value;


        private readonly NetCoreScope _parent;
        private static readonly Action<Burden> emptyOnAfterCreated = delegate { };
		private readonly Action<Burden> _onAfterCreated;
		private readonly IScopeCache _scopeCache;

        private NetCoreScope(NetCoreScope parent)
        {
            _parent = parent;
            _scopeCache = new ScopeCache();
            _onAfterCreated = emptyOnAfterCreated;
        }
        public static NetCoreScope BeginScope(NetCoreScope parent)
        {
            var scope = new NetCoreScope(parent);
            _current.Value = scope;
            return scope;
        }

        public void Dispose()
		{
			var disposableCache = _scopeCache as IDisposable;
			if (disposableCache != null)
			{
				disposableCache.Dispose();
			}

            _current.Value = _parent;
		}

        public Burden GetCachedInstance(ComponentModel model, ScopedInstanceActivationCallback createInstance)
        {
            var burden = _scopeCache[model];
            if (burden == null)
            {
                _scopeCache[model] = burden = createInstance(_onAfterCreated);
            }
            return burden;
        }
    }
}
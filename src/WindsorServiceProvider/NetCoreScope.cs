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
        public static string NetCoreTransientMarker = "NetCoreTransient";

        private readonly NetCoreScope _parent;
        private static readonly Action<Burden> emptyOnAfterCreated = delegate { };
		private readonly IScopeCache _scopeCache;
        public bool RootScope {get;private set;}
        public int Nesting {get; private set;}

        private NetCoreScope(NetCoreScope parent, bool rootScope)
        {
            _parent = parent;
            _scopeCache = new ScopeCache();
            RootScope = rootScope;
            Nesting = (parent?.Nesting ?? 0) + 1;
        }
        public static NetCoreScope BeginScope(NetCoreScope parent)
        {
            var scope = new NetCoreScope(parent, false);
            _current.Value = scope;
            return scope;
        }

        public static NetCoreScope BeginRootScope()
        {
            var scope = new NetCoreScope(null, true);
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
            if(model.Configuration.Attributes.Get(NetCoreTransientMarker) == Boolean.TrueString ){
                var burder = createInstance(emptyOnAfterCreated);
                _scopeCache[burder] = burder;
                return burder;
            }
            else
            {
                var burden = _scopeCache[model];
                if (burden == null)
                {
                    _scopeCache[model] = burden = createInstance(emptyOnAfterCreated);
                }
                return burden;
            }
        }

        internal class ForcedScope : IDisposable
        {
            private readonly NetCoreScope _previousScope;
            public ForcedScope(NetCoreScope scope)
            {
                _previousScope = NetCoreScope.Current;
                NetCoreScope._current.Value = scope;
            }
            public void Dispose()
            {
                NetCoreScope._current.Value = _previousScope;
            }
        }
    }
}
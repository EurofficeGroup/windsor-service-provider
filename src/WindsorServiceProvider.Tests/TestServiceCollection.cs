using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace WindsorServiceProvider.Tests
{
    internal class TestServiceCollection : List<ServiceDescriptor>, IServiceCollection
    {
    }
}
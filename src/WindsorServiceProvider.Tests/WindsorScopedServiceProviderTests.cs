using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification;
using Microsoft.Extensions.DependencyInjection.Specification.Fakes;
using Xunit;

namespace WindsorServiceProvider.Tests
{
    public class WindsorScopedServiceProviderTests : DependencyInjectionSpecificationTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            var factory = new WindsorServiceProviderFactory();
            var container = factory.CreateBuilder(serviceCollection);
            return factory.CreateServiceProvider(container);
        }

        [Fact]
        public void NestedScopedServiceCanBeResolved2()
        {
            // Arrange
            var collection = new TestServiceCollection();
            collection.AddScoped<IFakeScopedService, FakeService>();
            var provider = CreateServiceProvider(collection);

            // Act
            using (var outerScope = provider.CreateScope())
            using (var innerScope = outerScope.ServiceProvider.CreateScope())
            {
                var outerScopedService = outerScope.ServiceProvider.GetService<IFakeScopedService>();
                var innerScopedService = innerScope.ServiceProvider.GetService<IFakeScopedService>();

                // Assert
                Assert.NotNull(outerScopedService);
                Assert.NotNull(innerScopedService);
                Assert.NotSame(outerScopedService, innerScopedService);
            }
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FluffySpoon.Extensions.MicrosoftDependencyInjection.Tests
{
    [TestClass]
    public class RegistrationTest
    {
        [TestMethod]
        public void CanRegisterGenericTypes()
        {
            var services = new ServiceCollection();
            services.AddAssemblyTypesAsImplementedInterfaces(new RegistrationSettings() {
                Assemblies = new [] {
                    typeof(RegistrationTest).Assembly
                }
            });

            var container = services.BuildServiceProvider();

            var instance = container.GetRequiredService<IGeneric<string>>();
            Assert.IsInstanceOfType(instance, typeof(Generic<string>));
        }

        [TestMethod]
        public void CanRegisterGenericClassWithNoGenericInterface()
        {
            var services = new ServiceCollection();
            services.AddAssemblyTypesAsImplementedInterfaces(new RegistrationSettings()
            {
                Assemblies = new[] {
                    typeof(RegistrationTest).Assembly
                }
            });

            var container = services.BuildServiceProvider();

            var instance = container.GetService<INonGeneric>();
            Assert.IsNull(instance);
        }

        [TestMethod]
        public void CanRegisterFactories()
        {
            var services = new ServiceCollection();

            var registrationSettings = new RegistrationSettings()
            {
                Assemblies = new[] {
                    typeof(RegistrationTest).Assembly
                }
            };
            services.AddAssemblyTypesAsImplementedInterfaces(registrationSettings);
            services.AddAssemblyTypesAsFactories(registrationSettings);

            var container = services.BuildServiceProvider();

            var instance = container.GetService<Func<IGenericWrapper>>()();
            Assert.IsNotNull(instance);
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluffySpoon.Extensions.MicrosoftDependencyInjection.Tests
{
    [TestClass]
    public class RegistrationTest
    {
        [TestMethod]
        public void CanRegisterGenericTypes()
        {
            var services = new ServiceCollection();
            services.AddAssemblyTypesAsImplementedInterfaces(typeof(RegistrationTest).Assembly);

            var container = services.BuildServiceProvider();

            var instance = container.GetRequiredService<IGeneric<string>>();
            Assert.IsInstanceOfType(instance, typeof(Generic<string>));
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace FluffySpoon.Extensions.MicrosoftDependencyInjection
{
	public static class RegistrationExtensions
    {
        private delegate ServiceDescriptor GetServiceDescriptorDelegate(
            Type interfaceType, 
            Type implementationType);

        public static void AddAssemblyTypesAsImplementedInterfaces(
            this IServiceCollection serviceCollection,
            RegistrationSettings settings)
        {
            AddAssemblyTypesAsImplementedInterfaces(
                serviceCollection,
                settings,
                (interfaceType, implementationType) => 
                    new ServiceDescriptor(
                        interfaceType, 
                        implementationType, 
                        settings.Scope ?? ServiceLifetime.Scoped));
        }

        public static void AddAssemblyTypesAsFactories(
            this IServiceCollection serviceCollection,
            RegistrationSettings settings)
        {
            AddAssemblyTypesAsImplementedInterfaces(
                serviceCollection,
                settings,
                (interfaceType, implementationType) => {
                    var interfaceFuncType = typeof(Func<>).MakeGenericType(interfaceType);
                    var serviceProviderExtensionsType = typeof(ServiceProviderServiceExtensions);
                    var serviceProviderExtensionsGetServiceMethod = serviceProviderExtensionsType
                        .GetMethods()
                        .Single(x =>
                            x.Name == nameof(ServiceProviderServiceExtensions.GetRequiredService) &&
                            x.IsGenericMethod)
                        .MakeGenericMethod(interfaceType);

                    return new ServiceDescriptor(
                        interfaceFuncType,
                        provider => Delegate.CreateDelegate(
                            interfaceFuncType,
                            provider,
                            serviceProviderExtensionsGetServiceMethod),
                        settings.Scope ?? ServiceLifetime.Singleton);
                });
        }

        private static void AddAssemblyTypesAsImplementedInterfaces(
			IServiceCollection serviceCollection, 
			RegistrationSettings settings,
            GetServiceDescriptorDelegate getServiceDescriptor)
		{
			foreach (var assembly in settings.Assemblies)
			{
				var classTypes = assembly
					.GetTypes()
					.Where(x => x.IsClass && !x.IsAbstract)
					.Where(x => DoesTypeMatchFilters(settings, x));
				foreach (var classType in classTypes)
				{
					try {
						var implementedInterfaceTypes = classType
							.GetInterfaces()
							.Where(x => DoesTypeMatchFilters(settings, x)); ;
						foreach (var implementedInterfaceType in implementedInterfaceTypes)
						{
							var implementationType = classType;
							var interfaceType = implementedInterfaceType;

							if (!interfaceType.IsGenericType && classType.IsGenericType)
							    continue;

							if (implementationType.IsGenericType)
							    implementationType = implementationType.GetGenericTypeDefinition();

							if (implementationType.IsGenericType && interfaceType.IsGenericType)
							    interfaceType = interfaceType.GetGenericTypeDefinition();

                            var descriptor = getServiceDescriptor(
                                interfaceType, 
                                implementationType);
                            serviceCollection.Add(descriptor);
						}
					} catch(Exception ex) {
						throw new Exception("Could not load type " + classType.FullName, ex);
					}
				}
			}
		}

		private static bool DoesTypeMatchFilters(
			RegistrationSettings settings, 
			Type type)
		{
			return settings.Filter == null || 
				(type.Namespace != null && 
				settings.Filter(type));
		}
	}
}

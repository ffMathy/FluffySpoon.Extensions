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
            ScanAssemblyTypes(
                serviceCollection,
                settings,
                (interfaceType, implementationType) => 
                    interfaceType != null ? 
                        new ServiceDescriptor(
                            interfaceType, 
                            implementationType, 
                            settings.Scope ?? ServiceLifetime.Scoped) :
                        null);
        }

        public static void AddAssemblyTypesAsSelf(
            this IServiceCollection serviceCollection,
            RegistrationSettings settings)
        {
            ScanAssemblyTypes(
                serviceCollection,
                settings,
                (interfaceType, implementationType) =>
                    interfaceType == null ?
                        new ServiceDescriptor(
                            implementationType,
                            implementationType,
                            settings.Scope ?? ServiceLifetime.Scoped) :
                        null);
        }

        public static void AddAssemblyTypesAsFactories(
            this IServiceCollection serviceCollection,
            RegistrationSettings settings)
        {
            ScanAssemblyTypes(
                serviceCollection,
                settings,
                (interfaceType, implementationType) =>
                {
                    if (interfaceType == null)
                        return null;

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

        private static void ScanAssemblyTypes(
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
                    try
                    {
                        var implementationType = classType;

                        if (implementationType.IsGenericType)
                            implementationType = implementationType.GetGenericTypeDefinition();

                        var implementationTypeDescriptor = getServiceDescriptor(null, implementationType);
                        if (implementationTypeDescriptor != null)
                            serviceCollection.Add(implementationTypeDescriptor);

                        var implementedInterfaceTypes = classType
							.GetInterfaces()
							.Where(x => DoesTypeMatchFilters(settings, x)); ;
						foreach (var implementedInterfaceType in implementedInterfaceTypes)
						{
							var interfaceType = implementedInterfaceType;

							if (!interfaceType.IsGenericType && classType.IsGenericType)
							    continue;

							if (implementationType.IsGenericType && interfaceType.IsGenericType)
							    interfaceType = interfaceType.GetGenericTypeDefinition();

                            var interfaceAndImplementationTypeDescriptor = getServiceDescriptor(
                                interfaceType, 
                                implementationType);
                            if(interfaceAndImplementationTypeDescriptor != null)
                                serviceCollection.Add(interfaceAndImplementationTypeDescriptor);
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
            var filterToUse = type.IsInterface ? 
                settings.InterfaceFilter : 
                settings.ImplementationFilter;

			return filterToUse == null || 
				(type.Namespace != null &&
                 filterToUse(type));
		}
	}
}

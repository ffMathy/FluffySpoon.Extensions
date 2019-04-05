using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace FluffySpoon.Extensions.MicrosoftDependencyInjection
{
	public static class RegistrationExtensions
	{
		public static void AddAssemblyTypesAsImplementedInterfaces(
			this IServiceCollection serviceCollection, 
			RegistrationSettings settings)
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

							serviceCollection.AddScoped(
							    interfaceType, 
							    implementationType);
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

		public static void AddAssemblyTypesAsImplementedInterfaces(
			this IServiceCollection serviceCollection, 
			params Assembly[] assemblies)
		{
			AddAssemblyTypesAsImplementedInterfaces(serviceCollection, new RegistrationSettings() {
				Assemblies = assemblies
			});
		}
	}
}

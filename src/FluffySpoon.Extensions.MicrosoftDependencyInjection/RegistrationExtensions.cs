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
					var implementedInterfaceTypes = classType
						.GetInterfaces()
						.Where(x => DoesTypeMatchFilters(settings, x)); ;
					foreach (var implementedInterfaceType in implementedInterfaceTypes)
					{
						serviceCollection.AddScoped(implementedInterfaceType, classType);
					}
				}
			}
		}

		private static bool DoesTypeMatchFilters(
			RegistrationSettings settings, 
			Type type)
		{
			return settings.NamespaceSearchString == null || 
				type.Namespace.Contains(settings.NamespaceSearchString);
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

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace FluffySpoon.Extensions.MicrosoftDependencyInjection
{
    public static class RegistrationExtensions
    {
		public static void AddAssemblyTypesAsImplementedInterfaces(this IServiceCollection serviceCollection, params Assembly[] assemblies) {
			foreach(var assembly in assemblies) {
				var classTypes = assembly
					.GetTypes()
					.Where(x => x.IsClass && !x.IsAbstract);
				foreach(var classType in classTypes) {
					var implementedInterfaceTypes = classType.GetInterfaces();
					foreach (var implementedInterfaceType in implementedInterfaceTypes)
					{
						serviceCollection.AddTransient(implementedInterfaceType, classType);
					}
				}
			}
		}
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace FluffySpoon.Extensions.MicrosoftDependencyInjection
{
    public static class RegistrationExtensions
    {
		public static void AddAssemblyTypesAsImplementedInterfaces(this ServiceCollection serviceCollection, params Assembly[] assemblies) {
			foreach(var assembly in assemblies) {
				var classTypes = assembly
					.GetTypes()
					.Where(x => x.IsClass);
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

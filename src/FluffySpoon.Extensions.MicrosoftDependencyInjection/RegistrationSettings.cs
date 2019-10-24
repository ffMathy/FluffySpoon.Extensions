using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FluffySpoon.Extensions.MicrosoftDependencyInjection
{
	public class RegistrationSettings
	{
		public Func<Type, bool> ImplementationFilter { get; set; }
        public Func<Type, bool> InterfaceFilter { get; set; }

        public ServiceLifetime? Scope { get; set; }

        public IEnumerable<Assembly> Assemblies { get; set; }

		public RegistrationSettings()
		{
			Assemblies = new List<Assembly>();
            Scope = ServiceLifetime.Scoped;
        }
	}
}

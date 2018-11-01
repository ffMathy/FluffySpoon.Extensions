using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FluffySpoon.Extensions.MicrosoftDependencyInjection
{
	public class RegistrationSettings
	{
		public string NamespaceSearchString { get; set; }

		public IEnumerable<Assembly> Assemblies { get; set; }

		public RegistrationSettings()
		{
			Assemblies = new List<Assembly>();
		}
	}
}

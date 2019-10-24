using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluffySpoon.Extensions.MicrosoftDependencyInjection
{
    public static class RegistrationFilters
    {
        public static Func<Type, bool> TypesWithinNamespace(params string[] namespaceFragments)
        {
            var @namespace = GetNamespaceFromFragments(namespaceFragments);
            return t => t.Namespace?.StartsWith(@namespace) == true;
        }

        public static Func<Type, bool> TypesWithNamespace(params string[] namespaceFragments)
        {
            var @namespace = GetNamespaceFromFragments(namespaceFragments);
            return t => t.Namespace == @namespace;
        }

        public static Func<Type, bool> TypesWithNamespaceOf(Type type)
        {
            return TypesWithNamespace(type.Namespace);
        }

        public static Func<Type, bool> TypesWithinNamespaceOf(Type type)
        {
            return TypesWithinNamespace(type.Namespace);
        }

        private static string GetNamespaceFromFragments(string[] namespaceFragments)
        {
            return namespaceFragments
                .DefaultIfEmpty()
                .Aggregate((a, b) => a + "." + b);
        }
    }
}

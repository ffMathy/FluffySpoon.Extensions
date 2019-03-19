using System;
using System.Collections.Generic;
using System.Text;

namespace FluffySpoon.Extensions.MicrosoftDependencyInjection.Tests
{
    internal interface IGenericWrapper
    {
        IGeneric<string> Generic { get; }
    }

    class GenericWrapper : IGenericWrapper
    {
        public IGeneric<string> Generic { get; }

        public GenericWrapper(
            IGeneric<string> generic)
        {
            Generic = generic;
        }
    }

    interface INonGeneric
    {

    }

    class GenericFromNonGeneric<T> : INonGeneric
    {

    }

    class Generic<T> : IGeneric<T>
    {

    }

    interface IGeneric<T>
    {

    }
}

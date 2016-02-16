/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;

namespace Edison.Injector
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InjectionInterfaceAttribute : Attribute
    {

        public Type Interface { get; private set; }

        public InjectionInterfaceAttribute(Type type)
        {
            if (!type.IsInterface)
            {
                throw new ArgumentException("Type passed for injection can only be an interface.");
            }

            Interface = type;
        }

    }
}

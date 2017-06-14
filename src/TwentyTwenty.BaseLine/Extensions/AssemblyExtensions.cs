using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TwentyTwenty.BaseLine
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        public static IEnumerable<Type> ScanFor<T>(this Assembly assembly)
            => assembly.GetLoadableTypes().Where(t => t.CanBeCastTo<T>());

        public static IEnumerable<Type> ScanForConcretions<T>(this Assembly assembly)
            => assembly.GetLoadableTypes().Where(t => t.IsConcreteTypeOf<T>());
    }
}
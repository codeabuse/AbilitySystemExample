using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelHunt
{
    public class ClassHelper
    {
        public struct BaseTypeImplementationData
        {
            public Type BaseType { get; }
            public Type[] ConcreteTypes { get; }
            public string[] TypesNames { get; }

            public BaseTypeImplementationData(Type baseType, Type[] concreteTypes)
            {
                BaseType = baseType;
                ConcreteTypes = concreteTypes;
                TypesNames = concreteTypes.Select(x => x.Name).ToArray();
            }
            
            public object CreateInstanceOfType(Type type, params object[] args)
            {
                return Activator.CreateInstance(type, args);
            }
        }
        
        private static readonly Dictionary<Type, BaseTypeImplementationData> cachedImplementationNames = new();

        public static BaseTypeImplementationData GetBaseTypeImplementationData(Type baseType)
        {
            if (cachedImplementationNames.ContainsKey(baseType))
                return cachedImplementationNames[baseType];
            var typesFound = new List<Type>(); 
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                typesFound.AddRange(assembly.GetExportedTypes().Where(x => !x.IsAbstract && x.IsAssignableFrom(baseType)));
            }

            var data = new BaseTypeImplementationData(baseType, typesFound.ToArray());
            cachedImplementationNames.Add(baseType, data);
            return data;
        }
    }
}
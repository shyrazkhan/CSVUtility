using System;
using System.Linq;

namespace CSVUtility.Core.Helpers
{
    public class StrategyLoader
    {
        public static T[] LoadStrategies<T>(string assemblyFilter = null)
        {
            Type[] types;
            if (string.IsNullOrWhiteSpace(assemblyFilter))
                types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            else
                types = System.Reflection.Assembly.Load(assemblyFilter).GetTypes();

            var filteredTypes = types.Where(type => type.IsInterface == false
                                                 && type.IsAbstract == false
                                                 && type.GetInterfaces().Contains(typeof(T))).ToArray();

            var strategies = filteredTypes.Select(Activator.CreateInstance)
                .Cast<T>()
                .ToArray();

            return strategies;
        }
    }
}

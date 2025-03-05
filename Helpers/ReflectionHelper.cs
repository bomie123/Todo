using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Helpers
{
    class ReflectionHelper
    {
        public static BindingFlags DefaultBindingFlags =
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static MethodInfo GetMethodTyped(string nameOfMethod, Type typeToSearch, Type typeToMakeTheMethod)
        {
            var method = typeToSearch.GetMethods(DefaultBindingFlags).FirstOrDefault(x =>
                x.Name.Equals(nameOfMethod, StringComparison.CurrentCultureIgnoreCase));
            return method.MakeGenericMethod(typeToMakeTheMethod);
        }

        public static List<PropertyInfo> GetPropertyInfos(Type typeToSearch, Func<PropertyInfo, bool> func)
            => GetPropertyInfos(typeToSearch, func, DefaultBindingFlags);

        public static List<PropertyInfo> GetPropertyInfos(Type typeToSearch, Func<PropertyInfo, bool> func, BindingFlags flags)
            => typeToSearch.GetType().GetProperties(DefaultBindingFlags)
                .Where(func).ToList();
    }
}

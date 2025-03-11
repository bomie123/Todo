using System.Reflection;

namespace TodoApp.Helpers
{
    class ReflectionHelper
    {
        public static BindingFlags DefaultBindingFlags =
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static CacheHelper cacheHelper { get; set; } = new CacheHelper("ReflectionHelper");
  

        public static MethodInfo GetMethodTyped(string nameOfMethod, Type typeToSearch, Type typeToMakeTheMethod)
        => cacheHelper.GetOrCreate<MethodInfo>($"{nameOfMethod}{typeToSearch.Name}{typeToMakeTheMethod.Name}", () =>
        {
            var method = typeToSearch.GetMethods(DefaultBindingFlags).FirstOrDefault(x =>
                x.Name.Equals(nameOfMethod, StringComparison.CurrentCultureIgnoreCase));
            return method.MakeGenericMethod(typeToMakeTheMethod);
        });

        public static List<PropertyInfo> GetPropertyInfos(Type typeToSearch, Func<PropertyInfo, bool> func)
            => GetPropertyInfos(typeToSearch, func, DefaultBindingFlags);

        public static List<PropertyInfo> GetPropertyInfos(Type typeToSearch, Func<PropertyInfo, bool> func, BindingFlags flags)
            => cacheHelper.GetOrCreate($"propertyInfo-{typeToSearch.Name}", ()=>
            {
                return typeToSearch.GetProperties(flags).Where(func).ToList();
            });


    }
}

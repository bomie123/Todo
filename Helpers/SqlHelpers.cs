namespace TodoApp.Helpers
{
    class SqlHelpers
    {
        public static string EqualToo(string NameOfProperty, object valueOfProperty) => $"{NameOfProperty} = {GetStringInCorrectWrappers(valueOfProperty.ToString(), valueOfProperty.GetType())}";

        public static string GetStringInCorrectWrappers(string value, Type propertyType)
        {
            if (new List<Type>() { typeof(string), typeof(DateTime), typeof(TimeSpan), typeof(decimal) }.Any(z => z == propertyType))
            {
                return $"\"{value}\"";
            }
            if (new List<Type>() { typeof(byte[]), typeof(bool), typeof(byte), typeof(int), typeof(long), typeof(double) }.Any(z => z == propertyType || propertyType.IsEnum))
            {
                return value;
            }


            throw new Exception($"Couldnt find the correct wrappers for type {propertyType.Name}");
        }
    }
}

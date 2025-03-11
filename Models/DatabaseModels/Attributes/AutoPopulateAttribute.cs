namespace TodoApp.Models.DatabaseModels.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    class AutoPopulateAttribute : Attribute
    {
        public Type Type { get; set; }
        public AutoPopulateAttribute(Type type)
        {
            Type = type;
        }
    }
}

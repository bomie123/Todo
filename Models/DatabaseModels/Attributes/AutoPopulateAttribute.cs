using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

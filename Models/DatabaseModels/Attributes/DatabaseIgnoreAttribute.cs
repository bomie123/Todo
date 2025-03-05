using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models.DatabaseModels.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    class DatabaseIgnoreAttribute : Attribute
    {
        public DatabaseIgnoreAttribute(){}
    }
}

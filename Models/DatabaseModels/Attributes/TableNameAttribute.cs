using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models.DatabaseModels.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class TableNameAttribute : Attribute
    {
        public string TableName { get; set; }
        public TableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}

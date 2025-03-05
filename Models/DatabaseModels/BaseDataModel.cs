using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models.DatabaseModels
{
    public abstract class BaseDataModel
    {
        public string Id { get; set; }
        public long RowId { get; set; }
    }
}

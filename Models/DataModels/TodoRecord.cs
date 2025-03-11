using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models.DatabaseModels.Attributes;
using TodoApp.Models.DatabaseModels;

namespace TodoApp.Models.DataModels
{
    class TodoRecord : BaseDataModel
    {
        public string TodoText { get; set; }
        [ForeignKey(typeof(TodoCreationRecord))]
        public long TodoCreatorId { get; set; }
        [AutoPopulate(typeof(TodoCreationRecord))]
        public TodoCreationRecord TodoCreator { get; set; }
        public long TodoCreationRecordId { get; set; }
        public bool Completed { get; set; }
        public bool MustBeDoneToday { get; set; }
    }
}

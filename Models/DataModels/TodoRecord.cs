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
        #region Foreign fields
        [ForeignKey(typeof(TodoCreationRecord))]
        public long TodoCreatorId { get; set; }
        [AutoPopulate(typeof(TodoCreationRecord))]
        public TodoCreationRecord TodoCreator { get; set; }

        #endregion

        #region Calculated
        public string TodoText
        {
            get => TodoCreator?.TodoText??"";
        }
        #endregion

        public DateTime Deadline { get; set; }
        public bool Completed { get; set; }
    }
}

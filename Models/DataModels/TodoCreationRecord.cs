using TodoApp.Models.DatabaseModels;
using TodoApp.Models.DataModels.Enums;

namespace TodoApp.Models.DataModels
{
    class TodoCreationRecord : BaseDataModel
    {
        public string TodoText { get; set; }
        public TimeSpan CreateTodoEvery { get; set; }
        public Urgency TaskUrgency { get; set; }

        public bool HasPrepWork
        {
            get => ShowPrepWorkDaysBefore >0;
        }
        public Urgency PrepWorkUrgency { get; set; }
        public int ShowPrepWorkDaysBefore { get; set; }


    }

}

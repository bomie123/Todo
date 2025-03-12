using TodoApp.Models.DatabaseModels;
using TodoApp.Models.DataModels.Enums;

namespace TodoApp.Models.DataModels
{
    class TodoCreationRecord : BaseDataModel
    {
        public string TodoText { get; set; }
        public TimeSpan CreateTodoEvery { get; set; }
        public int DaysBeforeHighUrgency { get; set; }
        public int DaysBeforeMediumUrgency { get; set; }
        public int DaysBeforeLowUrgency { get; set; }

        public int TotalUrgencyDays
        {
            get => DaysBeforeHighUrgency + DaysBeforeMediumUrgency + DaysBeforeLowUrgency;
        }
    }
}

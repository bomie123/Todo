using KotlinX.Serialization.Descriptors;
using TodoApp.Models.DatabaseModels;
using TodoApp.Models.DatabaseModels.Attributes;
using TodoApp.Models.DataModels.Enums;

namespace TodoApp.Models.DataModels
{
    class TodoCreationRecord : BaseDataModel
    {
        public string TodoText { get; set; }
        public bool HasToBeDoneOnDate { get; set; }
        
        #region Notify
        public DateTime LastNotifyDateTime { get; set; }
        public int DaysBeforeReminder { get; set; }
        public bool MustBeDoneOnDeadline { get; set; }
        #endregion
        
        #region Repeat
        public RepeatInterval RepeatInterval { get; set; }
        public int RepeatFrequency { get; set; }
        #endregion

    }
}

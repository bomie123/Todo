using TodoApp.Models.DatabaseModels.Attributes;
using TodoApp.Models.DatabaseModels;
using TodoApp.Models.DataModels.Enums;

namespace TodoApp.Models.DataModels
{
    class TodoRecord : BaseDataModel
    {
        #region Foreign fields
        [ForeignKey(typeof(TodoCreationRecord))]
        public long TodoCreatorId { get; set; }
        [AutoPopulate]
        public TodoCreationRecord TodoCreator { get; set; }

        #endregion

        #region Calculated
        public string TodoText
        {
            get => TodoCreator?.TodoText ?? "";
        }
        public string HumanReadableTimeframe
        {
            get
            {

                var differenceTimespan = Deadline - DateTime.UtcNow;
                var days = differenceTimespan.Days;
                var text = "";
                if (days < 0)
                {
                    text += "Due for ";
                    days *= -1;
                }
                else if (days == 0)
                {
                    return "Today";
                }
                else
                {
                    text += "In ";
                }


                if (days > 7)
                {
                    var weeks = Math.Round((double)(days / 7), MidpointRounding.ToZero);
                    text += weeks + $" week{(weeks == 1 ? "" : "s")} ";
                }
                var displayDays = Math.Round((double)(days % 7), MidpointRounding.ToZero);

                text += $"{displayDays} day{(displayDays == 1 ? "" : "s")}";
                return text;
            }
        }

        public string DisplayTodoText() =>
            $"{(HasReachedDeadline() ? "Prep:" : "")}{TodoText} {HumanReadableTimeframe}";


        public Urgency CurrentTaskUrgency
        {
            get
            {
                if (HasReachedDeadline())
                    return TodoCreator.TaskUrgency;
                else if (TodoCreator?.HasPrepWork ?? false)
                    return TodoCreator.PrepWorkUrgency;
                return Urgency.Low;
            }
        }
        public bool PrepWorkCompleted
        {
            get => CompletedPrepWorkAt != null;
        }
        public bool CompletedTask
        {
            get => CompletedTaskAt != null;
        }
        public bool HasReachedDeadline() => DateTime.UtcNow.AddDays(-1) < Deadline;

        public bool DisplayOnList() => !(HasReachedDeadline() ? CompletedTask : PrepWorkCompleted);
        #endregion
        public DateTime Deadline { get; set; }
        public DateTime? CompletedPrepWorkAt { get; set; }
        public DateTime? CompletedTaskAt { get; set; }
    }
}

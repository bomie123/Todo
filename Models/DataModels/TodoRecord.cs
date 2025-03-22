using TodoApp.Helpers;
using TodoApp.Models.DatabaseModels.Attributes;
using TodoApp.Models.DatabaseModels;
using TodoApp.Models.DataModels.Enums;
using DateTime = System.DateTime;

namespace TodoApp.Models.DataModels
{
    class TodoRecord : BaseDataModel
    {
        public string Label { get; set; }
        public string LongText { get; set; }
        public int ShowReminderBeforeDays { get; set; }
        public DateTime ActionDate { get; set; }
        public TimeSpan? RepeatEvery { get; set; }
        public Importance MaxImportance { get; set; }

        #region Calculated Fields

        public bool Show
        {
            get => ShowReminder || Due;
        }
        public Importance CurrentImportance
        {
            get
            {
                switch (MaxImportance)
                {
                    case Importance.High:
                        if (Due)
                            return Importance.High;
                        return Importance.Medium;
                    case Importance.Medium:
                        if (Due)
                            return Importance.Medium;
                        return Importance.Low;
                    default: return Importance.Low;
                }
            }
        }
        public bool Due { get => ActionDate < DateTime.UtcNow || DateHelper.GetDateEqual(ActionDate, DateTime.UtcNow); }
        public bool ShowReminder
        {
            get => ShowReminderBeforeDays != 0 && (ActionDate.AddDays(ShowReminderBeforeDays *-1) < DateTime.UtcNow || DateHelper.GetDateEqual(ActionDate.AddDays(ShowReminderBeforeDays * -1), DateTime.UtcNow)) ;
        }
        public DateTime ReminderDate
        {
            get
            {
                return ActionDate.AddDays(ShowReminderBeforeDays);
            }
        }
        public string HumanReadableTimeframe
        {
            get
            {

                var differenceTimespan = ActionDate - DateTime.UtcNow;
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

        #endregion
    }
}

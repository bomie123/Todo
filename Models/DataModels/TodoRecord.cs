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
        [DatabaseIgnore]
        public string TodoText
        {
            get => TodoCreator?.TodoText??"";
        }
        [DatabaseIgnore]
        public string HumanReadableTimeframe
        {
            get
            {
                var differenceTimespan = DateTime.UtcNow - Deadline;
                if (differenceTimespan.Days > 7) //return in week format
                    return $"{Math.Round((double)(differenceTimespan.Days /7), MidpointRounding.ToZero)} weeks {differenceTimespan.Days %7} days";
                return $"";
            }
        }
        [DatabaseIgnore]
        public Urgency Urgency
        {
            get
            {
                var dayCount = 0;
                if (Deadline.AddDays(TodoCreator.DaysBeforeHighUrgency * -1) > DateTime.UtcNow)
                    return Urgency.High;
                dayCount += TodoCreator.DaysBeforeHighUrgency;
                if (Deadline.AddDays((TodoCreator.DaysBeforeMediumUrgency + dayCount) * -1) > DateTime.UtcNow)
                    return Urgency.Medium;
                return Urgency.Low;
            }
        }
        #endregion
        public DateTime Deadline { get; set; }

        public bool Completed { get; set; }
    }
}

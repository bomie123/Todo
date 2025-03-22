using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Helpers
{
    class DateHelper
    {
        public static bool GetDateEqual(DateTime date1, DateTime date2)
            => new DateTime(date1.Year, date1.Month, date1.Day) == new DateTime(date2.Year, date2.Month, date2.Day);
    }
}

// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leisn.Xaml.Wpf.Extensions
{
    public static class DateTimeEx
    {
        public static DateOnly ToDateOnly(this DateTime date) => new(date.Year, date.Month, date.Day);
        public static TimeOnly ToTimeOnly(this DateTime date) => new(date.Hour, date.Minute, date.Second, date.Millisecond);
        public static DateTime WithDate(this DateTime self, int year, int month, int day)
            => new(year, month, day, self.Hour, self.Minute, self.Second, self.Millisecond, self.Kind);
        public static DateTime WithTime(this DateTime self, int hour, int minute, int second = 0, int millisecond = 0)
            => new(self.Year, self.Month, self.Day, hour, minute, second, millisecond, self.Kind);
        public static DateTime WithDate(this DateTime self, DateOnly date)
            => self.WithDate(date.Year, date.Month, date.Day);
        public static DateTime WithTime(this DateTime self, TimeOnly time)
            => self.WithTime(time.Hour, time.Minute, time.Second, time.Millisecond);
    }
}

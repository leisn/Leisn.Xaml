﻿// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;

namespace Leisn.Xaml.Wpf.Extensions
{
    public static class DateTimeEx
    {
        public static DateOnly ToDateOnly(this DateTime date)
        {
            return new(date.Year, date.Month, date.Day);
        }

        public static TimeOnly ToTimeOnly(this DateTime date)
        {
            return new(date.Hour, date.Minute, date.Second, date.Millisecond);
        }

        public static DateTime WithDate(this DateTime self, int year, int month, int day)
        {
            return new(year, month, day, self.Hour, self.Minute, self.Second, self.Millisecond, self.Kind);
        }

        public static DateTime WithTime(this DateTime self, int hour, int minute, int second = 0, int millisecond = 0)
        {
            return new(self.Year, self.Month, self.Day, hour, minute, second, millisecond, self.Kind);
        }

        public static DateTime WithDate(this DateTime self, DateOnly date)
        {
            return self.WithDate(date.Year, date.Month, date.Day);
        }

        public static DateTime WithTime(this DateTime self, TimeOnly time)
        {
            return self.WithTime(time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        #region 农历
        private static readonly string[] Tiangans = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };
        private static readonly string[] Dizhis = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };
        private static readonly Calendar LunisolarCalendar = Calendar.ReadOnly(new ChineseLunisolarCalendar());
        public static string GetLunisolarYear(this DateTime self)
        {
            var year = LunisolarCalendar.GetYear(self);
            var tiangan = (year - 4) % 10;
            var dizhi = (year - 4) % 12;
            return string.Concat(Tiangans[tiangan], Dizhis[dizhi]);
        }
        private static readonly string[] Shengxiaos = { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };
        public static string GetZodiac(this DateTime self)
        {
            var dizhi = (LunisolarCalendar.GetYear(self) - 4) % 12;
            return Shengxiaos[dizhi];
        }
        private static readonly string[] Yuefens = { "正", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "腊" };
        public static string GetLunisolarMonth(this DateTime self)
        {
            var mouth = LunisolarCalendar.GetMonth(self);
            var leapMouth = LunisolarCalendar.GetLeapMonth(LunisolarCalendar.GetYear(self));
            bool isLeap = mouth == leapMouth;
            return isLeap ? $"闰{Yuefens[mouth - 1]}" : Yuefens[mouth > leapMouth ? mouth - 1 : mouth];
        }

        private static readonly string[] Riqis1 = { "初", "十", "廿", "三" };
        private static readonly string[] Riqis2 = { "十", "一", "二", "三", "四", "五", "六", "七", "八", "九"};

        public static string GetLunisolarDay(this DateTime self, bool getMonthOfFirstDay = false)
        {
            var day = LunisolarCalendar.GetDayOfMonth(self);
            if (getMonthOfFirstDay && day == 1)
                return string.Concat(GetLunisolarMonth(self), "月");
            if (day == 20)
                return "二十";
            int d1 = (day - 1) / 10;
            int d2 = day % 10;
            if (d1 > 1 && d2 == 0)
                return string.Concat(Riqis1[d1 + 1], Riqis2[d2]);
            return string.Concat(Riqis1[d1], Riqis2[d2]);
        }

        public static string ToLunisolarDateString(this DateTime self)
        {
            return $"{GetLunisolarYear(self)}年 {GetLunisolarMonth(self)}月{GetLunisolarDay(self)}";
        }
        #endregion
    }
}

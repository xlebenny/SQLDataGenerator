using Benny.CSharpHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SQLDataGeneratorLibrary.FormatterTable;

namespace SQLDataGeneratorLibrary
{
    // ~  range
    // /  or (either one)
    internal class DateTimeFormatter : IFormatter
    {
        public DateTImeFormatter_Data[] _datas { get; set; }
        private State[] _states { get; set; }

        private static Random Random = new Random();

        public override IFormatter CreateInstance(string args)
        {
            _datas = JsonConvert.DeserializeObject<DateTImeFormatter_Data[]>(args);
            return this;
        }

        public override void BeforeTableExecute(FormatterTableHelper helper)
        {
            var temp = new List<State>();

            foreach (var data in _datas) { temp.Add(new State(data)); }

            _states = temp.ToArray();
        }

        public override string FormatField(FormatterTableHelper helper)
        {
            var state = _states[Random.Next(_states.Length)];
            var result = state.GetRandomDateTime();

            //TODO support other database
            return result.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("_states: ");
            foreach (var state in _states) { sb.Append("\t"); sb.AppendLine(state.ToString()); }

            return sb.ToString();
        }

        private class State
        {
            public Range<long> DateRange = new Range<long>();
            public Dictionary<DayOfWeek, Range<TimeSpan>> TimeRanges = new Dictionary<DayOfWeek, Range<TimeSpan>>(); //[dayOfWeek][time(offset)]

            private static Func<string, DateTime> PraseDate = x => DateTime.ParseExact(x, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            private static Func<string, TimeSpan> PraseTime = x => TimeSpan.ParseExact(x, "hh\\:mm\\:ss", System.Globalization.CultureInfo.InvariantCulture);

            private static Dictionary<string, DayOfWeek> DayOfWeekMap = new Dictionary<string, DayOfWeek>()
            {
                { "mon", DayOfWeek.Monday }, { "tue", DayOfWeek.Tuesday }, { "wed", DayOfWeek.Wednesday },
                { "thur", DayOfWeek.Thursday}, { "fri", DayOfWeek.Friday}, { "sat", DayOfWeek.Saturday },
                { "sun", DayOfWeek.Sunday }
            };

            public State(DateTImeFormatter_Data data)
            {
                #region DateRange

                var dates = data.DayRange.Split('~');
                var dateRange = new Range<long>
                {
                    Minimum = PraseDate(dates[0]).Ticks,
                    Maximum = dates.Length > 1 ? PraseDate(dates[1]).Ticks : PraseDate(dates[0]).AddDays(1).AddSeconds(-1).Ticks
                };
                DateRange = dateRange;

                #endregion DateRange

                #region TimeRanges

                foreach (var timeRangeOfWeekDay in data.TimeRangeOfWeekDay)
                {
                    var splited = timeRangeOfWeekDay.Split(new char[] { ':' }, 2);
                    var weekdays = splited[0].Split('/');
                    var times = splited[1].Split('~');
                    var timeRange = new Range<TimeSpan>
                    {
                        Minimum = PraseTime(times[0]),
                        Maximum = PraseTime(times[1])
                    };

                    //if throw exception here, please make sure input is correct
                    weekdays.ToList().ForEach(x => TimeRanges.Add(DayOfWeekMap[x.ToLower()], timeRange));
                }

                #endregion TimeRanges
            }

            public DateTime GetRandomDateTime()
            {
                var result = RetryHelper.Retry<DateTime>(
                            () => new DateTime(Random.Next(DateRange.Minimum, DateRange.Maximum)),
                            x =>
                            {
                                var timeRange = TimeRanges.GetValueOrDefault(x.DayOfWeek);
                                return timeRange != null
                                    //time in range
                                    && (timeRange.Minimum <= x.TimeOfDay && x.TimeOfDay <= timeRange.Maximum);
                            }
                );

                return result;
            }
        }

        internal class DateTImeFormatter_Data
        {
            public string DayRange { get; set; }
            public string[] TimeRangeOfWeekDay { get; set; }
        }
    }
}
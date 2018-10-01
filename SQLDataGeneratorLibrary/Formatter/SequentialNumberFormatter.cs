using System.Collections.Generic;
using static SQLDataGeneratorLibrary.FormatterTable;

namespace SQLDataGeneratorLibrary
{
    internal class SequentialNumberFormatter : IFormatter
    {
        public long Start { get; set; } = 0;
        public long End { get; set; } = long.MaxValue;
        public long Step { get; set; } = 1;
        public string Padding { get; set; } = "{0}";

        private State _state { get; set; }

        public override void BeforeTableExecute(FormatterTableHelper helper)
        {
            _state = new State { Current = Start };

            //can't read { in Format, so add { in here
            //if (Padding[0] != '{')
            //    Padding = string.Format("{{{0}}}", Padding);
        }

        public override string FormatField(FormatterTableHelper helper)
        {
            return string.Format(Padding, _state.Current);
        }

        public override void AfterRowExecute(FormatterTableHelper helper)
        {
            _state.Current += Step;

            if (_state.Current > End)
                _state.Current = Start;
        }

        public override string ToString()
        {
            return $"Start: { Start }, End: { End }, Step: { Step }, Padding: { Padding }";
        }

        private class State
        {
            public long Current { get; set; }
        }
    }
}
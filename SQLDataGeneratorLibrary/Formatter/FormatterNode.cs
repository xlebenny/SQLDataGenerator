using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLDataGeneratorLibrary
{
    internal class FormatterNode : IFormatter
    {
        public const string VariableString = "{x}";

        public string FormatString { get; set; } = "{x}"; //TODO private
        public List<IFormatter> Children { get; set; } = new List<IFormatter>();

        public override void BeforeTableExecute(FormatterTable.FormatterTableHelper helper)
        {
            var i = 0;

            //https://stackoverflow.com/questions/2245442/c-sharp-split-a-string-by-another-string
            FormatString = FormatString
                                    .Split(new string[] { VariableString }, StringSplitOptions.None)
                                    .Aggregate((x, y) => string.Format("{0}{{{1}}}{2}", x, i++, y))
                                    //because string.format escape word is {{ }}
                                    .Replace("\\{", "{{").Replace("\\}", "}}");

            //TODO don't copy
            Children.ForEach(x => x.BeforeTableExecute(helper));
        }

        public override void BeforeRowExecute(FormatterTable.FormatterTableHelper helper)
        {
            //TODO don't copy
            Children.ForEach(x => x.BeforeRowExecute(helper));
        }

        public override void BeforeFieldExecute(FormatterTable.FormatterTableHelper helper)
        {
            //TODO don't copy
            Children.ForEach(x => x.BeforeFieldExecute(helper));
        }

        public override string FormatField(FormatterTable.FormatterTableHelper helper)
        {
            var formatArgs = Children.Select(x => helper.FormatField(x)).ToArray();

            return string.Format(FormatString, formatArgs);
        }

        public override void AfterFieldExecute(FormatterTable.FormatterTableHelper helper)
        {
            //TODO don't copy
            Children.ForEach(x => x.AfterFieldExecute(helper));
        }

        public override void AfterRowExecute(FormatterTable.FormatterTableHelper helper)
        {
            //TODO don't copy
            Children.ForEach(x => x.AfterRowExecute(helper));
        }

        public override void AfterTableExecute(FormatterTable.FormatterTableHelper helper)
        {
            //TODO don't copy
            Children.ForEach(x => x.AfterTableExecute(helper));
        }

        public List<IFormatter> GetAllChildren()
        {
            var result = new List<IFormatter>();

            Children.ForEach(x =>
            {
                if (x is FormatterNode)
                    result.AddRange(((FormatterNode)x).GetAllChildren());
                result.Add(x);
            });

            return result;
        }
    }
}
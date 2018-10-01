using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static SQLDataGeneratorLibrary.FormatterTable;

namespace SQLDataGeneratorLibrary
{
    internal class DictionaryFormatter : IFormatter
    {
        public string Name { get; set; }
        public string Field { get; set; }

        private static Func<string, string, string> GetKey =
            (tableName, dictName) => string.Format("{0}-=-{1}", tableName, dictName);

        private State _state { get; set; }

        public override void BeforeTableExecute(FormatterTableHelper helper)
        {
            //one dict name, one state
            var key = GetKey(helper.CurrentTable.TableName, Name);

            if (helper.Session.ContainsKey(key))
            {
                _state = (State)helper.Session[key];
            }
            else
            {
                _state = new State(Name);
                _state.Init();
                helper.Session[key] = _state;
            }

            _state.RegistField(Field);
        }

        public override string FormatField(FormatterTableHelper helper)
        {
            return _state.NextWord(Field);
        }

        public override string ToString()
        {
            return $"_state: { _state.ToString() }";
        }

        private class State
        {
            private string FileName;
            private string[] ColumnNames;
            private Dictionary<string, string>[] Lines; //rows, field[columnName]
            private long CurrentIndex;
            private IList<string> RegistedFields = new List<string>();

            public State(string fileName)
            {
                FileName = string.Format("./Dictionary/{0}.txt", fileName);
            }

            public void Init()
            {
                if (Lines == null)
                {
                    var lines = ReadFromFile();
                    ColumnNames = lines[0].Split('\t');
                    Lines = lines
                                .Skip(1)
                                .Select(x =>
                                {
                                    var ary = x.Split('\t');
                                    var result = new Dictionary<string, string>();

                                    for (var i = 0; i < ary.Length; i++)
                                        result.Add(ColumnNames[i], ary[i]);
                                    return result;
                                }).ToArray();
                }
                ShuffleWordArray();
                CurrentIndex = 0;
            }

            public void RegistField(string fieldName)
            {
                RegistedFields.Add(fieldName);
            }

            public string NextWord(string field)
            {
                string result = null;
                Func<long> index = () => CurrentIndex / RegistedFields.LongCount();

                if (index() >= Lines.LongLength)
                    Init();

                result = Lines[index()][field];

                CurrentIndex++;

                return result;
            }

            //ref https://stackoverflow.com/questions/108819/best-way-to-randomize-an-array-with-net
            private void ShuffleWordArray()
            {
                var random = new Random();

                Lines = Lines.OrderBy(x => random.Next()).ToArray();
            }

            private string[] ReadFromFile()
            {
                //If file not found,
                //Right click txt > Property > Build Actioin: Content, Copy to Output Directory: Copy If newer
                return File
                            .ReadAllLines(FileName)
                            .Select(x => x.Trim())
                            .Where(x =>
                                        x.Length != 0
                                        && !x.StartsWith("#") && !x.StartsWith("//")
                            )
                            .ToArray();
            }

            public override string ToString()
            {
                return $"FileName: { FileName }, CurrentIndex: { CurrentIndex }";
            }
        }
    }
}
using Benny.CSharpHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SQLDataGeneratorLibrary
{
    public class DateTimeConverter : IReferenceFormatterConverter
    {
        private Dictionary<string, MethodInfo> methodCache = new Dictionary<string, MethodInfo>();

        public string Convert(string value, object[] parameters)
        {
            var date = DateTime.Parse(value);
            var methodName = (string)parameters[0];
            var args = parameters.Skip(1).ToArray();
            var method = methodCache.GetValueOrAddDefault(
                methodName,
                typeof(DateTime).GetMethod(methodName)
            );
            var result = (DateTime)method.Invoke(date, args);

            //TODO support other database
            return result.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
using Newtonsoft.Json;
using static SQLDataGeneratorLibrary.FormatterTable;

namespace SQLDataGeneratorLibrary
{
    public abstract class IFormatter
    {
        //FIXME: bad idea create self at instance, but i need set this to default behavior & no time to fix
        public virtual IFormatter CreateInstance(string args)
        {
            return (IFormatter)JsonConvert.DeserializeObject(string.Format("{{ {0} }}", args), this.GetType());
        }

        public virtual void BeforeTableExecute(FormatterTableHelper helper)
        {
            //waiting for override
        }

        public virtual void BeforeRowExecute(FormatterTableHelper helper)
        {
            //waiting for override
        }

        public virtual void BeforeFieldExecute(FormatterTableHelper helper)
        {
            //waiting for override
        }

        public abstract string FormatField(FormatterTableHelper helper);

        public virtual void AfterFieldExecute(FormatterTableHelper helper)
        {
            //waiting for override
        }

        public virtual void AfterRowExecute(FormatterTableHelper helper)
        {
            //waiting for override
        }

        public virtual void AfterTableExecute(FormatterTableHelper helper)
        {
            //waiting for override
        }

        public virtual void DependencyByTableExecuted(FormatterTableHelper helper)
        {
            //waiting for override
        }
    }
}
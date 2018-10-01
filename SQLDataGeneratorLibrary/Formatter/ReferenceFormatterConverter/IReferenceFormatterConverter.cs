namespace SQLDataGeneratorLibrary
{
    public interface IReferenceFormatterConverter
    {
        /// <summary>
        /// Implete when need to change reference value
        /// </summary>
        /// <param name="value">Value from reference</param>
        /// <param name="parameters">converter paramter</param>
        /// <returns></returns>
        string Convert(string value, object[] parameters);
    }
}
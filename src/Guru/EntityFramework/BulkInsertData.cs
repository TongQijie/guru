namespace Guru.EntityFramework
{
    public class BulkInsertData
    {
        public string TableName { get; set; }

        public string[] ParameterNames { get; set; }

        public object[,] Values { get; set; }
    }
}

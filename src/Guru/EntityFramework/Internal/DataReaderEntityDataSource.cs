using System.Data;

namespace Guru.EntityFramework.Internal
{
    internal class DataReaderEntityDataSource : AbstractEntityDataSource
    {
        public DataReaderEntityDataSource(IDataReader dataReader)
        {
            _DataReader = dataReader;
        }

        private IDataReader _DataReader = null;

        public override object this[string columnName]
        {
            get { return _DataReader[columnName]; }
        }

        public override object this[int index]
        {
            get { return _DataReader[index]; }
        }

        public override bool ContainsColumn(string columnName)
        {
            return true;
        }

        public override void Dispose()
        {
            if (_DataReader != null)
            {
                _DataReader.Dispose();
            }
        }
    }
}
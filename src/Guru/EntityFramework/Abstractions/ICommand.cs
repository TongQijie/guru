using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Guru.EntityFramework.Abstractions
{
    public interface ICommand
    {
        DbCommand DbCommand { get; }

        IDatabase Database { get; }

        void AddParameter(string parameterName, DbType dbType, ParameterDirection direction, int size);

        void SetParameterValue(string parameterName, object parameter);

        void SetParameterValues(string parameterName, params object[] parameters);

        void FormatCommandText(int index, params object[] args);

        object GetParameterValue(string parameterName);

        T GetScalar<T>();

        Task<T> GetScalarAsync<T>();

        List<T> GetEntities<T>() where T : class;

        Task<List<T>> GetEntitiesAsync<T>() where T : class;

        int ExecuteNonQuery();

        Task<int> ExecuteNonQueryAsync();
    }
}
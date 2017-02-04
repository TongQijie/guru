using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Guru.EntityFramework.Abstractions
{
    public interface IDatabase : IDisposable
    {
        DbProviderFactory DbProviderFactory { get; }

        T GetScalar<T>(ICommand command);

        Task<T> GetScalarAsync<T>(ICommand command);

        List<T> GetEntities<T>(ICommand command) where T : class;

        Task<List<T>> GetEntitiesAsync<T>(ICommand command) where T : class;

        int ExecuteNonQuery(ICommand command);

        Task<int> ExecuteNonQueryAsync(ICommand command);

        bool ExecuteTransaction(Func<IDatabase, bool> execution);

        Task<bool> ExecuteTransactionAsync(Func<IDatabase, Task<bool>> execution);
    }
}
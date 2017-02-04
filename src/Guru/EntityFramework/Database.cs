using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

using Guru.EntityFramework.Internal;
using Guru.EntityFramework.Abstractions;
using System.Threading.Tasks;

namespace Guru.EntityFramework
{
    public class Database : IDatabase
    {
        public DbProviderFactory DbProviderFactory { get; private set; }

        protected string _ConnectionString = null;

        protected DbConnection _DbConnection = null;

        private DbTransaction _DbTransaction = null;

        public Database(DbProviderFactory dbProviderFactory, string connectionString)
        {
            DbProviderFactory = dbProviderFactory;
            _ConnectionString = connectionString;
        }

        private DbConnection CreateConnection()
        {
            if (_DbConnection == null)
            {
                _DbConnection = DbProviderFactory.CreateConnection();
                _DbConnection.ConnectionString = _ConnectionString;
            }

            return _DbConnection;
        }

        private DbConnection OpenConnection()
        {
            if (_DbConnection == null)
            {
                CreateConnection();
            }

            if (_DbConnection.State != ConnectionState.Open)
            {
                _DbConnection.Open();
            }

            return _DbConnection;
        }

        private async Task<DbConnection> OpenConnectionAsync()
        {
            if (_DbConnection == null)
            {
                CreateConnection();
            }

            if (_DbConnection.State != ConnectionState.Open)
            {
                await _DbConnection.OpenAsync();
            }

            return _DbConnection;
        }

        private DbCommand CreateDbCommnad(ICommand databaseCommand)
        {
            databaseCommand.DbCommand.Connection = OpenConnection();
            databaseCommand.DbCommand.Transaction = _DbTransaction;
            return databaseCommand.DbCommand;
        }

        private async Task<DbCommand> CreateDbCommnadAsync(ICommand databaseCommand)
        {
            databaseCommand.DbCommand.Connection = await OpenConnectionAsync();
            databaseCommand.DbCommand.Transaction = _DbTransaction;
            return databaseCommand.DbCommand;
        }

        public int ExecuteNonQuery(ICommand command)
        {
            return CreateDbCommnad(command).ExecuteNonQuery();
        }

        public bool ExecuteTransaction(Func<IDatabase, bool> execution)
        {
            var success = false;
            try
            {
                _DbTransaction = OpenConnection().BeginTransaction();
                success = execution(this);
            }
            catch (Exception e)
            {
                throw new Exception("failed to execute transaction.", e);
            }
            finally
            {
                if (_DbTransaction != null)
                {
                    if (success)
                    {
                        _DbTransaction.Commit();
                    }
                    else
                    {
                        _DbTransaction.Rollback();
                    }

                    _DbTransaction = null;
                }
            }

            return success;
        }

        public async Task<bool> ExecuteTransactionAsync(Func<IDatabase, Task<bool>> execution)
        {
            var success = false;
            try
            {
                _DbTransaction = (await OpenConnectionAsync()).BeginTransaction();
                success = await execution(this);
            }
            catch (Exception e)
            {
                throw new Exception("failed to execute transaction.", e);
            }
            finally
            {
                if (_DbTransaction != null)
                {
                    if (success)
                    {
                        _DbTransaction.Commit();
                    }
                    else
                    {
                        _DbTransaction.Rollback();
                    }

                    _DbTransaction = null;
                }
            }

            return success;
        }

        public List<T> GetEntities<T>(ICommand command) where T : class
        {
            var result = new List<T>();
            using (var reader = CreateDbCommnad(command).ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add((T)EntityBuilder.GetBuilder(typeof(T)).Build(reader));
                }
            }
            return result;
        }

        public T GetScalar<T>(ICommand command)
        {
            return (T)CreateDbCommnad(command).ExecuteScalar();
        }

        public void Dispose()
        {
            if (_DbConnection != null)
            {
                if (_DbConnection.State == ConnectionState.Open)
                {
                    _DbConnection.Close();
                }
                _DbConnection = null;
            }
        }

        public async Task<T> GetScalarAsync<T>(ICommand command)
        {
            return (T)(await (await CreateDbCommnadAsync(command)).ExecuteScalarAsync());
        }

        public async Task<List<T>> GetEntitiesAsync<T>(ICommand command) where T : class
        {
            var result = new List<T>();
            using (var reader = await (await CreateDbCommnadAsync(command)).ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    result.Add((T)EntityBuilder.GetBuilder(typeof(T)).Build(reader));
                }
            }

            return result;
        }

        public async Task<int> ExecuteNonQueryAsync(ICommand command)
        {
            return await (await CreateDbCommnadAsync(command)).ExecuteNonQueryAsync();
        }
    }
}
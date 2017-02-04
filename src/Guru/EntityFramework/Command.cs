using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Guru.EntityFramework.Abstractions;
using System.Threading.Tasks;

namespace Guru.EntityFramework
{
    public class Command : ICommand
    {
        public DbCommand DbCommand { get; private set; }

        public IDatabase Database { get; private set; }

        public Command(IDatabase database, CommandType commandType, string commandText)
        {
            Database = database;
            DbCommand = database.DbProviderFactory.CreateCommand();
            DbCommand.CommandText = commandText.Replace('．', '.').Replace('　', ' ');
            DbCommand.CommandType = commandType;
        }

        public object GetParameterValue(string parameterName)
        {
            if (DbCommand.Parameters.Contains(parameterName))
            {
                return DbCommand.Parameters[parameterName].Value;
            }
            else
            {
                return null;
            }
        }

        public void AddParameter(string parameterName, DbType dbType, ParameterDirection direction, int size)
        {
            if (!DbCommand.Parameters.Contains(parameterName))
            {
                var dbParameter = Database.DbProviderFactory.CreateParameter();
                dbParameter.ParameterName = parameterName;
                dbParameter.DbType = dbType;
                dbParameter.Direction = direction;
                dbParameter.Size = size;
                DbCommand.Parameters.Add(dbParameter);
            }
        }

        public void SetParameterValue(string parameterName, object parameterValue)
        {
            if (!DbCommand.Parameters.Contains(parameterName))
            {
                throw new ArgumentException("parameter does not exists.");
            }
            else
            {
                if (parameterValue == null)
                {
                    DbCommand.Parameters[parameterName].Value = DBNull.Value;
                }
                else
                {
                    DbCommand.Parameters[parameterName].Value = parameterValue;
                }
            }
        }

        public void SetParameterValues(string parameterName, params object[] parameterValues)
        {
            if (!DbCommand.Parameters.Contains(parameterName))
            {
                throw new ArgumentException("parameter does not exists.");
            }

            var stringBuilder = new StringBuilder();
            for (int i = 0; i < parameterValues.Length; i++)
            {
                var name = parameterName + i;
                stringBuilder.AppendFormat("{0},", name);
                AddParameter(name, DbCommand.Parameters[parameterName].DbType, ParameterDirection.Input, DbCommand.Parameters[parameterName].Size);
                SetParameterValue(name, parameterValues[i]);
            }
            stringBuilder.ToString().Trim(',');

            DbCommand.Parameters.Remove(DbCommand.Parameters[parameterName]);
            DbCommand.CommandText = DbCommand.CommandText.Replace(parameterName, stringBuilder.ToString().Trim(','));
        }

        public void FormatCommandText(int index, params object[] args)
        {
            if (args == null || args.Length == 0 || !Regex.IsMatch(DbCommand.CommandText, "\\x7b" + index + "\\x7d"))
            {
                return;
            }

            var stringArgs = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is string || args[i] is DateTime)
                {
                    stringArgs[i] = string.Format("'{0}'", args[i].ToString());
                }
                else
                {
                    stringArgs[i] = args[i].ToString();
                }
            }

            DbCommand.CommandText = Regex.Replace(DbCommand.CommandText, "\\x7b" + index + "\\x7d", string.Join(",", stringArgs));
        }

        public T GetScalar<T>()
        {
            using (Database)
            {
                return Database.GetScalar<T>(this);
            }
        }

        public List<T> GetEntities<T>() where T : class
        {
            using (Database)
            {
                return Database.GetEntities<T>(this);
            }
        }

        public int ExecuteNonQuery()
        {
            using (Database)
            {
                return Database.ExecuteNonQuery(this);
            }
        }

        public async Task<T> GetScalarAsync<T>()
        {
            using (Database)
            {
                return await Database.GetScalarAsync<T>(this);
            }
        }

        public async Task<List<T>> GetEntitiesAsync<T>() where T : class
        {
            using (Database)
            {
                return await Database.GetEntitiesAsync<T>(this);
            }
        }

        public async Task<int> ExecuteNonQueryAsync()
        {
            using (Database)
            {
                return await Database.ExecuteNonQueryAsync(this);
            }
        }
    }
}
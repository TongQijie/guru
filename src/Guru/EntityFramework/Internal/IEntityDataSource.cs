using System;

namespace Guru.EntityFramework.Internal
{
    internal interface IEntityDataSource : IDisposable
    {
        object this[string columnName] { get; }

        object this[int index] { get; }

        bool ContainsColumn(string columnName);

        object GetValue(string columnName, Type targetType);
    }
}
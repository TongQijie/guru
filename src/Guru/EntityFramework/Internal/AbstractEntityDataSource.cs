using System;
using System.Reflection;

using Guru.ExtensionMethod;

namespace Guru.EntityFramework.Internal
{
    internal abstract class AbstractEntityDataSource : IEntityDataSource
    {
        public abstract object this[int index] { get; }

        public abstract object this[string columnName] { get; }

        public abstract bool ContainsColumn(string columnName);

        public object GetValue(string columnName, Type targetType)
        {
            if (!ContainsColumn(columnName))
            {
                return null;
            }

            if (this[columnName] == DBNull.Value)
            {
                return targetType.GetDefaultValue();
            }

            if (targetType == typeof(string))
            {
                return this[columnName].ToString().Trim();
            }

            if (targetType.GetTypeInfo().IsEnum)
            {
                return Enum.ToObject(targetType, this[columnName]);
            }

            return Convert.ChangeType(this[columnName], targetType);
        }

        public virtual void Dispose()
        {
        }
    }
}
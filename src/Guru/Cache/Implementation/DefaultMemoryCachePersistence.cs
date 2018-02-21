using Guru.DependencyInjection;
using Guru.ExtensionMethod;
using Guru.Formatter.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Guru.Cache.Implementation
{
    internal class DefaultMemoryCachePersistence
    {
        private readonly ILightningFormatter _Formatter;

        public DefaultMemoryCachePersistence()
        {
            _Formatter = DependencyContainer.Resolve<IJsonLightningFormatter>();
        }

        public void StoreToFile(IEnumerable<DefaultMemoryCacheItem> items)
        {
            var entities = new List<Entity>();
            foreach (var item in items)
            {
                var entity = new Entity()
                {
                    Key = item.Key,
                    Timestamp = item.ExpiryTime.Timestamp(),
                    Type = item.Value.GetType().AssemblyQualifiedName,
                };

                var type = item.Value.GetType();

                if (item.Value.GetType() == typeof(string) || type.IsValueType)
                {
                    entity.Value = item.Value.ToString();
                }
                else
                {
                    entity.Value = _Formatter.WriteObject(item.Value);
                }

                entities.Add(entity);
            }

            "./Cache".EnsureFolder();

            using (var outputStream = new FileStream("./Cache/MemoryCache.dat".FullPath(), FileMode.Create, FileAccess.Write))
            {
                _Formatter.WriteObject(entities, outputStream);
            }
        }

        public IEnumerable<DefaultMemoryCacheItem> RestoreFromFile()
        {
            if (!"./Cache/MemoryCache.dat".IsFile())
            {
                return null;
            }

            List<Entity> entities = null;
            using (var inputStream = new FileStream("./Cache/MemoryCache.dat".FullPath(), FileMode.Open, FileAccess.Read))
            {
                entities = _Formatter.ReadObject<List<Entity>>(inputStream);
            }

            var items = new List<DefaultMemoryCacheItem>();

            foreach (var entity in entities)
            {
                var item = new DefaultMemoryCacheItem()
                {
                    Key = entity.Key,
                    ExpiryTime = entity.Timestamp.DateTime(),
                };

                var type = Type.GetType(entity.Type);
                if (type == null)
                {
                    continue;
                }

                if (type.IsValueType || type == typeof(string))
                {
                    item.Value = entity.Value.ConvertTo(type);
                }
                else
                {
                    item.Value = _Formatter.ReadObject(type, entity.Value.ToString());
                }

                items.Add(item);
            }

            return items;
        }

        public class Entity
        {
            public string Key { get; set; }

            public string Type { get; set; }

            public string Value { get; set; }

            public long Timestamp { get; set; }
        }
    }
}
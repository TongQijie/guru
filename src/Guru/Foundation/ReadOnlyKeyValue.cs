namespace Guru.Foundation
{
    public class ReadOnlyKeyValue<TKey, TValue>
    {
        public ReadOnlyKeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; private set; }

        public TValue Value { get; private set; }
    }
}
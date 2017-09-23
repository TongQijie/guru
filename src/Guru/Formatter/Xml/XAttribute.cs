namespace Guru.Formatter.Xml
{
    public class XAttribute : XBase
    {
        public byte[] Key { get; set; }

        public byte[] Value { get; set; }

        public override string ToString()
        {
            return System.Text.Encoding.UTF8.GetString(Key) + "=" + System.Text.Encoding.UTF8.GetString(Value);
        }

        public string KeyString
        {
            get { return System.Text.Encoding.UTF8.GetString(Key); }
        }

        public string ValueString
        {
            get { return System.Text.Encoding.UTF8.GetString(Value); }
        }
    }
}

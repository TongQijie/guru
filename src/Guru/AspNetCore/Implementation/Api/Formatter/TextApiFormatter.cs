using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Guru.AspNetCore.Implementation.Api.Formatter
{
    public class TextApiFormatter : AbstractApiFormatter
    {
        public TextApiFormatter()
        {
            ContentType = "plain/text";
        }

        public override async Task<object> Read(Type targetType, Stream stream)
        {
            var data = new byte[16 * 1024];
            var index = 0;

            var buffer = new byte[4 * 1024];
            var count = 0;
            while ((count = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                if ((data.Length - index) >= count)
                {
                    Buffer.BlockCopy(buffer, 0, data, index, count);
                    index += count;
                }
                else
                {
                    var newData = new byte[data.Length * 2];
                    Buffer.BlockCopy(data, 0, newData, 0, index);
                    Buffer.BlockCopy(buffer, 0, data, index, count);
                    index += count;
                    data = newData;
                }
            }

            return Encoding.UTF8.GetString(data, 0, index);
        }

        public override async Task Write(object instance, Stream stream)
        {
            var data = Encoding.UTF8.GetBytes(instance?.ToString());
            await stream.WriteAsync(data, 0, data.Length);
        }
    }
}
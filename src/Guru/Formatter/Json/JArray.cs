using System.Threading.Tasks;

using Guru.ExtensionMethod;
using Guru.Formatter.Internal;

namespace Guru.Formatter.Json
{
    public class JArray : JBase
    {
        public JBase[] Elements { get; set; }

        internal override bool Fill(IReaderStream stream, byte[] seperators, byte[] terminators)
        {
            Elements = new JBase[0];

            while (!Parse(stream))
            {
                ;
            }

            var b = stream.SeekBytesUntilVisiableChar();
            if (b == -1)
            {
                return true;
            }

            if (seperators != null && seperators.Exists(x => x == b))
            {
                return false;
            }

            if (terminators != null && terminators.Exists(x => x == b))
            {
                return true;
            }

            throw new Errors.JsonDeserializeFailedException(stream.Position, "collection element has invalid terminal byte.");
        }

        internal override async Task<bool> FillAsync(IReaderStream stream, byte[] seperators, byte[] terminators)
        {
            Elements = new JBase[0];

            while (!await ParseAsync(stream)) { }

            var b = await stream.SeekBytesUntilVisiableCharAsync();
            if (b == -1)
            {
                return true;
            }

            if (seperators != null && seperators.Exists(x => x == b))
            {
                return false;
            }

            if (terminators != null && terminators.Exists(x => x == b))
            {
                return true;
            }

            throw new Errors.JsonDeserializeFailedException(stream.Position, "collection element has invalid terminal byte.");
        }

        private bool Parse(IReaderStream stream)
        {
            var args = new JsonParserArgs()
            {
                ExternalObject = this,
                Stream = stream,
            };
            JsonParser.Parse(args);

            if (args.InternalObject != null)
            {
                Elements = Elements.Append(args.InternalObject);
            }

            return args.Handled;
        }

        private async Task<bool> ParseAsync(IReaderStream stream)
        {
            var args = new JsonParserArgs()
            {
                ExternalObject = this,
                Stream = stream,
            };
            await JsonParser.ParseAsync(args);

            if (args.InternalObject != null)
            {
                Elements = Elements.Append(args.InternalObject);
            }

            return args.Handled;
        }
    }
}
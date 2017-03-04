using System.Threading.Tasks;

using Guru.ExtensionMethod;
using Guru.Formatter.Internal;

namespace Guru.Formatter.Json
{
    public class JsonCollectionObject : JsonObject
    {
        public JsonCollectionElement[] Elements { get; set; }

        internal override bool Fill(IReaderStream stream, byte[] seperators, byte[] terminators)
        {
            Elements = new JsonCollectionElement[0];

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

            throw new Errors.JsonParseFailedException(stream.Position, "collection element has invalid terminal byte.");
        }

        internal override async Task<bool> FillAsync(IReaderStream stream, byte[] seperators, byte[] terminators)
        {
            Elements = new JsonCollectionElement[0];

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

            throw new Errors.JsonParseFailedException(stream.Position, "collection element has invalid terminal byte.");
        }

        private bool Parse(IReaderStream stream)
        {
            var args = new JsonObjectParseArgs()
            {
                ExternalObject = this,
                Stream = stream,
            };
            JsonObjectParser.Parse(args);

            if (args.InternalObject != null)
            {
                Elements = Elements.Append(new JsonCollectionElement() { Value = args.InternalObject });
            }

            return args.Handled;
        }

        private async Task<bool> ParseAsync(IReaderStream stream)
        {
            var args = new JsonObjectParseArgs()
            {
                ExternalObject = this,
                Stream = stream,
            };
            await JsonObjectParser.ParseAsync(args);

            if (args.InternalObject != null)
            {
                Elements = Elements.Append(new JsonCollectionElement() { Value = args.InternalObject });
            }

            return args.Handled;
        }
    }
}
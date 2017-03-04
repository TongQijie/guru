using System.Threading.Tasks;

using Guru.ExtensionMethod;
using Guru.Formatter.Internal;

namespace Guru.Formatter.Json
{
    internal class JsonDictionaryObject : JsonObject
    {
        public JsonDictionaryElement[] Elements { get; set; }

        internal override bool Fill(IReaderStream stream, byte[] seperators, byte[] terminators)
        {
            Elements = new JsonDictionaryElement[0];

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

            throw new Errors.JsonParseFailedException(stream.Position, "dictionary element has invalid terminal byte.");
        }

        internal override async Task<bool> FillAsync(IReaderStream stream, byte[] seperators, byte[] terminators)
        {
            Elements = new JsonDictionaryElement[0];

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

            throw new Errors.JsonParseFailedException(stream.Position, "dictionary element has invalid terminal byte.");
        }

        private bool Parse(IReaderStream stream)
        {
            var b = stream.SeekBytesUntilVisiableChar();
            if (b == JsonConstants.Right_Brace)
            {
                return true;
            }
            else if (b != JsonConstants.Double_Quotes)
            {
                throw new Errors.JsonParseFailedException(stream.Position, "dictionary name is invalid.");
            }

            var buf = stream.ReadBytesUntil(JsonConstants.Double_Quotes);
            if (buf == null)
            {
                buf = new byte[0];
            }

            if (!buf.HasLength())
            {
                throw new Errors.JsonParseFailedException(stream.Position, "dictionary element name is empty.");
            }

            stream.SeekBytesUntilEqual(JsonConstants.Colon);

            var args = new JsonObjectParseArgs()
            {
                ExternalObject = this,
                Stream = stream,
            };
            JsonObjectParser.Parse(args);

            if (args.InternalObject != null)
            {
                Elements = Elements.Append(new JsonDictionaryElement()
                {
                    Key = buf,
                    Value = args.InternalObject,
                });
            }

            return args.Handled;
        }

        private async Task<bool> ParseAsync(IReaderStream stream)
        {
            var b = await stream.SeekBytesUntilVisiableCharAsync();
            if (b == JsonConstants.Right_Brace)
            {
                return true;
            }
            else if (b != JsonConstants.Double_Quotes)
            {
                throw new Errors.JsonParseFailedException(stream.Position, "dictionary name is invalid.");
            }

            var buf = await stream.ReadBytesUntilAsync(JsonConstants.Double_Quotes);
            if (buf == null)
            {
                buf = new byte[0];
            }

            if (!buf.HasLength())
            {
                throw new Errors.JsonParseFailedException(stream.Position, "dictionary element name is empty.");
            }

            await stream.SeekBytesUntilEqualAsync(JsonConstants.Colon);

            var args = new JsonObjectParseArgs()
            {
                ExternalObject = this,
                Stream = stream,
            };
            await JsonObjectParser.ParseAsync(args);

            if (args.InternalObject != null)
            {
                Elements = Elements.Append(new JsonDictionaryElement()
                {
                    Key = buf,
                    Value = args.InternalObject,
                });
            }

            return args.Handled;
        }
    }
}
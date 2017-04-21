using System.Threading.Tasks;

namespace Guru.Formatter.Json
{
    internal static class JsonParser
    {
        public static void Parse(JsonParserArgs args)
        {
            int b = args.Stream.SeekBytesUntilVisiableChar();
            if (b == -1)
            {
                return;
            }

            byte[] seperators = null, terminators = null;
            if (args.ExternalObject != null)
            {
                if (args.ExternalObject is JObject)
                {
                    seperators = new byte[] { JsonConstants.Comma };
                    terminators = new byte[] { JsonConstants.Right_Brace };
                }
                else if (args.ExternalObject is JArray)
                {
                    seperators = new byte[] { JsonConstants.Comma };
                    terminators = new byte[] { JsonConstants.Right_Bracket };
                }
            }

            if (b == JsonConstants.Left_Brace)
            {
                args.InternalObject = new JObject();
            }
            else if (b == JsonConstants.Left_Bracket)
            {
                args.InternalObject = new JArray();
            }
            else if (b == JsonConstants.Right_Bracket)
            {
                args.Handled = true;
                return;
            }
            else
            {
                args.InternalObject = new JValue()
                {
                    EncompassedByQuote = b == JsonConstants.Double_Quotes,
                    Buffer = b == JsonConstants.Double_Quotes ? new byte[0] : new byte[1] { (byte)b },
                };
            }

            args.Handled = args.InternalObject.Fill(args.Stream, seperators, terminators);
        }

        public static async Task ParseAsync(JsonParserArgs args)
        {
            int b = await args.Stream.SeekBytesUntilVisiableCharAsync();
            if (b == -1)
            {
                return;
            }

            byte[] seperators = null, terminators = null;
            if (args.ExternalObject != null)
            {
                if (args.ExternalObject is JObject)
                {
                    seperators = new byte[] { JsonConstants.Comma };
                    terminators = new byte[] { JsonConstants.Right_Brace };
                }
                else if (args.ExternalObject is JArray)
                {
                    seperators = new byte[] { JsonConstants.Comma };
                    terminators = new byte[] { JsonConstants.Right_Bracket };
                }
            }

            if (b == JsonConstants.Left_Brace)
            {
                args.InternalObject = new JObject();
            }
            else if (b == JsonConstants.Left_Bracket)
            {
                args.InternalObject = new JArray();
            }
            else if (b == JsonConstants.Right_Bracket)
            {
                args.Handled = true;
                return;
            }
            else
            {
                args.InternalObject = new JValue()
                {
                    EncompassedByQuote = b == JsonConstants.Double_Quotes,
                    Buffer = b == JsonConstants.Double_Quotes ? new byte[0] : new byte[1] { (byte)b },
                };
            }

            args.Handled = await args.InternalObject.FillAsync(args.Stream, seperators, terminators);
        }
    }
}

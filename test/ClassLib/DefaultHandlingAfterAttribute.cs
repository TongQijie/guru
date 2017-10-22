using Guru.AspNetCore.Attributes;
using System;

namespace ClassLib
{
    public class DefaultHandlingAfterAttribute : HandlingAfterAttribute
    {
        public override HandlingResult Handle(string id, object args)
        {
            if (args != null)
            {
                Console.WriteLine($"id:{id}{Environment.NewLine}{args.ToString()}");
            }

            return HandlingResult.Succeed();
        }
    }
}

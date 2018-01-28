using System;
using System.Linq;
using Guru.AspNetCore.Attributes;

namespace ClassLib
{
    public class DefaultHandlingBeforeAttribute : HandlingBeforeAttribute
    {
        public override HandlingResult Handle(string id, Type returnType, params object[] args)
        {
            if (args != null)
            {
                Console.WriteLine($"id:{id}{Environment.NewLine}{string.Join(",", args.Select(x => x.ToString()))}");
            }
            
            return HandlingResult.Succeed();
        }
    }
}
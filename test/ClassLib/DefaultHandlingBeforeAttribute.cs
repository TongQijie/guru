using System;
using Guru.AspNetCore.Attributes;

namespace ClassLib
{
    public class DefaultHandlingBeforeAttribute : HandlingBeforeAttribute
    {
        public override HandlingBeforeResult Handle(params object[] args)
        {
            if (args != null)
            {
                foreach (var arg in args)
                {
                    Console.WriteLine(arg.ToString());
                }
            }
            
            return HandlingBeforeResult.Fail("error");
        }
    }
}
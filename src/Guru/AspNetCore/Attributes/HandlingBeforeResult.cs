namespace Guru.AspNetCore.Attributes
{
    public class HandlingBeforeResult
    {
        public bool Succeeded { get; private set; }

        public object ResultObject { get; private set; }

        public static HandlingBeforeResult Succeed()
        {
            return new HandlingBeforeResult()
            {
                Succeeded = true,
            };
        }

        public static HandlingBeforeResult Fail(object obj)
        {
            return new HandlingBeforeResult()
            {
                Succeeded = false,
                ResultObject = obj,
            };
        }
    }
}
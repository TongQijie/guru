namespace Guru.AspNetCore.Attributes
{
    public class HandlingResult
    {
        public bool Succeeded { get; private set; }

        public object ResultObject { get; private set; }

        public static HandlingResult Succeed()
        {
            return new HandlingResult()
            {
                Succeeded = true,
            };
        }

        public static HandlingResult Fail(object obj)
        {
            return new HandlingResult()
            {
                Succeeded = false,
                ResultObject = obj,
            };
        }
    }
}
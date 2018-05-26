namespace Guru.Restful
{
    public static class ResponseExtensions
    {
        private const int IllegalStatus = -99;

        public static T Ok<T>(this T response) where T : ResponseBase
        {
            if (response == null)
            {
                return response;
            }

            if (response.Head == null)
            {
                response.Head = new ResponseHead();
            }

            response.Head.Status = 0;
            response.Head.Message = "Ok";

            return response;
        }

        public static bool IsOk<T>(this T response) where T : ResponseBase
        {
            return !(response == null || response.Head == null || response.Head.Status != 0);
        }

        public static T Illegal<T>(this T response) where T : ResponseBase
        {
            return response.NotOk(IllegalStatus, "Illegal identity.");
        }

        public static T NotOk<T>(this T response) where T : ResponseBase
        {
            return response.NotOk(-1, "Unknown error.");
        }

        public static T NotOk<T>(this T response, int status) where T : ResponseBase
        {
            return response.NotOk(status, "Unknown error.");
        }

        public static T NotOk<T>(this T response, int status, string message) where T : ResponseBase
        {
            if (response == null)
            {
                return response;
            }

            if (response.Head == null)
            {
                response.Head = new ResponseHead();
            }

            response.Head.Status = status;
            response.Head.Message = message;

            return response;
        }

        public static bool IsIllegal<T>(this T response) where T : ResponseBase
        {
            return !(response == null || response.Head == null || response.Head.Status != IllegalStatus);
        }
    }
}

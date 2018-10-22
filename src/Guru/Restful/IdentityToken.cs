using System;

namespace Guru.Restful
{
    public class IdentityToken
    {
        public string Token { get; set; }

        public string UserId { get; set; }

        public DateTime Deadline { get; set; }
    }
}
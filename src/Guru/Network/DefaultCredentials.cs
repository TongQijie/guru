using System;
using System.Net;

namespace Guru.Network
{
    public class DefaultCredentials : ICredentials
    {
        private readonly string _Username;

        private readonly string _Password;

        private readonly string _Domain;

        public DefaultCredentials(string username, string password, string domain)
        {
            _Username = username;
            _Password = password;
            _Domain = domain;
        }

        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            return new NetworkCredential(_Username, _Password, _Domain);
        }
    }
}
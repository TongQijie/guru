﻿using System;
using System.Net;

namespace Guru.Network.Abstractions
{
    public interface IHttpManager
    {
        IHttpRequest Create();

        IHttpRequest Create(IWebProxy webProxy, bool ignoredCertificateValidation, TimeSpan? timeout);
    }
}
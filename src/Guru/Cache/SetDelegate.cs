using System;

namespace Guru.Cache
{
    public delegate void SetDelegate<T>(T value, DateTime expireTime);
}
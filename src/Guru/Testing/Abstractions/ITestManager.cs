﻿namespace Guru.Testing.Abstractions
{
    public interface ITestManager
    {
        bool TestModeEnabled { get; }

        void EnableTestMode();

        void DisableTestMode();

        void RunTest(string testClassName, string testMethodName);
    }
}
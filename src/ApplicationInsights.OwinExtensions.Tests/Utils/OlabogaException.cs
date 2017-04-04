using System;

namespace ApplicationInsights.OwinExtensions.Tests.Utils
{
    public class OlabogaException : Exception
    {
        public OlabogaException() : base("olaboga!") { }
    }
}
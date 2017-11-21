using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Moq;

namespace ApplicationInsights.OwinExtensions.Tests.Utils
{
    public class MockOwinContext : IOwinContext
    {
        public T Get<T>(string key)
        {
            if (!Environment.ContainsKey(key))
                return default(T);

            return (T)Environment[key];
        }

        public IOwinContext Set<T>(string key, T value)
        {
            Environment[key] = value;
            return this;
        }

        public IOwinRequest Request { get; set; }
        public IOwinResponse Response { get; set;  }
        public IAuthenticationManager Authentication { get; }
        public IDictionary<string, object> Environment { get; } = new Dictionary<string, object>();
        public TextWriter TraceOutput { get; set; }
    }

    public class MockOwinContextBuilder
    {
        private readonly MockOwinContext _context;

        public MockOwinContextBuilder()
        {
            _context = new MockOwinContext();
            WithDefaultRequest();
            WithDefaultResponse();
        }

        public MockOwinContextBuilder WithDefaultRequest() 
            => WithRequest(
                Mock.Of<IOwinRequest>(r =>
                    r.Method == "GET" &&
                    r.Path == new PathString("/path") &&
                    r.Uri == new Uri("http://google.com/path")
                ));

        public MockOwinContextBuilder WithRequest(IOwinRequest request)
        {
            _context.Request = request;
            return this;
        }

        public MockOwinContextBuilder WithDefaultResponse() 
            => WithResponse(Mock.Of<IOwinResponse>(r => r.StatusCode == 200));

        public MockOwinContextBuilder WithResponse(IOwinResponse response)
        {
            _context.Response = response;
            return this;
        }

        public MockOwinContext Build()
        {
            return _context;
        }
    }
}

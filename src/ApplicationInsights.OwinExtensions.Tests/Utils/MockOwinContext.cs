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
        private MockOwinContext _context;

        public MockOwinContextBuilder()
        {
            _context = new MockOwinContext();
            _context.Request = Mock.Of<IOwinRequest>();
            _context.Response = Mock.Of<IOwinResponse>();
        }

        public MockOwinContextBuilder WithRequest(IOwinRequest request)
        {
            _context.Request = request;
            return this;
        }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ApplicationInsights.OwinExtensions.Tests
{
    public class OperationContextTests
    {
        public OperationContextTests()
        {
            OperationContext.Set(null);
        }

        [Fact]
        public void Can_Store_Context()
        {
            OperationContext.Set(new OperationContext.Item("opid", "parentid"));

            OperationContext.Get().ShouldBeEquivalentTo(new OperationContext.Item("opid", "parentid"));
        }

        [Fact]
        public void Can_Establish_Scope()
        {
            using (new OperationContextScope("opid", "parentid"))
                OperationContext.Get().ShouldBeEquivalentTo(new OperationContext.Item("opid", "parentid"));

            OperationContext.Get().Should().BeNull();
        }

        [Fact]
        public void Can_Work_With_Subscope()
        {
            using (new OperationContextScope("opid", "op1"))
            {
                OperationContext.Get().ShouldBeEquivalentTo(new OperationContext.Item("opid", "op1"));
                using (new OperationContextScope("opid", "op2"))
                    OperationContext.Get().ShouldBeEquivalentTo(new OperationContext.Item("opid", "op2"));

                OperationContext.Get().ShouldBeEquivalentTo(new OperationContext.Item("opid", "op1"));
            }
            OperationContext.Get().Should().BeNull();
        }

        [Fact]
        public async Task Works_With_Async_Operations()
        {
            using (new OperationContextScope("opid", "op1"))
            {
                using (var client = new HttpClient())
                    await client.GetAsync("http://google.com").ConfigureAwait(false);

                OperationContext.Get().ShouldBeEquivalentTo(new OperationContext.Item("opid", "op1"));
            }

        }
    }
}

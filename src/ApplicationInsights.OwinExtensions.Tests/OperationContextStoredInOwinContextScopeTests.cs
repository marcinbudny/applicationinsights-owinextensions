using ApplicationInsights.OwinExtensions.Tests.Utils;
using FluentAssertions;
using Xunit;

namespace ApplicationInsights.OwinExtensions.Tests
{
    public class OperationContextStoredInOwinContextScopeTests
    {
        [Fact]
        public void Can_Store_Operation_Context_In_Owin_Environment()
        {
            var owinContext = new MockOwinContextBuilder().Build();

            using (new OperationContextScope("opid", "parentid"))
            using (new OperationContextStoredInOwinContextScope(owinContext))
            {
                owinContext.Get<string>(Consts.OperationIdContextKey).Should().Be("opid");
                owinContext.Get<string>(Consts.ParentOperationIdContextKey).Should().Be("parentid");
            }
        }

        [Fact]
        public void Can_Clear_Operation_Context_In_Owin_Environment()
        {
            var owinContext = new MockOwinContextBuilder().Build();

            using (new OperationContextScope("opid", "parentid"))
            using (new OperationContextStoredInOwinContextScope(owinContext)) { }

            owinContext.Get<string>(Consts.OperationIdContextKey).Should().BeNull();
            owinContext.Get<string>(Consts.ParentOperationIdContextKey).Should().BeNull();
        }

        [Fact]
        public void Can_Work_With_Subscopes()
        {
            var owinContext = new MockOwinContextBuilder().Build();

            using (new OperationContextScope("opid", "op1"))
            using (new OperationContextStoredInOwinContextScope(owinContext))
            {
                owinContext.Get<string>(Consts.OperationIdContextKey).Should().Be("opid");
                owinContext.Get<string>(Consts.ParentOperationIdContextKey).Should().Be("op1");

                using (new OperationContextScope("opid", "op2"))
                using (new OperationContextStoredInOwinContextScope(owinContext))
                {
                    owinContext.Get<string>(Consts.OperationIdContextKey).Should().Be("opid");
                    owinContext.Get<string>(Consts.ParentOperationIdContextKey).Should().Be("op2");
                }

                owinContext.Get<string>(Consts.OperationIdContextKey).Should().Be("opid");
                owinContext.Get<string>(Consts.ParentOperationIdContextKey).Should().Be("op1");
            }
            owinContext.Get<string>(Consts.OperationIdContextKey).Should().BeNull();
            owinContext.Get<string>(Consts.ParentOperationIdContextKey).Should().BeNull();
        }
    }
}

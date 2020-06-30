using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Http;

namespace TestWebsite.Controllers
{
    [RoutePrefix("api/values")]
    public class ValuesController : ApiController
    {
        [Route("")]
        public async Task<IEnumerable<string>> Get()
        {
            using (var client = new HttpClient())
            {
                await client.GetAsync("https://google.com");

                await client.GetAsync("https://bing.com");

                await client.GetAsync("https://yahoo.com");
            }

            return new[] { "value1", "value2" };
        }

        [Route("exception")]
        public Task GetException()
        {
            throw new Exception("olaboga!");
        }

        [Route("slow")]
        public async Task<string> GetSlowValue()
        {
            await Task.Delay(5000);
            
            return "slow action finished";
        }

    }
}

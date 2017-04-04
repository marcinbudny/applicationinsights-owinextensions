using System;
using System.Collections.Generic;
using System.Net.Http;
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

    }
}

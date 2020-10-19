using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApiJWT.Helpers;
using ApiJWT.Models;
using ApiJWT.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ApiJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly IAuthenticationServices _authenticationServices;

        public ValuesController(IOptions<AppSettings> appSettings, IAuthenticationServices authenticationServices)
        {
            _appSettings = appSettings.Value;
            _authenticationServices = authenticationServices;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("triggerfunctionapp")]
        public async Task<string> TriggerFunctionApp()
        {
            try
            {
                var client = new HttpClient();
                string url = $"{_appSettings.FunctionDomain}FunctionUser?name=userData";
                var result = await client.GetAsync(url);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    string jsonResult = result?.Content.ReadAsStringAsync().Result;
                    // var json = JsonConvert.DeserializeObject(jsonResult);
                    var users = JsonConvert.DeserializeObject<List<User>>(jsonResult);
                    _authenticationServices.AddUser(users);
                    return "Success trigger function app";
                }
            }
            catch(Exception ex)
            {
                return ex?.Message;
            }
            return "Fail trigger function app";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

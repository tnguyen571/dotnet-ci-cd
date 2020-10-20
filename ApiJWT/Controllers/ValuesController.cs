using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ApiJWT.Helpers;
using ApiJWT.Models;
using ApiJWT.Services;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;
        private static IQueueClient _queueClient;

        public ValuesController(IOptions<AppSettings> appSettings, IAuthenticationServices authenticationServices, ILogger<ValuesController> logger)
        {
            _appSettings = appSettings.Value;
            _authenticationServices = authenticationServices;
            _logger = logger;
            _queueClient = new QueueClient(_appSettings.ServiceBusConnectionString, _appSettings.QueueName);
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
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "dd26e2ab8642475095f0c61383fc00d0");
                client.DefaultRequestHeaders.Add("Ocp-Apim-Trace", "true");
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
            catch (Exception ex)
            {
                return ex?.Message;
            }
            return "Fail trigger function app";
        }

        // POST api/values
        [HttpGet("triggerservicesbus/{message}")]
        public async Task<string> TriggerFunctionAppServiceBus(string message)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                string messageBody = $"{message} at {dateTime.ToString()}";
                await SendMessagesAsync(messageBody);
                return $"Success trigger message {messageBody}";
            }
            catch (Exception ex)
            {
                return ex?.Message;
            }
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

        private async Task SendMessagesAsync(string messageBody)
        {
            try
            {
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                // Write the body of the message to the console.
                _logger.LogInformation($"Sending message: {messageBody}");

                // Send the message to the queue.
                await _queueClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}

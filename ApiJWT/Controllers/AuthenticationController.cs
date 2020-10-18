using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiJWT.Helpers;
using ApiJWT.Models;
using ApiJWT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        IAuthenticationServices _authenticationServices;

        public AuthenticationController(IAuthenticationServices authenticationServices)
        {
            _authenticationServices = authenticationServices;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _authenticationServices.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpGet]
        [CustomAuthorizeFilter]
        public IActionResult GetAll()
        {
            var users = _authenticationServices.GetAll();
            return Ok(users);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AssetManagementWebApi.Repositories.Entities;
using AssetManagementWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AssetManagementWebApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
       private readonly IAuthenticationService _authenticationService;
       public AuthenticationController( IAuthenticationService authenticationService)
        {
            _authenticationService=authenticationService;
        }
        
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromBody]LoginRequest request)
        {
            var result = await _authenticationService.Authenticate(request);
            if(result == "Wrong username or pass") return BadRequest("Wrong username or pass");
            if(result == "Account Disabled")return BadRequest("Account Disabled");
            return Ok(result); 
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("ChangePass")]
        public async Task<ActionResult<string>> FirstLogin([FromBody]ChangePasswordRequest request)
        {

            var result = await _authenticationService.FirstLogIn(request);
            
            if(result =="Ok") return Ok(result);
            return BadRequest(result);
        }
    }
}
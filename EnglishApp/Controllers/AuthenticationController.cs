using EnglishApp.Model;
using EnglishApp.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace EnglishApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationRepository _authentication;
        public AuthenticationController(AuthenticationRepository authenticationRepository) {
        _authentication = authenticationRepository;
        
        
        } 
        [HttpPost("signupsendotp")]
        public async Task<IActionResult> SignupSentOtp(SignUpModel signUpModel)
        {
            var result = await _authentication.SignUpSentOtp(signUpModel);
            return (result.Success ? Ok(result) : BadRequest(result)); 
        }

        [HttpPost("signupreceiveotp")]
        public async Task<IActionResult> SignUpReceivedOtp(ConfirmOtpModel confirm)
        {
            var result = await _authentication.SignUpReceiveOtp(confirm);
            return (result.Success ? Ok(result) : BadRequest(result));
        }





        }
    
}

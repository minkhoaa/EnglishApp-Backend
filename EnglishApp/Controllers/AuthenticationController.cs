using EnglishApp.Model;
using EnglishApp.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var result = await _authentication.Login(loginModel);  
            return (result).Success ? Ok(result) : NotFound(result);
        }
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(TokenModel token)
        {
            var result = await _authentication.AuthenticateAsync(token);    
            return (result ==  null) ? NotFound(result) : Ok(result);
        }
        [HttpPost("sendresetpasswordcode")]
        public async Task<IActionResult> SendResetPasswordCode(ForgotPasswordRequest forgotPasswordRequest)
        {
            var result = await _authentication.ForgotPassword(forgotPasswordRequest);
            return (result == null) ? NotFound(result) : Ok(result);
        }
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var result = await _authentication.ResetPassword(resetPasswordRequest);
            return (result == null) ? NotFound(result) : Ok(result);
        }
    }
    
}

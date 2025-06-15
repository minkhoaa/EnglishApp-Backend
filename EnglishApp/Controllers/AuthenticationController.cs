using EnglishApp.Model;
using EnglishApp.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;

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
            return (!result.Success) ? NotFound(result) : Ok(result);
        }
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var result = await _authentication.ResetPassword(resetPasswordRequest);
            return (!result.Success) ? NotFound(result) : Ok(result);
        }
        [HttpGet("signin-google")]
        public IActionResult inGoogle(string returnUrl = "/api/Authentication/profile")
        {
            var properties = new AuthenticationProperties { RedirectUri = returnUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/auth/signin-google");
            }

            // Lấy thông tin từ claim của Google
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
         
            var response = await _authentication.LoginWithGoogleAsync(User.Claims);

            return Ok(response);
        }
    }
    
}

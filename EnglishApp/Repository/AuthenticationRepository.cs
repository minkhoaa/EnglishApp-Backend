using EnglishApp.Data;
using EnglishApp.Model;
using System.Diagnostics.Eventing.Reader;
using System.IO.Pipelines;
using System.Security.Policy;

namespace EnglishApp.Repository
{
    public interface AuthenticationRepository
    {
        public Task<User> AuthenticateAsync(TokenModel token);
        public Task<ApiResponse> SignUpSentOtp(SignUpModel signupModel);
        public Task<ApiResponse> SignUpReceiveOtp(ConfirmOtpModel confirmOtpModel);

        public Task<ApiResponse> Login(LoginModel loginModel);

    }
}

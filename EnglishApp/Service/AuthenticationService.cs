using EnglishApp.Data;
using EnglishApp.Model;
using EnglishApp.Repository;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace EnglishApp.Service
{
    public class AuthenticationService : AuthenticationRepository
    {
        private readonly EnglishAppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IFluentEmail _fluentEmail;
        private readonly IConfiguration _configuration;
        public AuthenticationService(EnglishAppDbContext englishAppDbContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            IFluentEmail fluentEmail,
            IConfiguration configuration


            ) {
            _configuration = configuration;
            _context = englishAppDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _fluentEmail = fluentEmail;
        }
        public Task<User> AuthenticateAsync(TokenModel token)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> Login(LoginModel loginModel)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> SignUpReceiveOtp(ConfirmOtpModel confirmOtpModel)
        {
           var otp = await _context.TempOtps.AsNoTracking().FirstOrDefaultAsync(x=>x.Email == confirmOtpModel.Email);
            if (otp == null) {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng nhập đúng email"
                };
                
            }
            if (otp.Otp != confirmOtpModel.Otp)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Otp sai, vui lòng nhập lại"
                };
            }
            if (otp.Expiration < DateTime.UtcNow) {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Otp đã hết hạn"
                };
            }
            using var transction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    UserName = otp.Email,
                    Email = otp.Email,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, otp.Password!);
                if (!result.Succeeded)
                {
                    await transction.RollbackAsync();
                    return new ApiResponse
                    {

                        Success = false,
                        Message = "Đăng kí tài khoản không thành công"
                    };
                }
                if (!await _roleManager.RoleExistsAsync(RoleModel.User))
                {
                    var role = new Role { Name = RoleModel.User };
                    await _roleManager.CreateAsync(role);
                }
                await _userManager.AddToRoleAsync(user, RoleModel.User);
                await _context.TempOtps
                    .Where(x => x.Email == otp.Email)
                    .ExecuteDeleteAsync();
                await _context.SaveChangesAsync();
                await transction.CommitAsync();
                return new ApiResponse
                {
                    Success = true,
                    Message = "Tạo tài khoản thành công",
                    Data = new { user.Id, user.Email, user.UserName }
                };
            }catch
            {
                await transction.RollbackAsync();
                return new ApiResponse
                {
                    Success = false,
                    Message = "Failed to sign up user with otp for email " + confirmOtpModel.Email
                };
            }



        }


        public async Task<ApiResponse> SignUpSentOtp(SignUpModel signupModel)
        {
            var exisitedUuser = await _userManager.FindByEmailAsync(signupModel.Email);
            if (exisitedUuser != null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Email này đã được sử dụng",
                    Data = null!
                };
            }
            if (signupModel.Password != signupModel.ConfirmPassword)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Xác nhận mật khẩu sai"
                };
            }
            var existedOtp = await _context.TempOtps.AnyAsync(x => x.Email == signupModel.Email);
            string otp = new Random().Next(100000, 999999).ToString();

            if (!existedOtp)
            {
                await _context.AddAsync(new TempOtp
                {
                    Email = signupModel.Email,
                    Otp = otp,
                    Expiration = DateTime.UtcNow.AddMinutes(5),
                    Password = signupModel.Password,
                });
            }
            else
            {
                await _context.TempOtps
                    .Where(x => x.Email == signupModel.Email)
                    .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Otp, otp)
                    .SetProperty(x => x.Expiration, DateTime.UtcNow.AddMinutes(5))
                    );
            }
            await _context.SaveChangesAsync();
            await _fluentEmail
                .To(signupModel.Email)
                .Subject("Mã OTP xác thực email")
                .Body($"<p>Mã OTP của bạn là: <strong>{otp}</strong> (hiệu lực trong 5 phút).</p>", true)
                .SendAsync()
                ;
            return new ApiResponse
            {
                Success = true,
                Message = "Đã gửi otp thành công"
            };


        }
    

     
    }
}

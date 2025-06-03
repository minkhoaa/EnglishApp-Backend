using Castle.Components.DictionaryAdapter.Xml;
using EnglishApp.Data;
using EnglishApp.Model;
using EnglishApp.Repository;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        public async Task<User> AuthenticateAsync(TokenModel token)
        {
            if (string.IsNullOrEmpty(token.AccessToken)) return null!;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!);

            try
            {
                 tokenHandler.ValidateToken(token.AccessToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["JWT:ValidIssuer"],
                    ValidAudience = _configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validToken
                );
                var jwtToken = (JwtSecurityToken)validToken;
                var email = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email)) return null!;
                var user = await _userManager.FindByEmailAsync(email);
                return (user == null) ? null! : user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<ApiResponse> ForgotPassword(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return new ApiResponse { Success = false, Message = "Người dùng không tồn tại" };
            var otp = new Random().Next(100000, 999999).ToString();
            var existedOtp = await _context.TempOtps.AnyAsync(x => x.Email == request.Email);
            if (!existedOtp)
            {
                var tempOtp = new TempOtp
                {
                    Email = request.Email,
                    Otp = otp,
                    Password = string.Empty,
                    Expiration = DateTime.UtcNow.AddMinutes(5),
                };

                await _context.TempOtps.AddAsync(tempOtp);
            }
            else
            {
                await _context.TempOtps
                    .Where(x => x.Email == request.Email)
                    .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Otp, otp)
                    .SetProperty(x => x.Expiration, DateTime.UtcNow.AddMinutes(5))
                    );
            }
            await _context.SaveChangesAsync();
            try
            {
                await _fluentEmail
                    .To(request.Email)
                    .Subject("Mã xác nhận đặt lại mật khẩu")
                    .Body($"Mã OTP của bạn là: <strong>{otp}</strong>. Mã này sẽ hết hạn sau 5 phút.", true)
                    .SendAsync();
                return new ApiResponse { Success = true, Message = "Gửi mã đặt lại mật khẩu thành công" };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = "Gửi mã đặt lại mật khẩu thất bại, mã lỗi: " + ex.Message };
            }
        }

        public async Task<ApiResponse> Login(LoginModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user == null) {
                return new ApiResponse
                {
                    Success =false,
                    Message = "Không tìm thầy tài khoản người dùng"
                };
            }

            var passswordValid = await _userManager.CheckPasswordAsync(user, loginModel.Password);
            if (passswordValid == false) { return new ApiResponse { Success = false, Message = "Sai mật khẩu" }; }

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, loginModel.Email),
                new Claim(JwtRegisteredClaimNames.Email, loginModel.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256Signature)
                );
            return new ApiResponse { Success = true, Message = "Đăng nhập thành công", Data = new JwtSecurityTokenHandler().WriteToken(token) };
        }

        public async Task<ApiResponse> ResetPassword(ResetPasswordRequest resetPasswordModel)
        {
             var otp = await _context.TempOtps.AsNoTracking().FirstOrDefaultAsync(x=>x.Email==resetPasswordModel.Email);
            if (otp == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng nhập đúng email"
                };

            }
            if (otp.Otp != resetPasswordModel.ResetCode)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Otp sai, vui lòng nhập lại"
                };
            }
            if (otp.Expiration < DateTime.UtcNow)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Otp đã hết hạn"
                };
            }
            var user = await _userManager.FindByEmailAsync(otp.Email);
            if (user == null) { return new ApiResponse { Success = false, Message = "Người dùng không tồn tại" }; }
            var resetPassworkToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetPassworkToken, resetPasswordModel.NewPassword);
            if (result.Succeeded) {
                return new ApiResponse { Success = true, Message = "Thay đổi mật khẩu thành công" };
            }
            return new ApiResponse() { Success = false, Message = "Thay đổi mật khẩu thất bại" };

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

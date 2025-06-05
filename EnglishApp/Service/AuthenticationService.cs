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
using System.Linq.Expressions;
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
                var emailBody = $@"
                                <p>Chào {request.Email},</p>
                                <p>Mã OTP của bạn là: <strong>{otp}</strong></p>
                                <p>Mã này sẽ hết hạn sau 5 phút.</p>
                                <p>Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>";
                await _fluentEmail
                    .To(request.Email)
                    .Subject("Mã xác nhận đặt lại mật khẩu")
                    .Body(emailBody, isHtml:true)
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
            // 1. Lấy bản ghi TempOtp theo email
            var tempOtpEntity = await _context.TempOtps
                .FirstOrDefaultAsync(x => x.Email == confirmOtpModel.Email);

            if (tempOtpEntity == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Không tìm thấy yêu cầu đăng ký. Vui lòng làm lại từ đầu."
                };
            }

            // 2. Kiểm tra OTP có đúng không
            if (tempOtpEntity.Otp != confirmOtpModel.Otp)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "OTP không chính xác. Vui lòng nhập lại."
                };
            }

            // 3. Kiểm tra có hết hạn không
            if (tempOtpEntity.Expiration < DateTime.UtcNow)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "OTP đã hết hạn. Vui lòng yêu cầu lại."
                };
            }


            try
            {
                var user = new User
                {
                    UserName = tempOtpEntity.Email,
                    Email = tempOtpEntity.Email,
                    EmailConfirmed = true
                };

                var plainPassword = tempOtpEntity.Password;

                var createResult = await _userManager.CreateAsync(user, plainPassword!);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Khởi tạo tài khoản thất bại: " + errors
                    };
                }

                // 5. Gán Role "User"
                const string defaultRole = "User";
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    await _roleManager.CreateAsync(new Role { Name = defaultRole });
                }
                await _userManager.AddToRoleAsync(user, defaultRole);
                var userInfo = new UserInfo
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Birthday = new DateTime(1970, 1, 1).ToUniversalTime()
                };
                await _context.UserInfo.AddAsync(userInfo);

                _context.TempOtps.Remove(tempOtpEntity);
                await _context.SaveChangesAsync();

                // 7. Trả về thông tin user
                return new ApiResponse
                {
                    Success = true,
                    Message = "Đăng ký thành công!",
                    Data = new { user.Id, user.Email, user.UserName }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse {Success =false, Message ="Tạo mới tài khoản thất bại, mã lỗi: "+  ex.Message};
            }
        
        }

        public async Task<ApiResponse> SignUpSentOtp(SignUpModel signupModel)
        {
            // 1. Kiểm tra password & confirmPassword có khớp không
            if (signupModel.Password != signupModel.ConfirmPassword)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Mật khẩu và xác nhận mật khẩu không khớp."
                };
            }

            // 2. Kiểm tra email đã tồn tại trong Identity chưa
            var existingUser = await _userManager.FindByEmailAsync(signupModel.Email);
            if (existingUser != null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Email này đã được sử dụng."
                };
            }

            var otp = new Random().Next(100000, 999999).ToString();


            var existedOtp = await _context.TempOtps.AnyAsync(x => x.Email == signupModel.Email);
            if (!existedOtp)
            {
                var tempOtpEntity = new TempOtp
                {
                    Email = signupModel.Email,
                    Otp = otp,
                    Password = signupModel.Password,
                    Expiration = DateTime.UtcNow.AddMinutes(5)
                };
                await _context.TempOtps.AddAsync(tempOtpEntity);
            }
            else
            {
                await _context.TempOtps
                    .Where(x => x.Email == signupModel.Email)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Otp, otp)
                        .SetProperty(x => x.Password, signupModel.Password)
                        .SetProperty(x => x.Expiration, DateTime.UtcNow.AddMinutes(5))
                    );
            }

            await _context.SaveChangesAsync();

            // 6. Gửi OTP qua email
            var emailBody = $@"
                <p>Chào {signupModel.Email},</p>
                <p>Mã OTP của bạn là: <strong>{otp}</strong></p>
                <p>Mã này sẽ hết hạn sau 5 phút.</p>
                <p>Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>";

            try
            {
                await _fluentEmail
                    .To(signupModel.Email)
                    .Subject("Mã OTP xác thực đăng ký")
                    .Body(emailBody, isHtml: true)
                    .SendAsync();

                return new ApiResponse
                {
                    Success = true,
                    Message = "Đã gửi mã OTP đến email của bạn. Vui lòng kiểm tra để tiếp tục đăng ký."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Gửi email thất bại: " + ex.Message
                };
            }
        }

    }
}

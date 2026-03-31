using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace KASHOP.BLL.Service
{
    public class AuthenicationService : IAuthenicationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenicationService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }



        public async Task<RegisterResponse> RegisterAsync(RegisterRequest Request)
        {
            //حولت الdto to domain model
            var user = Request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, Request.Password);

            //foreach (var error in result.Errors)
            //{
            //    Console.WriteLine(error.Description);
            //}

            if (!result.Succeeded)
            {
                return new RegisterResponse
                {
                    Success = false,
                    messsage = "Error",
                    Errors = result.Errors.Select(p => p.Description).ToList()
                };
            }

            await _userManager.AddToRoleAsync(user, "User");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //بحول الرموز عشان يقدر يتعامل معها
            token = Uri.EscapeDataString(token);
            //هون لما اليورز يكبس على confirm email رح يروح على هذا الرابط عشان يتاكد من الايميل والي هون رستخدم فنكشن confirm email اللي عملتها في ال controller
            var emailUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/account/confirm?token={token}&userId={user.Id}";
            await _emailSender.SendEmailAsync(
              user.Email,
              "Welcome",
              $"<h1>Welcome {user.UserName}</h1>" +
              $"<a href='{emailUrl}'>Confirm Email</a>"
            );



            return new RegisterResponse
            {
                Success = true,
                messsage = "success"
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return new LoginResponse()
                {
                    Success = false,
                    messsage = "Invalid Email"
                };
            }
            //بدي افحص اذا الايميل متاكد منه ولا لا
            if (await _userManager.IsEmailConfirmedAsync(user) == false)
            {
                return new LoginResponse()
                {
                    Success = false,
                    messsage = "Please confirm your email"
                };
            }
            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                return new LoginResponse()
                {
                    Success = false,
                    messsage = "Invalid password"
                };
            }
            return new LoginResponse()
            {
                Success = true,
                messsage = "Login successful",
                AccessToken = await GenerateAccessToken(user)
            };

        }

        /*
         الtoken بكون فيها هاي الشغلات :
         issuer: localhost 7132
         audience: localhost 7132
         security key: secret key اللي انا حطيتها في الappsettings
         */

        private async Task<string> GenerateAccessToken(ApplicationUser user)
        {
            //هون انشئنا الpayload اللي رح نستخدمه في الtoken
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email),
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //هون انشئنا الtoken الي هي كلمة السر
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: userClaims,
                //بعد 15 دقيقة رح تنتهي صلاحية الtoken يعني رح يعمل لييوزر تسجيل خروج تلقائي
                expires: DateTime.Now.AddDays(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<bool> ConfirmEmailAsync(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return false;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return false;
            }
            return true;
        }

        public async Task<ForgotPasswordResponse> RequestPasswordResetAsync(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return new ForgotPasswordResponse
                {
                    Success = false,
                    messsage = "Invalid Email"
                };
            }
            //هون عشان اعمل الكود الي بدي ارسله للايميل
            var random=new Random();
            var code=random.Next(100000,999999).ToString();
            user.CodeResetPassword= code;
            //بعد 15 دقيقة رح تنتهي صلاحية الكود
            user.PasswordResetCodeExpiry= DateTime.Now.AddMinutes(15);

            await _userManager.UpdateAsync(user);

            await _emailSender.SendEmailAsync(
                user.Email,
                "Password Reset",
                $"<p>Your password reset code is:{code}</p>"
            );

            return new ForgotPasswordResponse
            {
                Success = true,
                messsage = "Password reset code sent to your email"
            };

        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "Email Not Found"
                };
            }
            else if (user.CodeResetPassword != request.Code)
            {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "invalid code"
                };
            }
            else if (user.PasswordResetCodeExpiry < DateTime.UtcNow)
            {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "Code Expired"
                };
            }

            var isSamePassword = await _userManager.CheckPasswordAsync(user, request.NewPassword);

            if (isSamePassword)
            {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "New Password Must Be Different From Old Password"
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //هاي اشي جاهز بيعمل تشفير اللباسوورد وبتعمله ابديت في الداتابيز
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (!result.Succeeded)
            {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "password reset failed"
                };
            }

            await _emailSender.SendEmailAsync(request.Email, "change password", $"<p> your password is changed </p>");

            return new ResetPasswordResponse()
            {
                Success = true,
                Message = "password reset successfully"
            };
        }


    }
}

using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IAuthenicationService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest Request);
        Task<LoginResponse> LoginAsync(LoginRequest Request);

        Task<bool> ConfirmEmailAsync(string token, string userId);

        Task<ForgotPasswordResponse> RequestPasswordResetAsync(ForgotPasswordRequest request);

        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request);
    }
}

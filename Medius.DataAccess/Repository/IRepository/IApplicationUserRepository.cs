using Medius.Model;
using Medius.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medius.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);
        Task<AuthenticateResponse> ChangePassword(ChangePassword model);
        ApplicationUser Register(RegisterRequest model, string origin);
        void VerifyEmail(string token);
        string ForgotPassword(ForgotPasswordRequest model, string origin);
        void ValidateResetToken(ValidateResetTokenRequest model);
        void ResetPassword(ResetPasswordRequest model);
        IEnumerable<AccountResponse> GetAll();
        AccountResponse GetById(string id);
        AccountResponse Create(CreateRequest model);
        AccountResponse Update(string id, UpdateRequest model);
        void Delete(string id);
    }
}

﻿using AutoMapper;
using Medius.DataAccess.Data;
using Medius.DataAccess.Repository.IRepository;
using Medius.Model;
using Medius.Model.ViewModels;
using Medius.Models.Enums;
using Medius.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace Medius.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
            private readonly IHostingEnvironment _env;
        private readonly EmailSender _emailService;

        public ApplicationUserRepository(ApplicationDbContext db, IMapper mapper, EmailSender emailService, IHostingEnvironment env) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _emailService = emailService;
            _env = env;
        }


        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var account = _db.Users.SingleOrDefault(x => x.Email == model.Email);

            if (account == null || !account.IsVerified || !BC.Verify(model.Password, account.PasswordHash))
                throw new AppException("Email or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = generateJwtToken(account);
            var refreshToken = generateRefreshToken(ipAddress);
            account.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from account
            removeOldRefreshTokens(account);

            // save changes to db
            _db.Update(account);
            _db.SaveChanges();

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;
            return response;
        }

        public async Task<AuthenticateResponse> ChangePassword(ChangePassword model)
        {
            var account = await _db.Users.SingleOrDefaultAsync(x => x.Id == model.id);

            if (account == null || !account.IsVerified || !BC.Verify(model.password, account.PasswordHash))
                throw new AppException("Email or password is incorrect");

            account.PasswordHash = BC.HashPassword(model.newPassword);
            account.Updated = DateTime.UtcNow;
            // save changes to db
            _db.Update(account);
            _db.SaveChanges();

            var response = _mapper.Map<AuthenticateResponse>(account);
            return response;
        }

    public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var (refreshToken, account) = getRefreshToken(token);

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            account.RefreshTokens.Add(newRefreshToken);

            removeOldRefreshTokens(account);

            _db.Update(account);
            _db.SaveChanges();

            // generate new jwt
            var jwtToken = generateJwtToken(account);

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = newRefreshToken.Token;
            return response;
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var (refreshToken, account) = getRefreshToken(token);

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _db.Update(account);
            _db.SaveChanges();
        }

        public ApplicationUser Register(RegisterRequest model, string origin)
        {

            // map model to new account object
            model.UserName = model.FirstName + ' ' + model.LastName;
            var account = _mapper.Map<ApplicationUser>(model);

            // first registered account is an admin
            var isFirstAccount = _db.ApplicationUsers.Count() == 0;
            account.Role = isFirstAccount ? Role.Admin : model.Role;
            account.Created = DateTime.UtcNow;
            account.VerificationToken = randomTokenString();

            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);

            // save account
            _db.ApplicationUsers.Add(account);
            _db.SaveChanges();
            // send email
            sendVerificationEmail(account, origin);
            return account;

            
        }

        public void VerifyEmail(string token)
        {
            var account = _db.ApplicationUsers.SingleOrDefault(x => x.VerificationToken == token);

            if (account == null) throw new AppException("Verification failed");

            account.Verified = DateTime.UtcNow;
            account.VerificationToken = null;

            _db.ApplicationUsers.Update(account);
            _db.SaveChanges();
        }

        public string ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = _db.ApplicationUsers.SingleOrDefault(x => x.Email == model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return null;

            // create reset token that expires after 1 day
            account.ResetToken = randomTokenString();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            _db.ApplicationUsers.Update(account);
            _db.SaveChanges();
            // Email Service
            //EmailService service = new EmailService();
            string path = Path.Combine(_env.WebRootPath, "VerificationCode.html");
            string content = System.IO.File.ReadAllText(path);
            content = content.Replace("{{resetToken}}", account.ResetToken);
            content = content.Replace("{{currentYear}}", DateTime.Now.Year.ToString());
            string subject = "Hello and Welcome to Medius !";

            _emailService.SendEmailAsync(model.Email, subject, content);

            return account.ResetToken;
        }


        public void ValidateResetToken(ValidateResetTokenRequest model)
        {
            var account = _db.ApplicationUsers.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
                throw new AppException("Invalid token");
        }

        public void ResetPassword(ResetPasswordRequest model)
        {
            var account = _db.ApplicationUsers.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
                throw new AppException("Invalid token");

            // update password and remove reset token
            account.PasswordHash = BC.HashPassword(model.Password);
            account.PasswordReset = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpires = null;

            _db.ApplicationUsers.Update(account);
            _db.SaveChanges();
        }

        public IEnumerable<AccountResponse> GetAll()
        {
            var accounts = _db.ApplicationUsers;
            return _mapper.Map<IList<AccountResponse>>(accounts);
        }

        public AccountResponse GetById(string id)
        {
            var account = getAccount(id);
            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Create(CreateRequest model)
        {
            // validate
            if (_db.ApplicationUsers.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            // map model to new account object
            var account = _mapper.Map<ApplicationUser>(model);
            account.Created = DateTime.UtcNow;
            account.Verified = DateTime.UtcNow;

            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);

            // save account
            _db.ApplicationUsers.Add(account);
            _db.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Update(string id, UpdateRequest model)
        {
            var account = getAccount(id);

            // validate
            if (account.Email != model.Email && _db.ApplicationUsers.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already taken");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                account.PasswordHash = BC.HashPassword(model.Password);

            // copy model to account and save
            _mapper.Map(model, account);
            account.Updated = DateTime.UtcNow;
            _db.ApplicationUsers.Update(account);
            _db.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public void Delete(string id)
        {
            var account = getAccount(id);
            _db.ApplicationUsers.Remove(account);
            _db.SaveChanges();
        }
        public async Task<AccountResponse> ArchiveUser(string id)
        {
            var account = getAccount(id);
            account.Active = false;
            await _db.SaveChangesAsync();
            var response = _mapper.Map<AccountResponse>(account);
            return response;
        }

        // helper methods

        private ApplicationUser getAccount(string id)
        {
            var account = _db.ApplicationUsers.Find(id);
            if (account == null) throw new KeyNotFoundException("Account not found");
            return account;
        }

        private (RefreshToken, ApplicationUser) getRefreshToken(string token)
        {
            var account = _db.ApplicationUsers.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (account == null) throw new AppException("Invalid token");
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive) throw new AppException("Invalid token");
            return (refreshToken, account);
        }

        private string generateJwtToken(ApplicationUser account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("dbd1d614-aeab-468f-8b71-ee8e1c006da6");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = randomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private void removeOldRefreshTokens(ApplicationUser account)
        {
            account.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(2) <= DateTime.UtcNow);
        }

        private string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private void sendVerificationEmail(ApplicationUser account, string origin)
        {
            string message;
            string subject;
            string path;
            string content;
            if (account.Role == Role.User)
            {
                subject = "Email Verification";
                path = Path.Combine(_env.WebRootPath, "WelcomeEmail.html");
                content = System.IO.File.ReadAllText(path);
                if (!string.IsNullOrEmpty(origin))
                {
                    var resetToken = $"{origin}/api/account/VerifyEmail?token={account.VerificationToken}";
                    message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{resetToken}"">{resetToken}</a></p>";
                    content = content.Replace("{{resetToken}}", resetToken);
                    content = content.Replace("{{verificationToken}}", account.VerificationToken);
                    content = content.Replace("{{message}}", message);
                    content = content.Replace("{{currentYear}}", DateTime.Now.Year.ToString());
                }
                else
                {
                    message = $@"<p>Please use the below token to verify your email address with the <code>/api/accounts/VerifyEmail</code> api route:</p>
                             <p><code>{account.VerificationToken}</code></p>";
                    content = content.Replace("{{resetToken}}", account.VerificationToken);
                    content = content.Replace("{{message}}", message);
                    content = content.Replace("{{currentYear}}", DateTime.Now.Year.ToString());

                }
            }
            else
            {
                subject = "Invite Email";
                path = Path.Combine(_env.WebRootPath, "InviteEmail.html");
                content = System.IO.File.ReadAllText(path);
                if (!string.IsNullOrEmpty(origin))
                {
                    var resetToken = $"{origin}/api/account/VerifyEmail?token={account.VerificationToken}";
                    message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{resetToken}"">{resetToken}</a></p>";
                    content = content.Replace("{{resetToken}}", resetToken);
                    content = content.Replace("{{verificationToken}}", account.VerificationToken);
                    content = content.Replace("{{message}}", message);
                    content = content.Replace("{{currentYear}}", DateTime.Now.Year.ToString());
                }
                else
                {
                    message = $@"<p>Please use the below token to verify your email address with the <code>/api/accounts/verify-email</code> api route:</p>
                             <p><code>{account.VerificationToken}</code></p>";

                    content = content.Replace("{{resetToken}}", account.VerificationToken);
                    content = content.Replace("{{message}}", message);
                    content = content.Replace("{{currentYear}}", DateTime.Now.Year.ToString());

                }
            }
            _emailService.SendEmailAsync(account.Email, subject, content);

        }

        //private void sendAlreadyRegisteredEmail(string email, string origin)
        //{
        //    string message;
        //    if (!string.IsNullOrEmpty(origin))
        //        message = $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>";
        //    else
        //        message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

        //    _emailService.SendEmailAsync(
        //        email,
        //        subject: "Sign-up Verification API - Email Already Registered",
        //        $@"<h4>Email Already Registered</h4>
        //                 <p>Your email <strong>{email}</strong> is already registered.</p>
        //                 {message}"
        //    );
        //}

        private void sendPasswordResetEmail(ApplicationUser account, string origin)
        {
            string message;
            string subject = "Reset Password";
            string path = Path.Combine(_env.WebRootPath, "PasswordResetEmail.html");
            string content = System.IO.File.ReadAllText(path);
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/api/account/reset-password?token={account.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
                content = content.Replace("{{resetToken}}", account.ResetToken);
                content = content.Replace("{{message}}", message);
                content = content.Replace("{{currentYear}}", DateTime.Now.Year.ToString());

            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{account.ResetToken}</code></p>";
                content = content.Replace("{{resetToken}}", account.ResetToken);
                content = content.Replace("{{message}}", message);
                content = content.Replace("{{currentYear}}", DateTime.Now.Year.ToString());

            }
            _emailService.SendEmailAsync(account.Email, subject, content);

        }

        public ApplicationUser GetUser(string id)
        {
            var account = getAccount(id);
            return account;
        }
    }
}

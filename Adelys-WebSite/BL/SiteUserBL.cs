using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Adelys_WebSite.BL
{
    public class SiteUserBL : ISiteUserBL
    {
        public const string SecurityStampClaim = "security_stamp";
        public const string MustChangePasswordClaim = "must_change_password";

        private const int MaxFailedLogins = 5;
        private const int GeneratedPasswordLength = 16;
        private const string PasswordAlphabet = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ23456789";
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

        private readonly ISiteUserRepository _repository;
        private readonly IPasswordHasher<SiteUser> _passwordHasher;

        public SiteUserBL(ISiteUserRepository repository, IPasswordHasher<SiteUser> passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public List<SiteUser> GetAllUsers()
        {
            return _repository.GetAll();
        }

        public SiteUser? GetUser(int id)
        {
            return _repository.GetById(id);
        }

        public SiteUser? GetUser(string username)
        {
            return _repository.GetByUsername(username);
        }

        /// <summary>
        /// Vérifie les identifiants et bloque le compte après plusieurs échecs
        /// </summary>
        /// <returns>L'utilisateur connecté, ou null sans préciser la raison de l'échec</returns>
        public SiteUser? Authenticate(string username, string password)
        {
            SiteUser? user = _repository.GetByUsername(username);
            if (user == null)
            {
                // Même temps de réponse qu'un compte existant
                _passwordHasher.HashPassword(new SiteUser(), password);
                return null;
            }

            if (!user.IsActive || user.LockoutEndUtc > DateTime.UtcNow)
            {
                return null;
            }

            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                user.FailedLoginCount++;
                if (user.FailedLoginCount >= MaxFailedLogins)
                {
                    user.FailedLoginCount = 0;
                    user.LockoutEndUtc = DateTime.UtcNow.Add(LockoutDuration);
                }
                _repository.Update(user);
                return null;
            }

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, password);
            }
            user.FailedLoginCount = 0;
            user.LockoutEndUtc = null;
            user.LastLoginUtc = DateTime.UtcNow;
            _repository.Update(user);
            return user;
        }

        /// <summary>
        /// Crée un compte avec un mot de passe généré, à changer à la première connexion
        /// </summary>
        /// <returns>Le mot de passe en clair, il n'est stocké nulle part</returns>
        public string CreateUser(string username, string role)
        {
            string password = GeneratePassword();
            SiteUser user = new SiteUser
            {
                Username = username,
                Role = role,
                CreatedAtUtc = DateTime.UtcNow,
                SecurityStamp = GenerateSecurityStamp()
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            _repository.Create(user);
            return password;
        }

        public string ResetPassword(SiteUser user)
        {
            string password = GeneratePassword();
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            user.MustChangePassword = true;
            user.FailedLoginCount = 0;
            user.LockoutEndUtc = null;
            user.SecurityStamp = GenerateSecurityStamp();
            _repository.Update(user);
            return password;
        }

        public bool ChangePassword(SiteUser user, string currentPassword, string newPassword)
        {
            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword) == PasswordVerificationResult.Failed)
            {
                return false;
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            user.MustChangePassword = false;
            user.SecurityStamp = GenerateSecurityStamp();
            _repository.Update(user);
            return true;
        }

        public void SetActive(SiteUser user, bool isActive)
        {
            user.IsActive = isActive;
            user.SecurityStamp = GenerateSecurityStamp();
            _repository.Update(user);
        }

        public void DeleteUser(SiteUser user)
        {
            _repository.Delete(user);
        }

        public void PromoteToAdmin(SiteUser user)
        {
            user.Role = SiteRoles.Admin;
            user.IsActive = true;
            _repository.Update(user);
        }

        /// <summary>
        /// Une session devient invalide dès que le compte est désactivé, supprimé ou que son mot de passe change
        /// </summary>
        public bool IsSessionValid(ClaimsPrincipal principal)
        {
            if (!int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out int id))
            {
                return false;
            }
            SiteUser? user = _repository.GetById(id);
            return user != null && user.IsActive && user.SecurityStamp == principal.FindFirstValue(SecurityStampClaim);
        }

        private static string GeneratePassword()
        {
            return RandomNumberGenerator.GetString(PasswordAlphabet, GeneratedPasswordLength);
        }

        private static string GenerateSecurityStamp()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}

using Adelys_WebSite.Models;
using System.Security.Claims;

namespace Adelys_WebSite.BL.Interfaces
{
    public interface ISiteUserBL
    {
        List<SiteUser> GetAllUsers();
        SiteUser? GetUser(int id);
        SiteUser? GetUser(string username);
        SiteUser? Authenticate(string username, string password);
        string CreateUser(string username, string role);
        string ResetPassword(SiteUser user);
        bool ChangePassword(SiteUser user, string currentPassword, string newPassword);
        void SetActive(SiteUser user, bool isActive);
        void DeleteUser(SiteUser user);
        void PromoteToAdmin(SiteUser user);
        bool IsSessionValid(ClaimsPrincipal principal);
    }
}
